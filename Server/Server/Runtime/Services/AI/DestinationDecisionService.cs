using Plugin.Interfaces;
using Plugin.Runtime.Services.AI.Inputs;
using Plugin.Runtime.Services.AI.Outputs;
using Plugin.Runtime.Services.UnitsPath;
using Plugin.Schemes;
using System;
using System.Collections.Generic;

namespace Plugin.Runtime.Services.AI
{
    /// <summary>
    /// Сервіс, робота котрого приймати рішення,
    /// на скільки позиція хороша для переміщеня юніта в поточні координати
    /// </summary>
    public class DestinationDecisionService : BaseDecision
    {
        /// <summary>
        /// Вказати відсоток, із якою ймовірностью юніт змінить своє саме краще рішення по переміщенню
        /// </summary>
        private const int PERCENT_TO_CHANGE_THE_BEST_DECISION = 55;
        /// <summary>
        /// Вказати відсоток, із якою ймовірностью юніт залишиться на місті
        /// і не прийме ніодного рішення для переміщення
        /// </summary>
        private const int PERCENT_TO_DO_NOT_MOVE_WITHOUT_ANY_DECISION = 20;
        /// <summary>
        /// Вказати відсоток, із якою ймовірностью юніт залишиться на місті
        /// після всих прийнятих рішень 
        /// </summary>
        private const int PERCENT_TO_DO_NOT_MOVE_AFTER_ALL_DECISION = 30;

        private CellWalkableService _cellWalkableService;
        private UnitsService _unitsService;
        private GridService _gridService;
        private UnitsPathService _unitsPathService;

        // Allow
        private float _i1_w1 = 0f;   // на практиці не працює рішення
        private float _i2_w1 = 1f;

        private const int INPUT_SIZE = 2;
        /// <summary>
        /// Логіювати прийняте рішення
        /// </summary>
        private const bool _isLog = false;



        public DestinationDecisionService(CellWalkableService cellWalkableService,
                                          UnitsService unitsService,
                                          GridService gridService,
                                          UnitsPathService unitsPathService)
        {
            _cellWalkableService = cellWalkableService;
            _unitsService = unitsService;
            _gridService = gridService;
            _unitsPathService = unitsPathService;
        }

        /// <summary>
        /// 
        /// </summary>
        public T Decision<T>(IUnit unit, List<Cell> cellsSettleArea) where T : IValueDecision
        {
            // (output, positionW, positionH)
            var allDecisions = new List<(IOutput, int, int)>();
            Random random = new Random();

            foreach (Cell destination in cellsSettleArea)
            {
                // Отримати список із інпутів, котрі дозволяють змінити старого VIP-а на нового
                var inputs = new float[INPUT_SIZE];
                CalculateInputsSize(unit, destination, ref inputs);

                allDecisions.Add((new AllowOutput(inputs,
                                            new float[INPUT_SIZE] { _i1_w1, _i2_w1 },
                                            0f,
                                            _isLog),
                                            (int)destination.wIndex,
                                            (int)destination.hIndex));
            }

            allDecisions.Sort((x, y) => x.Item1.Sum.CompareTo(y.Item1.Sum));
            allDecisions.Reverse();

            // перед тим, як прийняти рішення, куди піти юніту,
            // перепровіряємо кожну позицію на той факт, що цю позицію міг зайняти інший юніт
            var bodyWidth = unit.BodySize.x;
            var freeDecisions = new List<(IOutput, int, int)>();
            foreach ((IOutput, int, int) d in allDecisions)
            {
                bool isFree = true;
                for (int i = 0; i < bodyWidth; i++)
                {
                    if (_cellWalkableService.IsIgnorePosition(unit.GameId, unit.OwnerActorNr, d.Item2 + i, d.Item3))
                    {
                        isFree = false;
                        break;
                    }
                }

                if (isFree)
                {
                    freeDecisions.Add(d);
                }
            }

            // lastDecision - кінцеве рішення, куди піде юніт
            (IOutput, int, int) lastDecision = freeDecisions[0];

            if (random.Next(0, 100) < PERCENT_TO_CHANGE_THE_BEST_DECISION)
            {
                // Беремо рішення із максимальною вагою і знаходимо такіж рішення із такою ж вагою
                // і рандомно вибираємо одне із них. Це потрібно, що би постійно не обирати одне і теж саме 
                // рішення із максимальною вагою
                List<(IOutput, int, int)> bestDecisions = allDecisions.FindAll(x => x.Item1.Sum == allDecisions[0].Item1.Sum);
                lastDecision = bestDecisions[random.Next(0, bestDecisions.Count)];
            }

            // Перевірка, якщо нова позиція така ж як і поточна позиція, 
            // і в списку рішень є інші позиції, то змінити рішення на нову позицію
            // Бо є проблема, що юніт знаходить ідеальну позицію і знаходиться в зоні комфорта 
            var positionOnGrid = unit.Position;
            if (random.Next(0, 100) < PERCENT_TO_DO_NOT_MOVE_AFTER_ALL_DECISION)
            {
                if (freeDecisions.Count >= 2
                    && positionOnGrid.x == lastDecision.Item2
                    && positionOnGrid.y == lastDecision.Item3)
                {
                    (IOutput, int, int) randomDecision;

                    do
                    {
                        randomDecision = freeDecisions[random.Next(0, freeDecisions.Count)];
                    } while (lastDecision.Item2 == randomDecision.Item2 && lastDecision.Item3 == randomDecision.Item3);

                    lastDecision = randomDecision;
                }
            }

           
            if (random.Next(0, 100) < PERCENT_TO_DO_NOT_MOVE_WITHOUT_ANY_DECISION)
            {
                lastDecision.Item2 = positionOnGrid.x;
                lastDecision.Item3 = positionOnGrid.y;
            }

            // Якщо є прийняте рішення, то знайти рішення із максимальною сумою інпутів
            T decision = Activator.CreateInstance<T>();
            decision.Output = lastDecision.Item1;
            decision.Values = new List<int>() { lastDecision.Item2, lastDecision.Item3 };

            return decision;
        }

        /// <summary>
        /// Отримати список із інпутів
        /// </summary>
        private void CalculateInputsSize(IUnit unit, Cell destination, ref float[] inputs)
        {
            // Створюємо всі інпути, котрі будуть впливати на рішення
            var allowInputs = new IInput[INPUT_SIZE]
            {
                new DistanceToNearUnitInput(unit, destination, _unitsService, _gridService),
                new NextWayAreaInput(unit, destination, _gridService, _unitsPathService),
            };

            // Створити массив із вагів інпута
            for (int i = 0; i < allowInputs.Length; i++)
            {
                inputs[i] = allowInputs[i].Size;
            }
        }
    }
}

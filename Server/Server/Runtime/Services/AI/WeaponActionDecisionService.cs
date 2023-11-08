using Plugin.Interfaces;
using Plugin.Interfaces.Actions;
using Plugin.Runtime.Services.AI.Inputs;
using Plugin.Runtime.Services.AI.Outputs;
using Plugin.Runtime.Services.UnitsPath;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Plugin.Runtime.Services.AI
{
    /// <summary>
    /// Сервіс, робота котрого приймати рішення, як саме і кого вказаний юніт буде атакувати,
    /// якщо вказаний юніт має ActionComponent
    /// </summary>
    public class WeaponActionDecisionService : BaseDecision, IWeaponActionDecisionService, IActionDecisionService
    {
        private UnitsService _unitsService;

        /// <summary>
        /// Вказати відсоток, при якому прийняте рішення, якого юніта потрібно атакувати, буде змінено
        /// </summary>
        private const int PERCENT_TO_CHANGE_DECISION = 50;

        // Allow
        private float _i1_w1 = 1f;
        private float _i2_w1 = 1f;
        private float _i3_w1 = 1f;

        private const int INPUT_SIZE = 3;
        /// <summary>
        /// Логіювати прийняте рішення
        /// </summary>
        private const bool _isLog = false;

        private UnitsPathService _unitsPathService;

        public WeaponActionDecisionService(UnitsService unitsService, UnitsPathService unitsPathService)
        {
            _unitsService = unitsService;
            _unitsPathService = unitsPathService;
        }

        /// <summary>
        /// Чи може поточний клас прийняти рішення кого атакувати для вказаного юніта?
        /// </summary>
        public bool CanDecision(IUnit unitHunter)
        {
            return unitHunter is IDamageAction;
        }

        /// <summary>
        /// Прийняти рішення, якого юніта потрібно атакувати
        /// </summary>
        /// <param name="unitHunter">юніт, котрий буде атакувати</param>
        /// <param name="targetActorId">id актора, юнітів котрого будемо атакувати</param>
        public T Decision<T>(IUnit unitHunter, string gameId, int targetActorNr) where T : IUnitDecision
        {
            var targetUnits = new List<IUnit>();
            _unitsService.GetAliveUnits(gameId, targetActorNr, ref targetUnits);

            var decisions = new List<(IOutput, IUnit, IUnit)>();

            // Перебираємо кожного юніта гравця противника і для кожного юніта вираховуємо інпути
            for (int i = 0; i < targetUnits.Count; i++)
            {
                var inputs = new float[INPUT_SIZE];
                CalculateInputsSize(targetUnits[i], ref inputs);

                decisions.Add((new AllowOutput(inputs,
                                               new float[INPUT_SIZE] { _i1_w1, _i2_w1, _i3_w1 },
                                               0f,
                                               _isLog),
                                               unitHunter,
                                               targetUnits[i]));
            }

            (IOutput, IUnit, IUnit) decisionData = GetDecision(decisions);

            // Якщо є прийняте рішення, то знайти рішення із максимальною сумою інпутів
            T decision = Activator.CreateInstance<T>();
            decision.Output = decisionData.Item1;
            decision.Units = new List<IUnit>() { decisionData.Item2, decisionData.Item3 };

            return decision;
        }

        private (IOutput, IUnit, IUnit) GetDecision(List<(IOutput, IUnit, IUnit)> decision)
        {
            if (decision.Any(x => x.Item1.IsActive))
            {
                var activeList = decision.FindAll(x => x.Item1.IsActive);
                if (activeList.Count == 1)
                    return activeList[0];

                activeList.Sort((x, y) => x.Item1.Sum.CompareTo(y.Item1.Sum));
                activeList.Reverse();

                Random random = new Random();

                if (activeList.Count > 2 && random.Next(0, 100) < PERCENT_TO_CHANGE_DECISION)
                {
                    return activeList[random.Next(1, activeList.Count)];
                }

                return activeList[0];
            }

            return (new EmptyOutput(), null, null);    // ніодне рішення не було прийнято, або всі рішення мають однакову суму прийняття рішення
        }

        /// <summary>
        /// Отримати список із інпутів
        /// unit - юніт, котрого будемо атакувати
        /// </summary>
        private void CalculateInputsSize(IUnit unit, ref float[] inputs)
        {
            // Створюємо всі інпути, котрі будуть впливати на рішення
            var allowInputs = new IInput[INPUT_SIZE]
            {
                new IsVipInput(unit),
                new WayAreaInput(unit, _unitsPathService),
                new WasDamagedInput(unit, _unitsService),
            };

            // Створити массив із вагів інпута
            for (int i = 0; i < allowInputs.Length; i++)
            {
                inputs[i] = allowInputs[i].Size;
            }
        }
    }
}

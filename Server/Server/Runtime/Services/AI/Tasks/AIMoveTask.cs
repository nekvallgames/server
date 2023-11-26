using Plugin.Interfaces;
using Plugin.Schemes;
using System.Collections.Generic;

namespace Plugin.Runtime.Services.AI.Tasks
{
    /// <summary>
    /// Виконувач AI, котрий прийме рішення і виконає дії, як саме потрібно переставити юнітів
    /// </summary>
    public class AIMoveTask : IAiTask
    {
        public const string TASK_NAME = "AIMoveTask";
        public string Name => TASK_NAME;

        private UnitsService _unitsService;
        private CellWalkableService _cellWalkableService;
        private PathService _pathService;
        private DestinationDecisionService _destinationDecisionService;
        private SyncRoomService _syncRoomService;

        public AIMoveTask(UnitsService unitsService,
                          CellWalkableService cellWalkableService,
                          PathService pathService,
                          DestinationDecisionService destinationDecisionService,
                          SyncRoomService syncRoomService)
        {
            _unitsService = unitsService;
            _cellWalkableService = cellWalkableService;
            _pathService = pathService;
            _destinationDecisionService = destinationDecisionService;
            _syncRoomService = syncRoomService;
        }

        public void ExecuteTask(string gameId, int actorNr, int stepNumber)
        {
            var candidates = new List<IUnit>();
            _unitsService.GetAliveUnits(gameId, actorNr, ref candidates);

            if (candidates.Count <= 0)
            {
                return;    // в гравця не має взагалі юнітів, котрих будемо переміщати
            }

            // Высчитать путь, куда может переместится каждый юнит AI игрока
            _pathService.Calculate(gameId, actorNr);

            _cellWalkableService.ClearIgnoreList(gameId, actorNr);    // ???

            // перебираємо кожного юніта, та вираховуємо куди він переміститься
            foreach (IUnit unit in candidates)
            {
                // за допомогою сервісу CellWalkableService вираховуємо, куди може переміститься юніт
                _cellWalkableService.Calculate(unit);

                IValueDecision decision = _destinationDecisionService.Decision<ValueDecisionScheme>(unit, _cellWalkableService.Get(gameId, actorNr).CellsSettleArea);
                int posW = decision.Values[0];
                int posH = decision.Values[1];

                // На ширину юніта добавити сели в ігнор ліст
                // Що би наступний юніт не встав в одні і тіж координати
                int bodyWidth = unit.BodySize.x;
                for (int i = 0; i < bodyWidth; i++)
                {
                    _cellWalkableService.AddPositionToIgnoreList(gameId, actorNr, (posW + i, posH));
                }

                _syncRoomService.SyncPositionOnGrid(unit, posW, posH);
            }

            _cellWalkableService.ClearIgnoreList(gameId, actorNr);
        }
    }
}

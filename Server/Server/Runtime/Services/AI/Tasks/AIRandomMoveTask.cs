using Plugin.Interfaces;
using Plugin.Schemes;
using System;
using System.Collections.Generic;

namespace Plugin.Runtime.Services.AI.Tasks
{
    /// <summary>
    /// Виконувач AI, котрий прийме рішення і виконає дії,
    /// як саме рандомно переставити юнітів, без ніякої логіки
    /// </summary>
    public class AIRandomMoveTask : IAiTask
    {
        public const string TASK_NAME = "AIRandomMoveTask";
        public string Name => TASK_NAME;

        private UnitsService _unitsService;
        private CellWalkableService _cellWalkableService;
        private PathService _pathService;
        private DestinationDecisionService _destinationDecisionService;
        private ActorService _actorService;
        private SyncRoomService _syncRoomService;

        public AIRandomMoveTask(UnitsService unitsService,
                                CellWalkableService cellWalkableService,
                                PathService pathService,
                                DestinationDecisionService destinationDecisionService,
                                ActorService actorService,
                                SyncRoomService syncRoomService)
        {
            _unitsService = unitsService;
            _cellWalkableService = cellWalkableService;
            _pathService = pathService;
            _destinationDecisionService = destinationDecisionService;
            _actorService = actorService;
            _syncRoomService = syncRoomService;
        }

        public void ExecuteTask(string gameId)
        {
            int aiActorNr = _actorService.GetAiActor(gameId).ActorNr;

            var candidates = new List<IUnit>();
            _unitsService.GetAliveUnits(gameId, aiActorNr, ref candidates);

            if (candidates.Count <= 0){
                return;    // в гравця не має взагалі юнітів, котрих будемо переміщати
            }

            // Высчитать путь, куда может переместится каждый юнит AI игрока
            _pathService.Calculate(gameId, aiActorNr);

            // за допомогою сервісу CellWalkableService вираховуємо, куди може переміститься юніт
            _cellWalkableService.ClearIgnoreList(gameId, aiActorNr);
            _cellWalkableService.Calculate(candidates[0]);

            Random random = new Random();
            CellWalkablePrivateScheme cellWalkablePrivateScheme = _cellWalkableService.Get(gameId, aiActorNr);

            Cell position =
                cellWalkablePrivateScheme.CellsSettleArea[random.Next(0, cellWalkablePrivateScheme.CellsSettleArea.Count)];

            _syncRoomService.SyncPositionOnGrid(candidates[0], (int)position.wIndex, (int)position.hIndex);
        }
    }
}

using Plugin.Interfaces;

namespace Plugin.Runtime.Services.AI.Tasks
{
    /// <summary>
    /// Виконувач AI, котрий прийме рішення і виконає дії,
    /// як саме синхронізує позиції юнітів на ігровій сітці
    /// </summary>
    public class AIPositionTask : IAiTask
    {
        public const string TASK_NAME = "AIPositionTask";
        public string Name => TASK_NAME;

        private UnitsService _unitsService;
        private ActorService _actorService;
        private SyncRoomService _syncRoomService;

        public AIPositionTask(UnitsService unitsService, 
                              ActorService actorService, 
                              SyncRoomService syncRoomService)
        {
            _unitsService = unitsService;
            _actorService = actorService;
            _syncRoomService = syncRoomService;
        }

        public void ExecuteTask(string gameId)
        {
            int aiActorNr = _actorService.GetAiActor(gameId).ActorNr;

            foreach (IUnit unit in _unitsService.GetUnits(gameId, aiActorNr))
            {
                if (unit is IBarrierComponent)
                {
                    // TODO поточний юніт являється преградою, його не потрібно переміщати
                    continue;
                }

                _syncRoomService.SyncPositionOnGrid(unit);
            }
        }
    }
}

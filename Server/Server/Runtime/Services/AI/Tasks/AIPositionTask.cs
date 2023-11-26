using Plugin.Interfaces;
using System.Collections.Generic;

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
        private SyncRoomService _syncRoomService;

        public AIPositionTask(UnitsService unitsService, 
                              SyncRoomService syncRoomService)
        {
            _unitsService = unitsService;
            _syncRoomService = syncRoomService;
        }

        public void ExecuteTask(string gameId, int actorNr, int stepNumber)
        {
            List<IUnit> units = _unitsService.GetUnits(gameId, actorNr);

            foreach (IUnit unit in units)
            {
                if (unit is IBarrierComponent){
                    // TODO поточний юніт являється преградою, його не потрібно переміщати
                    continue;
                }

                _syncRoomService.SyncPositionOnGrid(unit, stepNumber);
            }
        }
    }
}

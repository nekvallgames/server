using Plugin.Interfaces;
using Plugin.Interfaces.Actions;
using System;
using System.Collections.Generic;

namespace Plugin.Runtime.Services.AI
{
    /// <summary>
    /// Сервіс, котрий імітує синхронізацію єкшенів вказаного юніта
    /// Це потрібно для юнітів AI. Ми вказаєто юніта AI, якого юніта потрібно
    /// атакувати, і поточний сервіс виконаю логіку атаки
    /// </summary>
    public class SimulateSyncActionService
    {
        private SyncRoomService _syncRoomService;
        private CellTemperatureTraceService _temperatureWalkableTraceService;
        private HitAreaService _hitAreaService;

        public SimulateSyncActionService(SyncRoomService syncRoomService,
                                         CellTemperatureTraceService temperatureWalkableTraceService,
                                         HitAreaService hitAreaService)
        {
            _syncRoomService = syncRoomService;
            _temperatureWalkableTraceService = temperatureWalkableTraceService;
            _hitAreaService = hitAreaService;
        }

        /// <summary>
        /// Виконати для аказаного юніта по вказаному юніту всі єкшени
        /// (Скоріше за все, це будуть всі єкшени атаки)
        /// </summary>
        public void SimulateAllActions(IUnit unitHunter, IUnit unitTarget)
        {
            // кількість єкшенів юніта, котрий буде атакувати
            //var hunterActionCapacity = _entityManager.GetComponentData<ActionCapacityComponent>(unitHunter.UnitEntity);
            var hunterActionCapacity = (unitHunter as IDamageAction).ActionCapacity;

            // карта селів, в котрих може знаходитись тіло юніта цілі
            var targetHitArea = new List<(int, int)>();
            _hitAreaService.GetWayBodyCellsArea(unitTarget, ref targetHitArea);

            var targetTemperatureArea = new List<(int, int)>();
            _temperatureWalkableTraceService.GetTrace(unitTarget.GameId, unitTarget.OwnerActorNr, ref targetHitArea, ref targetTemperatureArea, 1.5f);

            var hitPositions = new List<(int, int)>();
            if (targetTemperatureArea.Count > 0)
            {
                hitPositions.AddRange(targetTemperatureArea);
            }
            else
            {
                hitPositions.AddRange(targetHitArea);
            }

            Random random = new Random();

            for (int i = 0; i < hunterActionCapacity; i++)
            {
                int rPosition = random.Next(0, hitPositions.Count);

                _syncRoomService.SyncAction(unitHunter.GameId,
                                            unitHunter.OwnerActorNr,
                                            unitHunter.UnitId,
                                            unitHunter.InstanceId,
                                            unitTarget.OwnerActorNr,
                                            hitPositions[rPosition].Item1,
                                            hitPositions[rPosition].Item2);
            }
        }
    }
}

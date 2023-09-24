using Plugin.Interfaces;
using Plugin.Interfaces.UnitComponents;
using Plugin.Runtime.Services.Sync;
using Plugin.Runtime.Services.Sync.Groups;
using Plugin.Tools;

namespace Plugin.Runtime.Services.ExecuteAction
{
    /// <summary>
    /// Сервіс, котрий переміщає та позиціонує юнітів по ігровій сітці
    /// </summary>
    public class MoveService
    {
        private SyncService _syncService;

        public MoveService(SyncService syncService)
        {
            _syncService = syncService;
        }

        /// <summary>
        /// Позиционировать юнита в точке posW, posH на игровой сетке,
        /// без каких либо проверок, может юнит туда дойти или нет
        /// </summary>
        public void PositionOnGrid(IUnit unit, int posW, int posH)
        {
            unit.Position = new Int2(posW, posH);

            if (unit is IIgnoreSyncComponent){
                return;     // для поточного юніта не потрібно синхронізувати позіцію
            }

            // Синхронизировать позицию юнита на игровой сетке
            var syncData = new SyncPositionOnGridGroup(unit);
            _syncService.Add(unit.GameId, unit.OwnerActorNr, syncData);
        }
    }
}

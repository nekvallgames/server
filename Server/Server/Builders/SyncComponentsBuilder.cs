using Plugin.Interfaces;
using Plugin.Interfaces.UnitComponents;
using Plugin.OpComponents;

namespace Plugin.Builders
{
    /// <summary>
    /// Билдер, для созранения елементов истории действий
    /// </summary>
    public class SyncComponentsBuilder
    {
        public IGroupSyncComponents GroupSyncComponents { get; private set; }

        public static SyncComponentsBuilder Build(IGroupSyncComponents groupSyncComponents)
        {
            return new SyncComponentsBuilder
            {
                GroupSyncComponents = groupSyncComponents
            };
        }

        /// <summary>
        /// Сохранить уникальный идентификатор юнита
        /// </summary>
        public SyncComponentsBuilder SyncUnitId(int unitId, int instanceId)
        {
            var component = new UnitIdOpComponent
            {
                uid = unitId,
                i = instanceId
            };

            GroupSyncComponents.SyncComponents.Add(component);
            return this;
        }

        /// <summary>
        /// Сохранить точку, в которою выстрелил игрок
        /// </summary>
        public SyncComponentsBuilder SyncAction(int targetActorId, int cellW, int cellH)
        {
            var component = new ActionOpComponent
            {
                tai = targetActorId,
                w = cellW,
                h = cellH
            };

            GroupSyncComponents.SyncComponents.Add(component);
            return this;
        }


        /// <summary>
        /// Сохранить точку, в которою юнит применил дополнительное (пассивное) действие
        /// </summary>
        public SyncComponentsBuilder SyncAdditionalByPos(int targetActorId, int cellW, int cellH)
        {
            var component = new AdditionalByPosOpComponent
            {
                tai = targetActorId,
                w = cellW,
                h = cellH
            };

            GroupSyncComponents.SyncComponents.Add(component);
            return this;
        }

        /// <summary>
        /// Сохранить Id юніта, к которому скорее всего
        /// было выполнено текущее действие игрока
        /// </summary>
        public SyncComponentsBuilder SyncAdditionalByUnit(int actorId, int targetUnitId, int targetUnitInstanceId)
        {
            var component = new AdditionalByUnitOpComponent
            {
                tai = actorId,
                uid = targetUnitId,
                iid = targetUnitInstanceId
            };

            GroupSyncComponents.SyncComponents.Add(component);
            return this;
        }

        /// <summary>
        /// Сохранить позицию на игровой сетке
        /// </summary>
        public SyncComponentsBuilder SyncPositionOnGrid(int cellW, int cellH)
        {
            var component = new PositionOnGridOpComponent
            {
                w = cellW,
                h = cellH
            };

            GroupSyncComponents.SyncComponents.Add(component);
            return this;
        }

        /// <summary>
        /// Созранить позицию на игровой сетке
        /// </summary>
        public SyncComponentsBuilder SyncPositionOnGrid(IUnit unit)
        {
            var posOnGrid = unit.Position;
            SyncPositionOnGrid(posOnGrid.x, posOnGrid.y);

            return this;
        }

        /// <summary>
        /// Сохранить изменение, текущий юнит VIP или нет
        /// </summary>
        public SyncComponentsBuilder SyncVip(IUnit unit)
        {
            bool isVip = (unit is IVipComponent) ? (unit as IVipComponent).IsVip : false;

            var component = new VipOpComponent
            {
                e = isVip
            };

            GroupSyncComponents.SyncComponents.Add(component);
            return this;
        }

        /// <summary>
        /// Симулювати, поточний юніт VIP чи ні
        /// </summary>
        public SyncComponentsBuilder SimulateSyncVip(IUnit unit, bool isVip)
        {
            var component = new VipOpComponent
            {
                e = true
            };

            GroupSyncComponents.SyncComponents.Add(component);
            return this;
        }
    }
}

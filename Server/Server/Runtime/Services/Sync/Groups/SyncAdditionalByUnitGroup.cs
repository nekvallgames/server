using Plugin.Builders;
using Plugin.Interfaces;
using System.Collections.Generic;

namespace Plugin.Runtime.Services.Sync.Groups
{
    /// <summary>
    /// Игрок на стороне сервера применил дополнительное (пассивное) действие юнита
    /// Создать группу из компонентов, которые нужны, что бы синхронизировать выполненное действие между клиентами
    /// </summary>
    public class SyncAdditionalByUnitGroup : ISyncGroupComponent
    {
        public List<ISyncComponent> SyncElements { get; }

        public SyncAdditionalByUnitGroup(IUnit unit, int targetActorId, int targetUnitId, int targetInstanceId)
        {
            SyncElements = new List<ISyncComponent>();

            var syncElements = SyncElementBuilder
               .Build(this)
               .SyncUnitId(unit.UnitId, unit.InstanceId)
               .SyncAdditionalByUnit(targetActorId, targetUnitId, targetInstanceId);
        }


    }
}

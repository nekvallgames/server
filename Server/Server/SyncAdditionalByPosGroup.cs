using Plugin.Builders;
using Plugin.Interfaces;
using System.Collections.Generic;

namespace Plugin.Runtime.Services.Sync.Groups
{
    /// <summary>
    /// Игрок на стороне сервера применил дополнительное (пассивное) действие юнита
    /// Создать группу из компонентов, которые нужны, что бы синхронизировать выполненное действие между клиентами
    /// </summary>
    public class SyncAdditionalByPosGroup : ISyncGroupComponent
    {
        public List<ISyncComponent> SyncElements { get; }

        public SyncAdditionalByPosGroup(IUnit unit, int targetActorId, int positionOnGridW, int positionOnGridH)
        {
            SyncElements = new List<ISyncComponent>();

            var syncElements = SyncElementBuilder
               .Build(this)
               .SyncUnitId(unit.UnitId, unit.InstanceId)
               .SyncAdditionalByPos(targetActorId, positionOnGridW, positionOnGridH);
        }


    }
}

using Plugin.Builders;
using Plugin.Interfaces;
using Plugin.Interfaces.UnitComponents;
using System.Collections.Generic;

namespace Plugin.Runtime.Services.Sync.Groups
{
    /// <summary>
    /// Игрок на стороне сервера активировал/деактивировал Vip
    /// Создать группу из компонентов, которые нужны, что бы синхронизировать выполненное действие между клиентами
    /// </summary>
    public class SyncVipGroup : ISyncGroupComponent
    {
        public List<ISyncComponent> SyncElements { get; }

        public SyncVipGroup(IUnit unit)
        {
            SyncElements = new List<ISyncComponent>();

            var syncElements = SyncElementBuilder
               .Build(this)
               .SyncUnitId(unit.UnitId, unit.InstanceId)
               .SyncVip((unit as IVipComponent).IsVip);
        }
    }
}

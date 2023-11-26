using Plugin.Builders;
using Plugin.Interfaces;
using System.Collections.Generic;

namespace Plugin.Runtime.Services.Sync.Groups
{
    /// <summary>
    /// Игрок на стороне сервера переместил юнита на игровой сетке
    /// Создать группу из компонентов, которые нужны, что бы синхронизировать перемещение между клиентами
    /// </summary>
    public struct SyncPositionOnGridGroup : ISyncGroupComponent
    {
        public List<ISyncComponent> SyncElements { get; }

        public SyncPositionOnGridGroup(IUnit unit)
        {
            SyncElements = new List<ISyncComponent>();

            var syncElements = SyncElementBuilder
               .Build(this)
               .SyncUnitId(unit.UnitId, unit.InstanceId)
               .SyncPositionOnGrid(unit.Position);
        }
    }
}

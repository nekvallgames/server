using Plugin.Interfaces;
using Plugin.Interfaces.UnitComponents;
using Plugin.Tools;

namespace Plugin.Runtime.Units
{
    /// <summary>
    /// Обычный юнит барриер. Стенка из металического забора с сеткой
    /// </summary>
    public class UnitIronFenceBarrier : BaseUnit, IHealthComponent, IUnit, IIgnoreSyncComponent
    {
        public const int UnitId = 36;

        public override Int2 BodySize => new Int2(4, 2);

        int IHealthComponent.Capacity { get; set; }
        int IHealthComponent.CapacityMax => 100;    // Количество жизней юнита на старте игры

        public UnitIronFenceBarrier(string gameId, int ownerActorId, int unitId, int instanceUnitId) : base(gameId, ownerActorId, unitId, instanceUnitId)
        {

        }
    }
}


using Plugin.Interfaces;
using Plugin.Interfaces.UnitComponents;
using Plugin.Tools;

namespace Plugin.Runtime.Units
{
    /// <summary>
    /// Обычный юнит барриер. Металева бочка
    /// </summary>
    public class UnitBarrel : BaseUnit, IHealthComponent, IUnit, IIgnoreSyncComponent
    {
        public const int UnitId = 6;

        public override Int2 BodySize => new Int2(2, 3);

        int IHealthComponent.Capacity { get; set; }
        int IHealthComponent.CapacityMax => 100;    // Количество жизней юнита на старте игры

        public UnitBarrel(string gameId, int ownerActorId, int unitId, int instanceUnitId) : base(gameId, ownerActorId, unitId, instanceUnitId)
        {

        }
    }
}


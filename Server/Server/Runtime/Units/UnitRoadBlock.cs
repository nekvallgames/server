using Plugin.Interfaces;
using Plugin.Interfaces.UnitComponents;
using Plugin.Tools;

namespace Plugin.Runtime.Units
{
    /// <summary>
    /// Обычный юнит барриер. Дорожня переграда
    /// </summary>
    public class UnitRoadBlock : BaseUnit, IHealthComponent, IUnit, IIgnoreSyncComponent
    {
        public const int UnitId = 8;

        public override Int2 BodySize => new Int2(4, 2);

        int IHealthComponent.Capacity { get; set; }
        int IHealthComponent.CapacityMax => 100;    // Количество жизней юнита на старте игры

        public UnitRoadBlock(string gameId, int ownerActorId, int unitId, int instanceUnitId) : base(gameId, ownerActorId, unitId, instanceUnitId)
        {

        }
    }
}

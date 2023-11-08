using Plugin.Interfaces;
using Plugin.Interfaces.UnitComponents;
using Plugin.Parameters;
using Plugin.Schemes;
using Plugin.Tools;

namespace Plugin.Runtime.Units
{
    /// <summary>
    /// Обычный юнит барриер. Контейнер із сміттям
    /// </summary>
    public class UnitTrash : BaseUnit, IHealthComponent, IUnit, IIgnoreSyncComponent, IBarrierComponent
    {
        public const int UnitId = 7;

        public override Int2 BodySize => new Int2(4, 3);
        public override PartBodyScheme[] AreaGrid => new PartBodyScheme[] {
            new PartBodyScheme(0,2, Enums.PartBody.head), new PartBodyScheme(1,2, Enums.PartBody.head), new PartBodyScheme(2,2, Enums.PartBody.head), new PartBodyScheme(3,2, Enums.PartBody.head),
            new PartBodyScheme(0,1, Enums.PartBody.head), new PartBodyScheme(1,1, Enums.PartBody.head), new PartBodyScheme(2,1, Enums.PartBody.head), new PartBodyScheme(3,1, Enums.PartBody.head),
            new PartBodyScheme(0,0, Enums.PartBody.body), new PartBodyScheme(1,0, Enums.PartBody.body), new PartBodyScheme(2,0, Enums.PartBody.body), new PartBodyScheme(3,0, Enums.PartBody.body)
        };

        public int HealthCapacity { get; set; }
        public int HealthCapacityMax { get; private set; }    // Количество жизней юнита на старте игры

        public override bool IsHumanoid => false;
        public UnitTrash(UnitFactoryParameters parameters) : base(parameters)
        {
            // Set health
            int health = unitPublicScheme.health + increaseUnitHealthService.GetAdditionalHealthByLevel(UnitId, Level);
            HealthCapacity = health;
            HealthCapacityMax = health;
        }
    }
}

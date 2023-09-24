using Plugin.Interfaces;
using Plugin.Interfaces.UnitComponents;
using Plugin.Parameters;
using Plugin.Schemes;
using Plugin.Tools;

namespace Plugin.Runtime.Units
{
    /// <summary>
    /// Юнит с огнестрельным дробовиком
    /// </summary>
    public class UnitUmp45 : BaseActionUnit, 
        IHealthComponent, 
        IHealthBuffComponent, 
        IArmorComponent, 
        IVipComponent, 
        IHealingAdditional,
        IWalkableComponent,
        INavigationWayForDuelComponent,
        IGodModeWalkableComponent
    {
        public const int UnitId = 15;

        public override Int2 BodySize => new Int2(2, 5);
        public override PartBodyScheme[] AreaGrid => new PartBodyScheme[] {
            new PartBodyScheme(0,4, Enums.PartBody.head), new PartBodyScheme(1,4, Enums.PartBody.head),
            new PartBodyScheme(0,3, Enums.PartBody.head), new PartBodyScheme(1,3, Enums.PartBody.head),
            new PartBodyScheme(0,2, Enums.PartBody.body), new PartBodyScheme(1,2, Enums.PartBody.body),
            new PartBodyScheme(0,1, Enums.PartBody.bottom), new PartBodyScheme(1,1, Enums.PartBody.bottom),
            new PartBodyScheme(0,0, Enums.PartBody.bottom), new PartBodyScheme(1,0, Enums.PartBody.bottom)
        };

        public int HealthCapacity { get; set; }
        public int HealthCapacityMax { get; private set; }    // Количество жизней юнита на старте игры

        public int ArmorCapacity { get; set; }
        public int ArmorCapacityMax { get; }    // Количество броні юнита на старте игры

        public override int OriginalDamage { get; }
        public override int OriginalActionCapacity { get; set; }


        public override Int2[] ActionArea => new Int2[] { new Int2(0, 0), new Int2(0, 1), new Int2(1, 2) };

        public bool IsVip { get; set; }
        public int HealthBuffCapacity { get; set; }

        public int WayMask => 6;
        public Enums.WalkNavigation NavigationWay { get; set; } = Enums.WalkNavigation.body_width_2;
        public Enums.WalkNavigation NavigationWayForDuel => Enums.WalkNavigation.body_width_2_for_duel;

        public bool HasGodModeWalkable { get; set; }

        #region additional

        private int _additionalCapacity;
        private int _additionalPower;

        public bool CanExecuteAdditional()
        {
            return _additionalCapacity > 0;
        }

        public int GetHealthPower()
        {
            return _additionalPower;
        }

        public void SpendAdditional()
        {
            _additionalCapacity--;
            if (_additionalCapacity < 0)
                _additionalCapacity = 0;
        }

        public Int2[] GetAdditionalArea() => new Int2[] { new Int2(0, 0) };

        #endregion

        public UnitUmp45(UnitFactoryParameters parameters) : base(parameters)
        {
            // Set health
            int health = unitPublicScheme.health + increaseUnitHealthService.GetAdditionalHealthByLevel(UnitId, Level);
            HealthCapacity = health;
            HealthCapacityMax = health;

            // Set damage
            int damage = unitPublicScheme.damage + increaseUnitDamageService.GetAdditionalDamageByLevel(UnitId, Level);
            Damage = damage;
            OriginalDamage = damage;

            // Set damage capacity
            ActionCapacity = unitPublicScheme.capacity;
            OriginalActionCapacity = unitPublicScheme.capacity;

            // Set additional 
            _additionalCapacity = unitPublicScheme.additionalCapacity;
            _additionalPower = unitPublicScheme.additionalPower;

            // Set damage
            ArmorCapacity = unitPublicScheme.armor;
            ArmorCapacityMax = unitPublicScheme.armor;
        }
    }
}

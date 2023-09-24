using Plugin.Interfaces;
using Plugin.Interfaces.UnitComponents;
using Plugin.Parameters;
using Plugin.Schemes;
using Plugin.Tools;

namespace Plugin.Runtime.Units
{
    /// <summary>
    /// Обычный юнит с огнестрельным пистолетом
    /// </summary>
    public class UnitPistol : BaseActionUnit, 
        IHealthComponent, 
        IHealthBuffComponent, 
        IVipComponent, 
        IHealingAdditional, 
        IWalkableComponent,
        INavigationWayForDuelComponent,
        IGodModeWalkableComponent
    {
        public const int UnitId = 0;

        public override Int2 BodySize => new Int2(1, 4);

        public override PartBodyScheme[] AreaGrid => new PartBodyScheme[] { 
            new PartBodyScheme(0,3, Enums.PartBody.head),
            new PartBodyScheme(0,2, Enums.PartBody.body),
            new PartBodyScheme(0,1, Enums.PartBody.bottom),
            new PartBodyScheme(0,0, Enums.PartBody.bottom)
        };

        public int HealthCapacity { get; set; }
        public int HealthCapacityMax { get; private set; }    // Количество жизней юнита на старте игры

        public override int OriginalDamage { get; }
        public override int OriginalActionCapacity { get; set; }

        public override Int2[] ActionArea => new Int2[] { new Int2(0, 0) };

        public bool IsVip { get; set; }
        public int HealthBuffCapacity { get; set; }

        public int WayMask => 6;
        public Enums.WalkNavigation NavigationWay { get; set; } = Enums.WalkNavigation.body_width_1;
        public Enums.WalkNavigation NavigationWayForDuel => Enums.WalkNavigation.body_width_1_for_duel;

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

        public UnitPistol(UnitFactoryParameters parameters) : base(parameters)
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
        }
    }
}

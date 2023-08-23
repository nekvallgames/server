﻿using Plugin.Interfaces;
using Plugin.Interfaces.UnitComponents;
using Plugin.Parameters;
using Plugin.Schemes;
using Plugin.Tools;

namespace Plugin.Runtime.Units
{
    /// <summary>
    /// Юнит с огнестрельным дробовиком
    /// </summary>
    public class UnitShotGun : BaseActionUnit, IHealthComponent, IHealthBuffComponent, IVipComponent, IHealingAdditional
    {
        public const int UnitId = 1;

        public override Int2 BodySize => new Int2(3, 5);
        public override PartBodyScheme[] AreaGrid => new PartBodyScheme[] {
            new PartBodyScheme(0,4, Enums.PartBody.head), new PartBodyScheme(1,4, Enums.PartBody.head), new PartBodyScheme(2,4, Enums.PartBody.head),
            new PartBodyScheme(0,3, Enums.PartBody.head), new PartBodyScheme(1,3, Enums.PartBody.head), new PartBodyScheme(2,3, Enums.PartBody.head),
            new PartBodyScheme(0,2, Enums.PartBody.body), new PartBodyScheme(1,2, Enums.PartBody.body), new PartBodyScheme(2,2, Enums.PartBody.body),
            new PartBodyScheme(0,1, Enums.PartBody.bottom), new PartBodyScheme(1,1, Enums.PartBody.bottom), new PartBodyScheme(2,1, Enums.PartBody.bottom),
            new PartBodyScheme(0,0, Enums.PartBody.bottom), new PartBodyScheme(1,0, Enums.PartBody.bottom), new PartBodyScheme(2,0, Enums.PartBody.bottom),
        };

        public int HealthCapacity { get; set; }
        public int HealthCapacityMax { get; private set; }    // Количество жизней юнита на старте игры

        public override int OriginalDamage { get; }
        public override int OriginalActionCapacity { get; set; }

        public override Int2[] ActionArea => new Int2[] { new Int2(-1, 1), new Int2(0, 0), new Int2(1, 1) };

        public bool IsVip { get; set; }
        public int HealthBuffCapacity { get; set; }

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

        public UnitShotGun(UnitFactoryParameters parameters) : base(parameters)
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

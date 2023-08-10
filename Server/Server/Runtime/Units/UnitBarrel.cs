﻿using Plugin.Interfaces;
using Plugin.Interfaces.UnitComponents;
using Plugin.Parameters;
using Plugin.Schemes;
using Plugin.Tools;

namespace Plugin.Runtime.Units
{
    /// <summary>
    /// Обычный юнит барриер. Металева бочка
    /// </summary>
    public class UnitBarrel : BaseUnit, IHealthComponent, IUnit, IIgnoreSyncComponent, IBarrier
    {
        public const int UnitId = 6;

        public override Int2 BodySize => new Int2(2, 3);
        public override PartBodyScheme[] AreaGrid => new PartBodyScheme[] {
            new PartBodyScheme(0,2, Enums.PartBody.body), new PartBodyScheme(1,2, Enums.PartBody.body),
            new PartBodyScheme(0,1, Enums.PartBody.body), new PartBodyScheme(1,1, Enums.PartBody.body),
            new PartBodyScheme(0,0, Enums.PartBody.body), new PartBodyScheme(1,0, Enums.PartBody.body)
        };

        public int HealthCapacity { get; set; }
        public int HealthCapacityMax { get; private set; }    // Количество жизней юнита на старте игры

        public UnitBarrel(UnitFactoryParameters parameters) : base(parameters)
        {
            // Set health
            int health = unitPublicScheme.health + increaseUnitHealthService.GetAdditionalHealthByLevel(UnitId, Level);
            HealthCapacity = health;
            HealthCapacityMax = health;
        }
    }
}


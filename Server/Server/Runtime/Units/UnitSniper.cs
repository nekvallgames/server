using Plugin.Interfaces;
using Plugin.Interfaces.UnitComponents;
using Plugin.Tools;

namespace Plugin.Runtime.Units
{
    /// <summary>
    /// Юнит с огнестрельной снайперской винтовкой
    /// </summary>
    public class UnitSniper : BaseDamageActionUnit, IHealthComponent, IVipComponent, IHealingAdditional
    {
        public const int UnitId = 3;

        public override Int2 BodySize => new Int2(2, 5);

        int IHealthComponent.Capacity { get; set; }
        int IHealthComponent.CapacityMax => 100;    // Количество жизней юнита на старте игры

        public override int OriginalPower => 0;
        public override int OriginalCapacity { get; set; } = 999;

        public override Int2[] DamageActionArea => new Int2[] { new Int2(0, 0) };

        bool IVipComponent.Enable { get; set; }

        #region additional

        private int _additionalCapacity = 2;

        public bool CanExecuteAdditional()
        {
            return _additionalCapacity > 0;
        }

        public short GetHealthPower()
        {
            return 30;
        }

        public void SpendAdditional()
        {
            _additionalCapacity--;
        }

        public Int2[] GetAdditionalArea() => new Int2[] { new Int2(0, 0) };

        #endregion

        public UnitSniper(string gameId, int ownerActorId, int unitId, int instanceUnitId) : base(gameId, ownerActorId, unitId, instanceUnitId)
        {

        }
    }
}

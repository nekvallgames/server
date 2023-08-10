using Plugin.Interfaces;
using Plugin.Schemes;
using Plugin.Tools;

namespace Plugin.Runtime.Services
{
    /// <summary>
    /// Класс сервис, который конвертирует наносимый урон по юниту,
    /// учитывая часть тела, к которую нанесли урон
    /// То есть, урон в голову юнита, будет больше, чем урон по ногам юнита
    /// </summary>
    public class BodyDamageConverterService
    {
        private UnitDamageMultiplicationService _unitDamageMultiplicationService;

        public BodyDamageConverterService(UnitDamageMultiplicationService unitDamageMultiplicationService)
        {
            _unitDamageMultiplicationService = unitDamageMultiplicationService;
        }

        /// <summary>
        /// Конвертировать урон по части тела
        /// entity - сущность, по которой нужно нанести урон
        /// damage - урон, который нужно нанести
        /// gridHitW - позиция попадания в сущность на игровой сетке
        /// gridHitH - позиция попадания в сущность на игровой сетке
        /// </summary>
        public int Converter(IUnit unit, int damage, int gridHitW, int gridHitH)
        {
            int damageByBody = 0;

            // позиция юнита на игровой сетке
            var entityPositionOnGrid = unit.Position;

            // узнать локальную позицию нанесения урона.
            // То есть, gridHitW и gridHitH - это глобальная позиция на игровой сетке.
            // А нам нужно узнать теперь позицию относительно юнита
            int localW = gridHitW - entityPositionOnGrid.x;
            int localH = gridHitH - entityPositionOnGrid.y;

            Enums.PartBody hitPartBody = GetPartBody(unit, localW, localH);
            float multiplication;

            switch (hitPartBody)
            {
                case Enums.PartBody.head:
                case Enums.PartBody.body:
                case Enums.PartBody.bottom:
                    {
                        multiplication = _unitDamageMultiplicationService.Get(hitPartBody);
                    }
                    break;

                default:
                case Enums.PartBody.empty:
                    multiplication = 0;
                    break;
            }

            damageByBody = (int)(damage * multiplication);

            return damageByBody;
        }

        private Enums.PartBody GetPartBody(IUnit unit, int localW, int localH)
        {
            foreach (PartBodyScheme area in unit.AreaGrid)
            {
                if (area.wIndex == localW && area.hIndex == localH)
                {
                    return area.partBody;
                }
            }

            LogChannel.Log("BodyDamageConverterService :: Converter().GetPartBody() wrong hit position. Fix me!");

            return Enums.PartBody.empty;
        }
    }
}

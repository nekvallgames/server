using Plugin.Interfaces;
using Plugin.Interfaces.UnitComponents;

namespace Plugin.Runtime.Services.AI.Inputs
{
    /// <summary>
    /// Інпут, котрий відповідає за дані, чи являється поточний юніт VIP-ом?
    /// Якщо юніт VIP, то результат Size == 1
    /// Якщо юніт не VIP, то результат Size == 0
    /// </summary>
    public class IsVipInput : BaseInput
    {
        private IUnit _unit;

        public IsVipInput(IUnit unit)
        {
            _unit = unit;
        }

        public override float Size {
            get {
                if (!(_unit is IVipComponent))
                    return 0f;

                return (_unit as IVipComponent).IsVip ? 1f : 0f;
            }
        }
    }
}

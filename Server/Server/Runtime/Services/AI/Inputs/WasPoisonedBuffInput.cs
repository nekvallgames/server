using Plugin.Interfaces;

namespace Plugin.Runtime.Services.AI.Inputs
{
    /// <summary>
    /// Інпут, котрий відповідає за дані, чи являється поточний юніт отравлений ядом?
    /// Якщо юніт отравлений ядом, то результат Size == 1
    /// Якщо юніт не отравлений ядом, то результат Size == 0
    /// </summary>
    public class WasPoisonedBuffInput : BaseInput
    {
        private IUnit _unit;

        public WasPoisonedBuffInput(IUnit unit)
        {
            _unit = unit;
        }

        public override float Size
        {
            get
            {
                if (!(_unit is IPoisonBuffComponent)){
                    return 0f;
                }

                return (_unit as IPoisonBuffComponent).PoisonBuff > 0 ? 0f : 1f;
            }
        }
    }
}

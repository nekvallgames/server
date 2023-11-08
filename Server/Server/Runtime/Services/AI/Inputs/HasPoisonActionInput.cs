using Plugin.Interfaces;

namespace Plugin.Runtime.Services.AI.Inputs
{
    /// <summary>
    /// Інпут, котрий відповідає за дані, чи має поточний юніт PoisonActionComponent?
    /// Наприклад, юніт UnitPoison, котрий отравляє ворогів ядовитими стрілами
    /// Якщо має поточний юніт PoisonActionComponent, то результат Size == 1
    /// Якщо не має поточний юніт PoisonActionComponent, то результат Size == 0
    /// </summary>
    public class HasPoisonActionInput : BaseInput
    {
        private IUnit _unit;

        public HasPoisonActionInput(IUnit unit)
        {
            _unit = unit;
        }

        public override float Size => (_unit is IPoisonActionComponent) ? 1f : 0f;
    }
}

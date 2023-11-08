using Plugin.Interfaces;

namespace Plugin.Runtime.Services.AI.Inputs
{
    /// <summary>
    /// Інпут, котрий відповідає за дані, чи являється поточний юніт гуманоїдом?
    /// Якщо юніт гуманоїд, то результат Size == 1
    /// Якщо юніт не гуманоїд, то результат Size == 0
    /// </summary>
    public class IsHumanoidInput : BaseInput
    {
        private IUnit _unit;

        public IsHumanoidInput(IUnit unit)
        {
            _unit = unit;
        }

        public override float Size => _unit.IsHumanoid ? 1f : 0f;
    }
}

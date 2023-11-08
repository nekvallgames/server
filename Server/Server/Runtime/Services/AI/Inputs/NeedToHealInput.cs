using Plugin.Interfaces;
using Plugin.Interfaces.UnitComponents;

namespace Plugin.Runtime.Services.AI.Inputs
{
    /// <summary>
    /// Інпут, котрий відповідає за дані, чи потрібно вилікувати поточного юніта?
    /// Наприклад, якщо юніт має НЕ максимальну кількість здоров'я, то поточного юніта можно вилікувати
    /// Якщо юніт має не максимальну кількість здоров'я, то результат Size == 1
    /// Якщо юніт має максимальну кількість здоров'я, то результат Size == 0
    /// </summary>
    public class NeedToHealInput : BaseInput
    {
        private UnitsService _unitService;
        private IUnit _unit;

        public NeedToHealInput(IUnit unit, UnitsService unitService)
        {
            _unit = unit;
            _unitService = unitService;
        }

        public override float Size
        {
            get
            {
                if(!(_unit is IHealthComponent))
                    return 0f;

                int healthWithoutBuff = _unitService.GetHealthWithoutBuff(_unit);
                int maxHealth = (_unit as IHealthComponent).HealthCapacityMax;
                return (healthWithoutBuff == maxHealth) ? 0f : 1f;
            }
        }
    }
}

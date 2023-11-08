using Plugin.Interfaces;
using Plugin.Interfaces.UnitComponents;

namespace Plugin.Runtime.Services.AI.Inputs
{
    /// <summary>
    /// Інпут, котрий відповідає за дані, чи являється поточний юніт пораненим?
    /// Якщо юніт поранений, то результат Size == 1
    /// Якщо юніт не поранений, то результат Size == 0
    /// </summary>
    public class WasDamagedInput : BaseInput
    {
        private UnitsService _unitService;
        private IUnit _unit;

        public WasDamagedInput(IUnit unit, UnitsService unitService)
        {
            _unit = unit;
            _unitService = unitService;
        }

        public override float Size
        {
            get
            {
                if (!(_unit is IHealthComponent)){
                    return 0f;
                }

                var maxHealth = (_unit as IHealthComponent).HealthCapacityMax;
                int originalHealth = _unitService.GetHealthWithoutBuff(_unit);

                return originalHealth < maxHealth ? 1f : 0f;
            }
        }

    }
}

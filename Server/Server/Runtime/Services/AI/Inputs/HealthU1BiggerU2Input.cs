using Plugin.Interfaces;
using System;

namespace Plugin.Runtime.Services.AI.Inputs
{
    /// <summary>
    /// Інпут, котрий порівняє рівень життів вказаних юнітів
    /// Якщо юніт _u1Id буде мати однакову кількість здоров'я із юнітом _u2Id, то результат Size == 0
    /// Якщо юніт 1 буде мати більше здоров'я, чим у юніта 2, то результат Size буде > 0
    /// Якщо юніт 1 буде мати менше здоров'я, чим у юніта 2, то результат Size буде == 0
    /// </summary>
    public class HealthU1BiggerU2Input : BaseInput
    {
        private UnitsService _unitService;

        private IUnit _u1Id;
        private IUnit _u2Id;

        public HealthU1BiggerU2Input(IUnit u1Id, IUnit u2Id, UnitsService unitsService)
        {
            _u1Id = u1Id;
            _u2Id = u2Id;

            _unitService = unitsService;
        }

        public override float Size
        {
            get
            {
                float u1Value = Math.Max(1, GetHealth(_u1Id));
                float u2Value = Math.Max(1, GetHealth(_u2Id));

                return CompareSize(u1Value, u2Value);
            }
        }

        private float GetHealth(IUnit unit)
        {
            return _unitService.GetHealthWithoutBuff(unit);
        }
    }
}

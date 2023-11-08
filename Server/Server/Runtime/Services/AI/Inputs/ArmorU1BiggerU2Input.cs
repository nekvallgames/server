using Plugin.Interfaces;
using Plugin.Interfaces.UnitComponents;
using System;

namespace Plugin.Runtime.Services.AI.Inputs
{
    /// <summary>
    /// Інпут, котрий порівняє рівень броні вказаних юнітів
    /// Якщо юніт _u1Id буде мати однакову кількість броні із юнітом _u2Id, то результат Size == 0
    /// Якщо юніт 1 буде мати більше броні, чим у юніта 2, то результат Size буде > 0
    /// Якщо юніт 1 буде мати менше броні, чим у юніта 2, то результат Size буде == 0
    /// </summary>
    public class ArmorU1BiggerU2Input : BaseInput
    {
        private IUnit _u1Id;
        private IUnit _u2Id;

        public ArmorU1BiggerU2Input(IUnit u1Id, IUnit u2Id)
        {
            _u1Id = u1Id;
            _u2Id = u2Id;
        }

        public override float Size
        {
            get
            {
                float u1Value = Math.Max(1, GetArmor(_u1Id));
                float u2Value = Math.Max(1, GetArmor(_u2Id));

                return CompareSize(u1Value, u2Value);
            }
        }

        private float GetArmor(IUnit unit)
        {
            if (!(unit is IArmorComponent))
            {
                return 0;
            }

            return (unit as IArmorComponent).ArmorCapacity;
        }
    }
}

using Plugin.Interfaces;
using System;

namespace Plugin.Runtime.Services.AI.Inputs
{
    /// <summary>
    /// Інпут, котрий порівняє розмір тіла вказаних юнітів
    /// Якщо юніт _u1Id буде мати однаковий розмір із юнітом _u2Id, то результат Size == 0
    /// Якщо юніт _u1Id буде більше юніта _u2Id, то результат Size буде > 0
    /// Якщо юніт _u1Id буде менше юніта _u2Id, то результат Size буде == 0
    /// </summary>
    public class SizeBodyU1LowerU2Input : BaseInput
    {
        private IUnit _u1Id;
        private IUnit _u2Id;

        public SizeBodyU1LowerU2Input(IUnit u1Id, IUnit u2Id)
        {
            _u1Id = u1Id;
            _u2Id = u2Id;
        }
        public override float Size
        {
            get
            {
                float u1Value = Math.Max(1, GetSizeBody(_u2Id));
                float u2Value = Math.Max(1, GetSizeBody(_u1Id));

                return CompareSize(u1Value, u2Value);
            }
        }

        private float GetSizeBody(IUnit unit)
        {
            return unit.BodySize.x * unit.BodySize.y;
        }
    }
}

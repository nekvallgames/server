using Plugin.Interfaces;

namespace Plugin.Runtime.Services.AI.Inputs
{
    public class SizeBodyInput : BaseInput
    {
        private IUnit _unit;

        public SizeBodyInput(IUnit unit)
        {
            _unit = unit;
        }

        public override float Size
        {
            get { return GetSizeBody(_unit); }
        }

        private float GetSizeBody(IUnit unit)
        {
            return unit.BodySize.x * unit.BodySize.y;
        }
    }
}

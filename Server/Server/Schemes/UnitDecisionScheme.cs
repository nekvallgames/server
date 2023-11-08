using Plugin.Interfaces;
using System.Collections.Generic;

namespace Plugin.Schemes
{
    public class UnitDecisionScheme : IUnitDecision
    {
        public IOutput Output { get; set; }
        public List<IUnit> Units { get; set; }
    }
}

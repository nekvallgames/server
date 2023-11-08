using System.Collections.Generic;

namespace Plugin.Interfaces
{
    public interface IUnitDecision : IDecision
    {
        List<IUnit> Units { get; set; }
    }
}

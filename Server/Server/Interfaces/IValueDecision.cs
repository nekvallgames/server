using System.Collections.Generic;

namespace Plugin.Interfaces
{
    public interface IValueDecision : IDecision
    {
        List<int> Values { get; set; }
    }
}

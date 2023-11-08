using Plugin.Interfaces;
using System.Collections.Generic;

namespace Plugin.Schemes
{
    public class ValueDecisionScheme : IValueDecision
    {
        public IOutput Output { get; set; }
        public List<int> Values { get; set; }
    }
}

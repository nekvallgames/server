using Plugin.Interfaces;
using Plugin.Runtime.Services.AI.Outputs;
using System.Collections.Generic;
using System.Linq;

namespace Plugin.Runtime.Services.AI
{
    public class BaseDecision
    {
        protected IOutput GetDecision(List<IOutput> outputs)
        {
            if (outputs.Any(x => x.IsActive))
            {
                var activeList = outputs.FindAll(x => x.IsActive);
                if (activeList.Count == 1)
                    return activeList[0];

                activeList.Sort((x, y) => x.Sum.CompareTo(y.Sum));
                activeList.Reverse();
                return activeList[0];
            }

            return new EmptyOutput();    // ніодне рішення не було прийнято, або всі рішення мають однакову суму прийняття рішення
        }
    }
}

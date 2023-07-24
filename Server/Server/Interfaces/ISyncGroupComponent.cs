using System.Collections.Generic;

namespace Plugin.Interfaces
{
    public interface ISyncGroupComponent
    {
        List<ISyncComponent> SyncElements { get; }
    }
}

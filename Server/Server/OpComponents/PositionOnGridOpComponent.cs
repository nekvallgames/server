using Newtonsoft.Json;
using Plugin.Interfaces;
using System;

namespace Plugin.OpComponents
{
    [Serializable]
    public struct PositionOnGridOpComponent : ISyncComponent
    {
        // syncStep
        public int ss;
        // groupIndex
        public int gi;
        // positionW
        public int w;
        // positionH
        public int h;

        [JsonIgnore]
        public int SyncStep
        {
            get { return ss; }
            set { ss = value; }
        }
        [JsonIgnore]
        public int GroupIndex
        {
            get { return gi; }
            set { gi = value; }
        }
    }
}

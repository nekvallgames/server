using Newtonsoft.Json;
using Plugin.Interfaces;
using System;

namespace Plugin.OpComponents
{
    /// <summary>
    /// Синхронізувати додаткову дію юніта по вказаній позиції
    /// Тобто примінити додаткову дію по вказаній позиції на ігровій сітці
    /// </summary>
    [Serializable]
    public struct AdditionalByPosOpComponent : ISyncComponent
    {
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


        // syncStep
        public int ss;
        // groupIndex
        public int gi;
        /// <summary>
        /// Target actor Id
        /// </summary>
        public int tai;
        // positionW
        public int w;
        // positionH
        public int h;
    }
}

using Newtonsoft.Json;
using Plugin.Interfaces;
using System;

namespace Plugin.OpComponents
{
    /// <summary>
    /// Синхронізувати додаткову дію юніта для вказаного юніта
    /// Тобто примінити додаткову дію по вказаному юніту
    /// </summary>
    [Serializable]
    public struct AdditionalByUnitOpComponent : ISyncComponent
    {
        // syncStep
        public int ss;
        // groupIndex
        public int gi;
        /// <summary>
        /// Target actor id
        /// </summary>
        public int tai;
        // unit Id
        public int uid;
        // instance Id
        public int iid;

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

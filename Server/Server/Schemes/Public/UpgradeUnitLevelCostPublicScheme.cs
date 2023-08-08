using Newtonsoft.Json;
using System;

namespace Plugin.Schemes
{
    [Serializable]
    public struct UpgradeUnitLevelCostPublicScheme
    {
        [JsonProperty("cost")]
        public int[] Cost;

        [JsonProperty("capacities")]
        public CapacitiesScheme[] Capacities;
    }

    [Serializable]
    public struct CapacitiesScheme
    {
        public int[] ids;
        public int[] capacity;
    }
}

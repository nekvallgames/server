using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Plugin.Schemes.Public
{
    [Serializable]
    public class UnitsPublicScheme : List<UnitPublicScheme>
    {

    }

    [Serializable]
    public struct UnitPublicScheme
    {
        public int id;
        public string name;
        public string description;
        public int health;
        public int damage;
        public int capacity;
        public int armor;
        [JsonProperty("additional_power")]
        public int additionalPower;
        [JsonProperty("additional_capacity")]
        public int additionalCapacity;
        [JsonProperty("additional_damage")]
        public int additionalDamage;
    }
}

using Newtonsoft.Json;
using System;

namespace Plugin.Schemes
{
    [Serializable]
    public struct AbortScheme
    {
        /// <summary>
        /// Поточна кількість рейтингу гравця
        /// </summary>
        [JsonProperty("r")]
        public int rating;
    }
}

using Newtonsoft.Json;

namespace Plugin.Schemes
{
    public struct AbortScheme
    {
        /// <summary>
        /// Поточна кількість рейтингу гравця
        /// </summary>
        [JsonProperty("r")]
        public int rating;
    }
}

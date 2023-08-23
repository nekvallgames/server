using Newtonsoft.Json;
using System;

namespace Plugin.Schemes
{
    [Serializable]
    public struct StepResultScheme
    {
        /// <summary>
        /// Ігровий мод
        /// </summary>
        [JsonProperty("gm")]
        public int gameMode;

        /// <summary>
        /// Вказати, поточний гравець виграв чи ні?
        /// </summary>
        [JsonProperty("w")]
        public bool isWin;

        /// <summary>
        /// Поточна кількість рейтингу гравця
        /// </summary>
        [JsonProperty("r")]
        public int rating;

        /// <summary>
        /// Кроки, котрі були виконані гравцями
        /// </summary>
        [JsonProperty("s")]
        public StepScheme stepScheme;
    }
}

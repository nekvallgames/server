using Newtonsoft.Json;
using System;

namespace Plugin.Schemes.Public
{
    [Serializable]
    public struct PlotPublicScheme
    {
        [JsonProperty("scenario_step_first_move_time")]
        public int FirstMoveTime;
        [JsonProperty("scenario_step_first_attack_time")]
        public int FirstAttackTime;
        [JsonProperty("vip_health_buff")]
        public int VipHealthBuff;
        [JsonProperty("increase_rating")]
        public int IncreaseRating;
        [JsonProperty("decrease_rating")]
        public int DecreaseRating;
    }
}

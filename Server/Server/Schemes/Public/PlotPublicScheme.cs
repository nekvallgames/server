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
        // [JsonProperty("increase_damage_every_step")]
        // public int IncreaseDamageEveryStep;
        [JsonProperty("increase_damage_for_last_unit")]
        public int IncreaseDamageForLastUnit;
    }
}

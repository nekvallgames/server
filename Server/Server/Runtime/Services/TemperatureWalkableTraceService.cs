namespace Plugin.Runtime.Services
{
    /// <summary>
    /// Сервіс, котрий буде відслідковувати температурний слід юнітів,
    /// котрі переміщаються по карті
    /// Чим довше юніт стоїть на одному місці, тим гарячіше буде його слід
    /// </summary>
    public class TemperatureWalkableTraceService
    {
        private UnitsService _unitService;
        private HitAreaService _hitAreaService;

        /// <summary>
        /// При кожному оновленні температурного сліду на скільки слід буде нагріватись
        /// </summary>
        private const float INCREASE_TEMPERATURE = 1f;
        /// <summary>
        /// При кожному оновленні температурного сліду на скільки слід буде охолоджуватись
        /// </summary>
        private const float DECREASE_TEMPERATURE = 0.4f;


    }
}

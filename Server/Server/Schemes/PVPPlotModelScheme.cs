using Plugin.Interfaces;

namespace Plugin.Schemes
{
    /// <summary>
    /// Схема, котра буде зберігати в данні ігрового сценарія PVP
    /// Поточна схема буде лежати в моделі PlotsPrivateModel
    /// </summary>
    public class PVPPlotModelScheme : IPlotModelScheme
    {
        /// <summary>
        /// Вказати id ігрової кімнати, в котрій знаходиться актер
        /// </summary>
        public string GameId { get; }

        /// <summary>
        /// Поточний крок синхронізації ігрового сценарія
        /// </summary>
        public int SyncStep { get; set; }


        public PVPPlotModelScheme(string gameId)
        {
            GameId = gameId;
        }
    }
}

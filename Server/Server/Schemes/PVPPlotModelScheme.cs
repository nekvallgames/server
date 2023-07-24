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
        /// Вказати id ігрової кімнати
        /// </summary>
        public string GameId { get; }

        /// <summary>
        /// Список із акторів, кому належить поточна модель
        /// </summary>
        public int OwnerActorId { get; }

        /// <summary>
        /// Поточний крок синхронізації ігрового сценарія
        /// </summary>
        public int SyncStep { get; set; }


        public PVPPlotModelScheme(string gameId, int ownerActorId)
        {
            GameId = gameId;
            OwnerActorId = ownerActorId;
        }
    }
}

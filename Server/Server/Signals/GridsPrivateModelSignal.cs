namespace Plugin.Signals
{
    /// <summary>
    /// Подія, коли модель із даними ігрових сіток була змінена
    /// </summary>
    public class GridsPrivateModelSignal : ModelChangeSignal
    {
        public string GameId { get; }
        public int OwnerActorId { get; }

        public GridsPrivateModelSignal(string gameId, int ownerActorId, StatusType status) : base(status)
        {
            GameId = gameId;
            OwnerActorId = ownerActorId;
        }
    }
}

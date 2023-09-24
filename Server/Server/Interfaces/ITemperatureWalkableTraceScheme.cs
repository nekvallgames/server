namespace Plugin.Interfaces
{
    public interface ITemperatureWalkableTraceScheme
    {
        /// <summary>
        /// Id ігрової кімнати, в котрій знаходяться гравці
        /// </summary>
        string GameId { get; }
        /// <summary>
        /// Id гравця в ігрової кімнаті
        /// </summary>
        int ActorNr { get; }
    }
}

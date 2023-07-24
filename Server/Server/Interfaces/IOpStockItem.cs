namespace Plugin.Interfaces
{
    public interface IOpStockItem
    {
        /// <summary>
        /// Вказати id ігрової кімнати
        /// </summary>
        string GameId { get; }

        /// <summary>
        /// Актор, хто прислав поточну операцію на Game Server
        /// </summary>
        int ActorId { get; }

        /// <summary>
        /// Код операции
        /// </summary>
        byte OpCode { get; }

        /// <summary>
        /// Данные операции
        /// </summary>
        object Data { get; }
    }
}

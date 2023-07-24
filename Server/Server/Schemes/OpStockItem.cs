using Plugin.Interfaces;

namespace Plugin.Schemes
{
    public struct OpStockItem : IOpStockItem
    {
        /// <summary>
        /// Вказати id ігрової кімнати
        /// </summary>
        public string GameId { get; }

        public int ActorId { get; }

        /// <summary>
        /// Код операции
        /// </summary>
        public byte OpCode { get; }

        /// <summary>
        /// Данные операции
        /// </summary>
        public object Data { get; }

        public OpStockItem(string gameId, int actorId, byte evCode, object data )
        {
            GameId = gameId;
            ActorId = actorId;
            OpCode = evCode;
            Data = data;
        }
    }
}

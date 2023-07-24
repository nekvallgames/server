namespace Plugin.Signals
{
    /// <summary>
    /// Подія, коли модель із даними операцій акторів була змінена
    /// </summary>
    public class OpStockPrivateModelSignal : ModelChangeSignal
    {
        /// <summary>
        /// Вказати id кімнати, актор котрої прислав операцію
        /// </summary>
        public string GameId { get; }
        /// <summary>
        /// Власник операції
        /// </summary>
        public int ActorId { get; }
        /// <summary>
        /// Код операції знаходиться в классі OperationCode
        /// </summary>
        public byte OpCode { get; }

        public OpStockPrivateModelSignal(string gameId, int actorId, byte opCode, StatusType status):base(status)
        {
            GameId = gameId;
            ActorId = actorId;
            OpCode = opCode;
        }
    }
}

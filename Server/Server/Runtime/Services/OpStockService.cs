using Plugin.Interfaces;
using Plugin.Models.Private;
using System.Linq;

namespace Plugin.Runtime.Services
{
    /// <summary>
    /// Сервіс, для маніпуляції операцій, котрі присилають актори
    /// </summary>
    public class OpStockService
    {
        private OpStockPrivateModel _model;

        public OpStockService(OpStockPrivateModel model)
        {
            _model = model;
        }

        public void Add(IOpStockItem opScheme)
        {
            _model.Add(opScheme);
        }

        /// <summary>
        /// Чи є на складі актора вказана операція? 
        /// </summary>
        public bool HasOp(string gameId, int actorId, byte opCode)
        {
            return _model.Items.Any(x => x.GameId == gameId && x.ActorId == actorId && x.OpCode == opCode);
        }

        /// <summary>
        /// Получить общее количество указаной операции на складе всех игроков
        /// </summary>
        public int GetOpCount(string gameId, byte operationCode)
        {
            return _model.Items.FindAll(x => x.GameId == gameId && x.OpCode == operationCode).Count;
        }

        /// <summary>
        /// Отримати операцію зі складу 
        /// </summary>
        public IOpStockItem GetOp(string gameId, int actorId, byte opCode)
        {
            return _model.Items.Find(x => x.GameId == gameId && x.ActorId == actorId && x.OpCode == opCode);
        }

        /// <summary>
        /// Отримати і видалити операцію зі складу 
        /// </summary>
        public IOpStockItem TakeOp(string gameId, int actorId, byte opCode)
        {
            var item = GetOp(gameId, actorId, opCode);
            _model.Remove(item);

            return item;
        }
    }
}

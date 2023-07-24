using System.Collections.Generic;

namespace Plugin.Interfaces
{
    public interface IExecuteOpGroup
    {
        /// <summary>
        /// Может ли текущий класс выполнить действие игрока?
        /// </summary>
        bool CanExecute(List<ISyncComponent> componentsGroup);

        /// <summary>
        /// Выполнить действие игрока
        /// </summary>
        void Execute(string gameId, int playerActorId, List<ISyncComponent> componentsGroup);
    }
}

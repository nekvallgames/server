namespace Plugin.Interfaces
{
    public interface IExecuteAction
    {
        /// <summary>
        /// Может ли текущий класс выполнить действие для юнита?
        /// </summary>
        bool CanExecute(IUnit unit);

        /// <summary>
        /// Выполнить действие
        /// </summary>
        void Execute(IUnit unit, string gameId, int targetActorId, int posW, int posH);
    }
}

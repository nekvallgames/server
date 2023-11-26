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
        void ExecuteByPos(IUnit unit, string gameId, int targetActorId, int targetPosW, int targetPosH);
        void ExecuteByUnit(IUnit unit, string gameId, int targetActorId, int targetUnitId, int targetInstanceId);
    }
}

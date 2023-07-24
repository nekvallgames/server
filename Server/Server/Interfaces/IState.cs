namespace Plugin.Interfaces
{
    /// <summary>
    /// Интерфейс для всех состояний работы стейт машины
    /// </summary>
    public interface IState
    {
        /// <summary>
        /// Вказати ім'я стейту
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Когда StateMachine изменяет состояние, то у нового состояния выполняется 
        /// текущий метот на старте
        /// </summary>
        void EnterState();

        /// <summary>
        /// Выполнится в момент перед выходом из текущего состояния
        /// </summary>
        void ExitState();
    }
}

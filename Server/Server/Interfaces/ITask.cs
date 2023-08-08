using System;

namespace Plugin.Interfaces
{
    public interface ITask : IName
    {
        /// <summary>
        /// Выполнится 1 раз при входе в состояние
        /// success - callback виконається, коли задача стейту буде виконана успішно
        /// fail - callback виконається, коли задача стейту буде виконана НЕ успішно
        /// </summary>
        void EnterTask(Action taskIsDone, Action taskIsFail);

        /// <summary>
        /// Выполнится 1 раз при выходе с сотояния
        /// </summary>
        void ExitTask();
    }
}

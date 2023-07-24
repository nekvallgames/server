using Plugin.Interfaces;

namespace Plugin.Signals
{
    public abstract class ModelChangeSignal : ISignal
    {
        /// <summary>
        /// Статус змінення моделі
        /// add - була добавлена нова операція
        /// remove - операція була оброблена і видалена
        /// </summary>
        public StatusType Status { get; }

        public enum StatusType
        {
            add,
            remove,
            change
        }

        public ModelChangeSignal(StatusType status)
        {
            Status = status;
        }
    }
}

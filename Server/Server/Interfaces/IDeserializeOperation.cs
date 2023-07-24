using Plugin.Schemes;
using System;

namespace Plugin.Interfaces
{
    /// <summary>
    /// Интерфейс, для всех десериализаторов
    /// </summary>
    public interface IDeserializeOperation
    {
        /// <summary>
        /// Может ли текущий десериализатор десериализировать операцию
        /// </summary>
        bool CanDeserialize(OpStockItem opData);

        Type TypeDeserialize { get; }

        /// <summary>
        /// Десериализировать операцию
        /// </summary>
        T Deserialize<T>(OpStockItem opData);
    }
}

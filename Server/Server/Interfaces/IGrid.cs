using Plugin.Schemes;

namespace Plugin.Interfaces
{
    public interface IGrid
    {
        /// <summary>
        /// Список с селлами из которых создана игровая сетка
        /// </summary>
        Cell[] СellsList { get; }

        /// <summary>
        /// Вказати id ігрової кімнати
        /// </summary>
        string GameId { get; }

        /// <summary>
        /// Владелец игровой сетки
        /// </summary>
        int OwnerActorNr { get; }

        /// <summary>
        /// Размер игровой сетки по ширине
        /// </summary>
        int SizeGridW { get; }

        /// <summary>
        /// Размер игровой сетки по высоте
        /// </summary>
        int SizeGridH { get; }
    }
}

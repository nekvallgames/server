using Plugin.Interfaces;

namespace Plugin.Schemes
{
    public struct GridScheme : IGrid
    {
        /// <summary>
        /// Список с селлами из которых создана игровая сетка
        /// </summary>
        public Cell[] СellsList { get; }

        /// <summary>
        /// Вказати id ігрової кімнати
        /// </summary>
        public string GameId { get; }

        /// <summary>
        /// Владелец игровой сетки
        /// </summary>
        public int OwnerActorNr { get; }

        /// <summary>
        /// Размер игровой сетки по ширине
        /// </summary>
        public int SizeGridW { get; }

        /// <summary>
        /// Размер игровой сетки по высоте
        /// </summary>
        public int SizeGridH { get; }


        public GridScheme(string gameId, int ownerActorId, int sizeGridW, int sizeGridH, Cell[] cellsList)
        {
            GameId = gameId;
            OwnerActorNr = ownerActorId;
            SizeGridW = sizeGridW;
            SizeGridH = sizeGridH;
            СellsList = cellsList;
        }
    }
}

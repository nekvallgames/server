using System.Collections.Generic;

namespace Plugin.Schemes
{
    public class CellWalkablePrivateScheme
    {
        public string GameId { get; }

        public int OwnerActorNr { get; }

        /// <summary>
        /// Сели, по котрим юніт може переміщатися.
        /// Це сели на яких юніт може стояти, і сели,
        /// по яким юніт може переміщатися, але встати не може
        /// Іншими словами, це всі сели, які знаходяться під маскою переміщення юніта
        /// </summary>
        public List<Cell> CellsUnderPath = new List<Cell>();

        /// <summary>
        /// Список из селлов, на которых перетаскиваемый юнит может встать
        /// То есть, это просто селлы, на которых теоретически юнит может встать,
        /// Но! юнит может не встать по ширине
        /// </summary>
        public List<Cell> CellsSettle = new List<Cell>();

        /// <summary>
        /// Список из селлов, на которых перетаскиваемый юнит может встать всей своей шириной
        /// То есть, мы можем поставить юнита на сюбой селл из массива _settleAreaCells без дополнительных проверок
        /// </summary>
        public List<Cell> CellsSettleArea = new List<Cell>();

        /// <summary>
        /// Список із координатами, котрі потрібно добавити в ігнор
        /// Тобто сели із поточного списка типу як заняті, на них не можна встати
        /// </summary>
        public List<(int, int)> IgnoreList = new List<(int, int)>();

        public CellWalkablePrivateScheme(string gameId, int ownerActorNr)
        {
            GameId = gameId;
            OwnerActorNr = ownerActorNr;
        }
    }
}

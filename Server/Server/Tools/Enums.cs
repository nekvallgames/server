namespace Plugin.Tools
{
    public struct Enums
    {
        /// <summary>
        /// Двуичная маска для ячеек
        /// </summary>
        public enum CellMask
        {
            wall = 000001,          // стена                      1
            wallLock = 000010,      // стела блок                 2
            floor = 000100,         // пол                        4
            floorLock = 001000,     // пол блок                   8
            border = 010000,        // края игровой сетки         16
        }

        /// <summary>
        /// Обозначение соседних ячеек игровой сетки
        /// </summary>
        public enum Direction
        {
            left,
            right,
            up,
            down
        }

        /// <summary>
        /// Часть тела
        /// </summary>
        public enum PartBody
        {
            empty = 0,    // пусто. Текущая часть тела не должна получать урон. Выстрел в текущею часть тела должен считатся промахом
            head = 1,    // голова
            body = 2,    // тело
            bottom = 3,    // ноги
        }
    }
}

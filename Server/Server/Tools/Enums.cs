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
    }
}

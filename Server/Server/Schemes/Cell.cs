using Plugin.Tools;

namespace Plugin.Schemes
{
    public class Cell
    {
        /// <summary>
        /// Индекс ячейки по ширине
        /// </summary>
        public uint wIndex;

        /// <summary>
        /// Индекс ячейки по высоте
        /// </summary>
        public uint hIndex;

        /// <summary>
        /// Двоичная маска для ячейки
        /// </summary>
        public Enums.CellMask binaryMask;

        /// <summary>
        /// Сосед сверху от текущей ячейки
        /// </summary>
        public Cell up;

        /// <summary>
        /// Сосед снизу от текущей ячейки
        /// </summary>
        public Cell down;

        /// <summary>
        /// Сосед слева от текущей ячейки
        /// </summary>
        public Cell left;

        /// <summary>
        /// Сосед справа от текущей ячейки
        /// </summary>
        public Cell right;

        public void Initialize(uint wIndex,
                               uint hIndex,
                               Enums.CellMask binaryMask,
                               Cell up,
                               Cell down,
                               Cell left,
                               Cell right)
        {
            this.wIndex = wIndex;
            this.hIndex = hIndex;
            this.binaryMask = binaryMask;
            this.up = up;
            this.down = down;
            this.left = left;
            this.right = right;
        }
    }
}

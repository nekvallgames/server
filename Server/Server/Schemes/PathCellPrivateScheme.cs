namespace Plugin.Schemes
{
    public class PathCellPrivateScheme
    {
        public static PathCellPrivateScheme Null
        {
            get
            {
                PathCellPrivateScheme cell = new PathCellPrivateScheme();
                cell.IsNull = true;

                return cell;
            }
        }

        public bool IsNull;

        /// <summary>
        /// Позиция на игровой сетке по ширине
        /// </summary>
        public int positionW;

        /// <summary>
        /// Позиция на игровой сетке по высоте
        /// </summary>
        public int positionH;

        /// <summary>
        /// Индекс пути текущей ячейки
        /// </summary>
        public int pathIndex = -1;

        /// <summary>
        /// Текущая ячейка уже проверялась?
        /// </summary>
        public bool IsDirty = false;
    }
}

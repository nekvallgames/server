using Plugin.Interfaces;
using Plugin.Schemes;
using Plugin.Tools;

namespace Plugin.Builders
{
    /// <summary>
    /// Білдер, котрий створить ігрову сітку для гравця
    /// </summary>
    public class GridBuilder
    {
        public IGrid Create(string gameId, int ownerActorId, Int2 sizeGrid, int[] gridMask)
        {
            int countCells = sizeGrid.x * sizeGrid.y;
            var cells = new Cell[countCells];

            // Создаем все селлы
            for (int i = 0; i < cells.Length; i++){
                cells[i] = new Cell();
            }

            // 2. Позиционируем и указываем соседей для каждой из ячеек
            uint h = 0;
            uint w = 0;

            Cell cell;

            for (int i = 0; i < cells.Length; i++)
            {
                if (i > 0 && (i % sizeGrid.x) == 0)
                {
                    w = 0; // ширина
                    h++;   // высота   
                }

                cell = cells[i];
                cell.Initialize(
                                w,
                                h,
                                GetBinaryMask(w, h, sizeGrid.x, sizeGrid.y, gridMask),
                                GetCellNearMe(i, Enums.Direction.up, ref cells, sizeGrid.x, sizeGrid.y),
                                GetCellNearMe(i, Enums.Direction.down, ref cells, sizeGrid.x, sizeGrid.y),
                                GetCellNearMe(i, Enums.Direction.left, ref cells, sizeGrid.x, sizeGrid.y),
                                GetCellNearMe(i, Enums.Direction.right, ref cells, sizeGrid.x, sizeGrid.y)
                );

                w++;
            }

            return new GridScheme(gameId, ownerActorId, sizeGrid.x, sizeGrid.y, cells);
        }

        /// <summary>
        /// Указав позицию ячейки выташить из gridMask ее двуичную маску
        /// Это что то типа парсера
        /// </summary>
        private Enums.CellMask GetBinaryMask(uint w, uint h, int gridWidth, int gridHeight, int[] gridMask)
        {
            // Конвертировать индекс, который вытащим из gridMask
            int startIndex = (int)h * gridWidth;
            startIndex += (int)w;

            return ConvertIndexIntoBinaryMask(gridMask[startIndex]);
        }

        /// <summary> 
        /// Конвертировать индекс переданой маски в бинарную маску
        /// </summary>
        private Enums.CellMask ConvertIndexIntoBinaryMask(int maskIndex)
        {
            switch (maskIndex)
            {
                default:
                case 1: return Enums.CellMask.wall;
                case 2: return Enums.CellMask.wallLock;
                case 4: return Enums.CellMask.floor;
                case 8: return Enums.CellMask.floorLock;
                case 16: return Enums.CellMask.border;
            }
        }

        /// <summary>
        /// Получить ячейку возле меня
        /// myIndex      - индекс селла в игровой сетке
        /// whatFindCell - указать направление, какого соседа искать, тот что left, right, up, down
        /// nextNear     - 1 следующая ячейка за рядом стоящей текущей ячейки, аналогично cellsList[myIndex].near.near
        ///                2 следующая следующая ячейка за рядом стоящей текущей ячейки, аналогично cellsList[myIndex].near.near.near
        /// </summary>
        private Cell GetCellNearMe(int myIndex, 
                                   Enums.Direction whatFindCell, 
                                   ref Cell[] cells, 
                                   int sizeGridW, 
                                   int sizeGridH, 
                                   int nextNear = 0)
        {
            Cell eCell = cells[myIndex];

            int findIndex = -1;
            int step = 0;

            while (step <= nextNear)
            {
                switch (whatFindCell)
                {
                    case Enums.Direction.up:
                        {
                            findIndex = myIndex + sizeGridW;
                        }
                        break;

                    case Enums.Direction.down:
                        {
                            findIndex = myIndex - sizeGridW;
                        }
                        break;

                    case Enums.Direction.left:
                        {

                            if (eCell.wIndex > 0)
                                findIndex = myIndex - 1;
                            else
                                return null;
                        }
                        break;

                    case Enums.Direction.right:
                        {

                            if (eCell.wIndex < sizeGridW - 1)
                                findIndex = myIndex + 1;
                            else
                                return null;
                        }
                        break;
                }


                if (findIndex >= 0 && findIndex < cells.Length)
                {
                    myIndex = findIndex;
                    eCell = cells[findIndex];
                }
                else
                {
                    return null;
                }

                step++;
            }

            return eCell;
        }
    }
}

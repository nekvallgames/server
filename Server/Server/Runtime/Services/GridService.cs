using Plugin.Builders;
using Plugin.Interfaces;
using Plugin.Models.Private;
using Plugin.Models.Public;
using Plugin.Runtime.Providers;
using Plugin.Schemes;
using Plugin.Tools;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Plugin.Runtime.Services
{
    /// <summary>
    /// Сервіс, котрий реалізує логіку ігрової сітки для акторів
    /// </summary>
    public class GridService
    {
        private LocationsPublicModel<LocationScheme> _locationsPublicModel;
        private GridsPrivateModel _gridsPrivateModel;
        private GridBuilder _gridBuilder;

        public GridService(PublicModelProvider publicModelProvider, 
                           PrivateModelProvider privateModelProvider, 
                           GridBuilder gridBuilder)
        {
            _locationsPublicModel = publicModelProvider.Get<LocationsPublicModel<LocationScheme>>();
            _gridsPrivateModel = privateModelProvider.Get<GridsPrivateModel>();
            _gridBuilder = gridBuilder;
        }

        public void Create(string gameId, int actorNr)
        {
            LocationScheme scheme = _locationsPublicModel.Items[0]; // TODO поки що постійно створюємо локацію за замовчуванням

            _gridsPrivateModel.Add(_gridBuilder.Create(gameId, actorNr, scheme.SizeGrid, scheme.GridMask));
        }

        public bool IsExist(string gameId, int actorNr)
        {
            return _gridsPrivateModel.Items.Any(x => x.GameId == gameId && x.OwnerActorNr == actorNr);
        }

        public IGrid Get(string gameId, int actorNr)
        {
            return _gridsPrivateModel.Items.First(x => x.GameId == gameId && x.OwnerActorNr == actorNr);
        }

        /// <summary>
        /// Получить ячейку
        /// grid - игровая сетка, на которой будем искать селл
        /// cellIndexW - позиция селла в игровой сетке
        /// cellIndexH - позиция селла в игровой сетке
        /// </summary>
        public Cell GetCell(IGrid grid, int cellIndexW, int cellIndexH)
        {
            if (!HasCell(grid, cellIndexW, cellIndexH))
                return null;

            int cellIndex = (grid.SizeGridW * cellIndexH) + cellIndexW;

            if (cellIndex < grid.CellsList.Length)
            {
                return grid.CellsList[cellIndex];
            }

            return null;
        }

        public bool HasCell(IGrid grid, int cellIndexW, int cellIndexH)
        {
            if (cellIndexW < 0
                || cellIndexW >= grid.SizeGridW
                || cellIndexH >= grid.SizeGridH)
            {
                return false; // вказаний селл вийшов за рамки ігрової сітки
            }

            return true;
        }

        public void RemoveAllIfExist(string gameId)
        {
            List<IGrid> grids = _gridsPrivateModel.Items.FindAll(x => x.GameId == gameId);
            foreach (IGrid grid in grids)
            {
                _gridsPrivateModel.Items.Remove(grid);
            }
        }

        public void Remove(string gameId, int actorNr)
        {
            if (!IsExist(gameId, actorNr))
                return;

            _gridsPrivateModel.Items.Remove(Get(gameId, actorNr));
        }

        /// <summary>
        /// Игрок перетаскивает юнита
        /// Может ли юнит стать в текущих координатах на игровой сетке?
        ///
        /// То есть, несколько юнитов не могут стоять в одних и тех же позиции по высоте H
        /// Также нужно учитывать ширину обьекта
        /// dragEndDropUnit - юнит, которого перетаскиваем
        /// unitPositionW - текущая позиция перетаскиваемого юнита
        /// unitPositionH - текущая позиция перетаскиваемого юнита
        /// draggingBodyWidth - ширина тела перетаскиваемого юнита
        /// </summary>
        public bool CheckCellsByOccupant(IGrid grid, IUnit dragEndDropUnit, int unitPositionW, int unitPositionH, int draggingBodyWidth)
        {
            bool isDragAndDropBarrier = dragEndDropUnit is IBarrierComponent;// _entityManager.HasComponent<BarrierComponent>(dragEndDropUnit);
            bool isDragAndDropUnit = !isDragAndDropBarrier;
            //var dragAndDropUnitComponent = _entityManager.GetComponentData<UnitComponent>(dragEndDropUnit);

            // Перебираем все ячейки по ширине юнита
            // Если все ячейки пусты и не заняты другими юнитами,
            // то текущий проверяемый юнит может встать на текущие координаты
            bool allowed = true;
            for (int i = 0; i < draggingBodyWidth; i++)
            {
                var checkingPosition = new Int2(unitPositionW + i, (int)unitPositionH);

                Cell cellUnderDragAndDropPosition = GetCell(grid, checkingPosition.x, checkingPosition.y);

                if (cellUnderDragAndDropPosition == null)
                {
                    allowed = false;    // юнит по ширине не влезает в проверяемые селлы
                    break;
                }

                // Проверяем, кто то стоит в текущем селле
                //DynamicBuffer<OccupiedComponent> occupiedList = _entityManager.GetBuffer<OccupiedComponent>(cellUnderDragAndDropPosition);

                if (cellUnderDragAndDropPosition.Occupied.Count > 0)
                {
                    // Перебираем каждого оккупанта селла и проверяем, он стоит в текущих координатах
                    foreach (IUnit occupied in cellUnderDragAndDropPosition.Occupied)
                    {
                        //var occupiedUnit = _entityManager.GetComponentData<UnitComponent>(occupied.entity);

                        if (occupied.UnitId == dragEndDropUnit.UnitId &&
                            occupied.InstanceId == dragEndDropUnit.InstanceId)
                        {
                            continue;    // Проверка на перетаскиваемого юнита. Что бы он сам себе не запретил перетаскивание
                        }

                        bool isOccupiedBarrier = occupied is IBarrierComponent;
                        bool isOccupiedUnit = !isOccupiedBarrier;

                        if (isDragAndDropBarrier && isOccupiedUnit)
                        {
                            continue;    // преграда может стоять в ячейке, где уже находится юнит
                        }
                        else if (isDragAndDropUnit && isOccupiedBarrier)
                        {
                            continue;    // юнит может стоять в ячейке, где уже находится преграда
                        }

                        //var occupiedPositionOnGrid = _entityManager.GetComponentData<PositionOnGridComponent>(occupied.entity);

                        if (checkingPosition.y == occupied.Position.y)
                        {
                            allowed = false;    // какой то юнит уже стоит в текущей позиции
                            break;
                        }
                    }

                }

            }

            return allowed;
        }

        /// <summary>
        /// Получить все селлы, по которым может перемещатся юнита
        /// </summary>
        public List<Cell> GetWalkCells(IGrid grid)
        {
            List<Cell> list = new List<Cell>();

            foreach (Cell cell in grid.CellsList)
            {
                if (cell.binaryMask == Enums.CellMask.floor)
                    list.Add(cell);
            }

            return list;
        }

        /// <summary>
        /// 
        /// </summary>
        public Int2 GetFreePosition(IGrid grid, IUnit unit)
        {
            // Получить список с ячейками пола, на которых можно заспавнить юнита
            List<Cell> walkCells = GetWalkCells(grid);

            Cell cellComponent;
            bool isCorrect = true;

            var areaInfoComponent = unit.BodySize;

            Random random = new Random();

            do
            {
                isCorrect = true;

                cellComponent = walkCells[random.Next(0, walkCells.Count - 1)];

                Cell checkingCell = cellComponent;

                // Проверяем все селлы на всю ширину юнита
                // Все селлы должны быть ходибельными
                for (int i = 0; i < areaInfoComponent.x; i++)
                {
                    if (checkingCell.binaryMask != Enums.CellMask.floor)
                    {
                        isCorrect = false;    // селл является ходячей стеной. По ней можно ходить, но встать на нее нельзя
                        break;
                    }

                    checkingCell = checkingCell.right;
                }


                if (isCorrect)
                {
                    // Также проверяем, если перетаскиваемый юнит находится в тех же координатах,
                    // в которых уже находится другой какой то юнит, то перетаскиваемый
                    // юнит нельзя позиционировать в этих же координатах. То есть, несколько юнитов
                    // не могут стоять в одних и тех же позиции по высоте H
                    // Также нужно учитывать ширину обьекта
                    isCorrect = CheckCellsByOccupant(grid,
                                                     unit,
                                                     (int)cellComponent.wIndex,
                                                     (int)cellComponent.hIndex,
                                                     (int)areaInfoComponent.x);
                }


            } while (!isCorrect);

            return new Int2((int)cellComponent.wIndex, (int)cellComponent.hIndex);
        }

        /// <summary>
        /// Освободить занятый район ячеек
        /// occupiedEntity - оккупант, от которого нужно освободить
        /// grid - игровая сетка, на которой нужно освободить
        /// cellIndexH - позиция селла на игровой сетке
        /// cellIndexW - позиция селла на игровой сетке
        /// heightArea - размер юнита в ширину и высоту
        /// widthArea  - размер юнита в ширину и высоту
        /// </summary>
        public void FreeArea(IUnit occupiedEntity, IGrid grid, int cellIndexH, int cellIndexW, int heightArea, int widthArea)
        {
            uint localW = 0;
            uint localH = 0;
            Cell cell;
            Cell bestLeftCell;                            // самая левая ячейка
            Cell cellComponent;

            // Находим левую нижнею ячейку и вручную выбрасываем ее из сетки
            cell = GetCell(grid, cellIndexW, cellIndexH);
            bestLeftCell = cell;
            cellComponent = cell;

            // Использую локальную сетку сущности, освобождаем остальные ячейки
            int areaSize = widthArea * heightArea;
            for (int i = 0; i < areaSize; i++)
            {
                FreeArea(occupiedEntity,
                         grid,
                         (int)cellComponent.wIndex,
                         (int)cellComponent.hIndex);

                // Двигаемся дальше по локальной сетке
                localW++;

                if (localW >= widthArea)
                {
                    localH++;
                    localW = 0;

                    bestLeftCell = bestLeftCell.up;
                    cell = bestLeftCell;
                }
                else
                {
                    cell = cellComponent.right;
                }

                if (cell != null)
                {
                    cellComponent = cell;
                }
            }
        }

        /// <summary>
        /// Освободить указаную ячейку от оккупанта
        /// occupiedEntity - оккупант
        /// grid - игровая сетка
        /// cellIndexH - индекс селла на указаной игровой сетке
        /// cellIndexW - индекс селла на указаной игровой сетке
        /// </summary>
        public void FreeArea(IUnit occupiedEntity,
                             IGrid grid,
                             int cellIndexW,
                             int cellIndexH)
        {
            Cell cell = GetCell(grid, cellIndexW, cellIndexH);
            if (cell == null)
                return;

            // Перебираем всех окупантов в текущей ячейки
            List<IUnit> occupiedList = cell.Occupied;

            int occupiedIndex = 0;
            bool foundOccupied = false;
            foreach (IUnit occupiedData in occupiedList)
            {
                if (occupiedData == occupiedEntity)
                {
                    foundOccupied = true;
                    break;
                }

                occupiedIndex++;
            }

            // Если в текущей ячейке есть occupiedEntity, то освобождаем ее
            if (foundOccupied)
            {
                occupiedList.RemoveAt(occupiedIndex);
            }
        }

        /// <summary>
        /// Занять окупантом указаный район ячеек
        /// occupiedEntity - оккупант, которым нужно занять указаную ячейку
        /// grid - игровая сетка
        /// cellIndexH - индекс селла на указаной игровой сетке
        /// cellIndexW - индекс селла на указаной игровой сетке
        /// heightArea - высота и ширина юнита
        /// widthArea  - высота и ширина юнита
        /// </summary>
        /*public void BusyArea(IUnit occupiedEntity, IGrid grid, int cellIndexH, int cellIndexW, int heightArea, int widthArea)
        {
            uint localW = 0;
            uint localH = 0;
            Cell cell;
            Cell bestLeftCell;   // самая левая ячейка
            Cell cellComponent;
            List<IUnit> cellOccupiedList;

            // Находим левую нижнею ячейку и вручную выбрасываем ее из сетки
            cell = GetCell(grid, cellIndexW, cellIndexH);
            bestLeftCell = cell;
            cellComponent = cell;
            cellOccupiedList = cell.Occupied;


            // Использую локальную сетку сущности, освобождаем остальные ячейки
            int areaSize = widthArea * heightArea;
            for (int i = 0; i < areaSize; i++)
            {
                // Добавить окупанта в заданую ячейку
                cellOccupiedList.Add(occupiedEntity);



                // Двигаемся дальше по локальной сетке
                localW++;

                if (localW >= widthArea)
                {
                    localH++;
                    localW = 0;

                    // bestLeftCell      = bestLeftCell != Entity.Null ? _entityManager.GetComponentData<CellComponent>(bestLeftCell).up : Entity.Null;
                    bestLeftCell = bestLeftCell.up;
                    cell = bestLeftCell;
                }
                else
                {
                    cell = cellComponent.right;
                }

                if (cell != null)
                {
                    cellComponent = cell;
                    cellOccupiedList = cell.Occupied;
                }
            }
        }*/

        /// <summary>
        /// Занять окупантом указаную ячейку
        /// occupiedEntity - оккупант, которым нужно занять указаную ячейку
        /// grid - игровая сетка
        /// cellIndexW - индекс селла на указаной игровой сетке
        /// cellIndexH - индекс селла на указаной игровой сетке
        /// </summary>
        public void BusyArea(IUnit occupiedUnit, IGrid grid, int cellIndexW, int cellIndexH)
        {
            Cell cell = GetCell(grid, cellIndexW, cellIndexH);

            if (cell == null)
                return;

            // Добавить окупанта в заданую ячейку
            cell.Occupied.Add(occupiedUnit);
        }
    }
}

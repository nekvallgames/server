using Plugin.Interfaces;
using Plugin.Models.Private;
using Plugin.Runtime.Services.UnitsPath;
using Plugin.Schemes;
using Plugin.Tools;
using System.Collections.Generic;
using System.Linq;

namespace Plugin.Runtime.Services
{
    /// <summary>
    /// Сервіс, котрий вираховує сели, по котрим юніт може переміщатись,
    /// де може переміщатись, але не може стати, та селли, на котрих юніт може стати всією своєю шириною  
    /// </summary>
    public class CellWalkableService
    {
        private UnitsPathService _unitsPathService;
        private GridService _gridService;
        private CellWalkablePrivateModel _cellWalkablePrivateModel;

        public CellWalkableService(UnitsPathService unitsPathService,
                                   GridService gridService,
                                   CellWalkablePrivateModel cellWalkablePrivateModel)
        {
            _unitsPathService = unitsPathService;
            _gridService = gridService;
            _cellWalkablePrivateModel = cellWalkablePrivateModel;
        }

        public void CreateScheme(string gameId, int actorNr)
        {
            if (_cellWalkablePrivateModel.Items.Any(x => x.GameId == gameId && x.OwnerActorNr == actorNr)){
                return;
            }

            _cellWalkablePrivateModel.Add(new CellWalkablePrivateScheme(gameId, actorNr));
        }

        public CellWalkablePrivateScheme Get(string gameId, int actorNr)
        {
            return _cellWalkablePrivateModel.Get(gameId, actorNr);
        }

        public void Calculate(string gameId, int actorNr)
        {
            
        }

        public void Calculate(IUnit unit)
        {
            CellWalkablePrivateScheme cellWalkablePrivateScheme = _cellWalkablePrivateModel.Get(unit.GameId, unit.OwnerActorNr);
            
            ResetCells(ref cellWalkablePrivateScheme.CellsUnderPath);
            EnableWalkables(unit, cellWalkablePrivateScheme, ref cellWalkablePrivateScheme.CellsUnderPath);
             
            CalculateSettles(ref cellWalkablePrivateScheme.CellsUnderPath, ref cellWalkablePrivateScheme.CellsSettle);
            CalculateSettleAreas(unit, ref cellWalkablePrivateScheme.CellsSettle, ref cellWalkablePrivateScheme.CellsSettleArea);
        }

        /// <summary>
        /// Сбрасываем на активной игровой сетке все ячейки с флажком IsWay
        /// </summary>
        private void ResetCells(ref List<Cell> cellsUnderPath)
        {
            foreach (Cell cell in cellsUnderPath){
                cell.IsWalk = false;
            }

            cellsUnderPath.Clear();
        }

        /// <summary>
        /// Селлы игровой сетки, по которым может перемещатся перетаскиваемый юнит, IsWalk сделать true
        /// Что бы контролировать, куда можно, а куда нет, перетаскивать юнита
        ///
        /// _controller.dragEndDropUnit - юнит, которого игрок перетаскивает
        /// </summary>
        private void EnableWalkables(IUnit unit, CellWalkablePrivateScheme cellWalkablePrivateScheme, ref List<Cell> cellsUnderPath)
        {
            // 1. Вытащить данные о том, куда юнита можно перемещать
            UnitPathPrivateScheme unitPathScheme = _unitsPathService.GetPathUnit(unit.GameId, 
                                                                                 unit.OwnerActorNr,
                                                                                 unit.UnitId,
                                                                                 unit.InstanceId);

            var moveTypeComponent = ((IWalkableComponent)unit);
            var isBorderMoveType = ((IWalkableComponent)unit).IsGodModeMovement;

            IGrid grid = _gridService.Get(unit.GameId, unit.OwnerActorNr);

            // 0. Проверяем юнита на его тип перемещения.
            // Если тип Enums.DragAndDropType.FreeWay      - значит делаем все ячейки, по которым перемещается юнит открытыми для перемещения
            // Если тип Enums.DragAndDropType.UseUnitWay   - значит юнит ограничен в перемещении
            if (isBorderMoveType)
            {
                // По этих клеточках может перемещатся юнит    moveTypeComponent.wayMask
                // Все клеточки сделать доступными для перемещения
                EnableWalkableByMask(grid, true, moveTypeComponent.WayMask, cellWalkablePrivateScheme.IgnoreList, ref cellsUnderPath);
            }
            else
            {
                // Сделать ячейки ходибельными, по которым юнит может перемещатся
                EnableWalkableByPath(grid, unitPathScheme, cellWalkablePrivateScheme.IgnoreList, ref cellsUnderPath);
            }
        }

        /// <summary>
        /// В текущих координатах юнит может стать всей своей шириной тела ?
        /// </summary>
        public bool CanStayHere(IUnit unit, int w, int h)
        {
            CellWalkablePrivateScheme cellWalkablePrivateScheme = _cellWalkablePrivateModel.Get(unit.GameId, unit.OwnerActorNr);

            if (cellWalkablePrivateScheme == null)
                return false;

            return cellWalkablePrivateScheme.CellsSettleArea.Any(x => x.wIndex == w && x.hIndex == h);
        }

        private void EnableWalkableByPath(IGrid grid, UnitPathPrivateScheme pathSchemeUnit, List<(int, int)> ignoreList, ref List<Cell> cellsUnderPath)
        {
            foreach ((int, int) pathCell in pathSchemeUnit.Path)
            {
                EnableWalkableByPosition(grid, true, pathCell.Item1, pathCell.Item2, ignoreList, ref cellsUnderPath);
            }
        }

        /// <summary>
        /// Отобразить на игровой сетке, ячейки, по которым юнит может перемещатся
        /// isWalk - можно/нельзя ходить по этому сселу
        /// findW findH индекс селла
        /// </summary>
        private void EnableWalkableByPosition(IGrid grid, bool isWalk, int positionW, int positionH, List<(int, int)> ignoreList, ref List<Cell> cellsUnderPath)
        {
            Cell cell = _gridService.GetCell(grid, positionW, positionH);

            if (cell == null)
                return;

            cell.IsWalk = isWalk;

            if (isWalk
                && !ignoreList.Any(x => x.Item1 == positionW && x.Item2 == positionH)) // додаткова перевірка на ігнор ліст
            {
                cellsUnderPath.Add(cell);
            }
        }

        /// <summary>
        /// Перебираем все клеточки, и те клеточки, которые подходят для маски, делаем их параметр IsWalk = isWalk
        /// </summary>
        private void EnableWalkableByMask(IGrid grid, bool isWalk, int binaryMask, List<(int, int)> ignoreList, ref List<Cell> cellsUnderPath)
        {
            for (uint i = 0; i < grid.CellsList.Length; i++)
            {
                Cell cell = grid.CellsList[i];

                int cellMask = (int)cell.binaryMask;
                int isMaskCorrect = (cellMask & binaryMask);

                if (isMaskCorrect > 0)
                {
                    cell.IsWalk = isWalk;
                    if (isWalk
                        && !ignoreList.Any(x => x.Item1 == cell.wIndex && x.Item2 == cell.hIndex))    // додаткова перевірка на ігнор ліст
                    {
                        cellsUnderPath.Add(cell);
                    }
                }
            }
        }

        private void CalculateSettles(ref List<Cell> cellsUnderPath, ref List<Cell> cellsSettle)
        {
            cellsSettle.Clear();

            foreach (Cell cell in cellsUnderPath)
            {
                uint cellMask = (uint)cell.binaryMask;
                uint IsBackFloorWall = (cellMask & (uint)Enums.CellMask.backFloorWall);

                if (IsBackFloorWall > 0)
                {
                    // текущий селл является ходибельной стеной. Текущий селл не добавляем
                }
                else
                {
                    cellsSettle.Add(cell);
                }
            }
        }

        /// <summary>
        /// Перебрати всі селли, на котрих може зупинитися юніт, і знайти сели,
        /// на котрих юніт зможе зупинитися, враховуючи його ширину
        /// </summary>
        private void CalculateSettleAreas(IUnit unit, ref List<Cell> cellsSettle, ref List<Cell> cellsSettleArea)
        {
            cellsSettleArea.Clear();

            // Вытащить ширину и высоту перетаскиваемого юнита
            //var areaInfo = _entityManager.GetComponentData<AreaInfoComponent>(unit.UnitEntity);
            int draggingBodyWidth = unit.BodySize.x;

            //var unitComponent = _entityManager.GetComponentData<UnitComponent>(unit.UnitEntity);
            IGrid grid = _gridService.Get(unit.GameId, unit.OwnerActorNr);

            // Перебираем все селлы, которые подходят для перемещения перетаскиваемого юнита
            foreach (Cell cell in cellsSettle)
            {
                Cell checkingCell = cell;

                // Проверяем все селлы на всю ширину юнита
                // Все селлы должны быть ходибельными
                bool isCorrect = true;
                for (int i = 0; i < draggingBodyWidth; i++)
                {
                    if ((checkingCell.binaryMask & Enums.CellMask.backFloorWall) > 0)
                    {
                        isCorrect = false;    // селл является ходячей стеной. По ней можно ходить, но встать на нее нельзя
                        break;
                    }

                    bool isWalkable = checkingCell.IsWalk;

                    if (isWalkable)
                    {
                        checkingCell = cell.right;
                    }
                    else
                    {
                        isCorrect = false;
                        break;
                    }
                }


                if (isCorrect)
                {
                    // Также проверяем, если перетаскиваемый юнит находится в тех же координатах,
                    // в которых уже находится другой какой то юнит, то перетаскиваемый
                    // юнит нельзя позиционировать в этих же координатах. То есть, несколько юнитов
                    // не могут стоять в одних и тех же позиции по высоте H
                    // Также нужно учитывать ширину обьекта
                    isCorrect = _gridService.CheckCellsByOccupant(grid,
                                                                  unit,
                                                                  (int)cell.wIndex,
                                                                  (int)cell.hIndex,
                                                                  (int)draggingBodyWidth);
                }

                if (isCorrect)
                {
                    cellsSettleArea.Add(cell);
                }
            }
        }

        /// <summary>
        /// Добавити позицію до ігнор списка
        /// </summary>
        public void AddPositionToIgnoreList(string gameId, int actorNr, (int, int) ignorePosition)
        {
            CellWalkablePrivateScheme cellWalkablePrivateScheme = _cellWalkablePrivateModel.Get(gameId, actorNr);
            cellWalkablePrivateScheme.IgnoreList.Add(ignorePosition);
        }

        public bool IsIgnorePosition(string gameId, int actorNr, int w, int h)
        {
            CellWalkablePrivateScheme cellWalkablePrivateScheme = _cellWalkablePrivateModel.Get(gameId, actorNr);
            
            return cellWalkablePrivateScheme.IgnoreList.Any(x => x.Item1 == w && x.Item2 == h);
        }

        /// <summary>
        /// Очистити ігнор список
        /// </summary>
        public void ClearIgnoreList(string gameId, int actorNr)
        {
            CellWalkablePrivateScheme cellWalkablePrivateScheme = _cellWalkablePrivateModel.Get(gameId, actorNr);

            cellWalkablePrivateScheme.IgnoreList.Clear();
        }

        public void RemoveAllIfExist(string gameId)
        {
            List<CellWalkablePrivateScheme> schemes = _cellWalkablePrivateModel.Items.FindAll(x => x.GameId == gameId);
            foreach (CellWalkablePrivateScheme scheme in schemes)
            {
                _cellWalkablePrivateModel.Items.Remove(scheme);
            }
        }
    }
}

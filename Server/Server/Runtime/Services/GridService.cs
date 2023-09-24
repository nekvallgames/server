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
            bool isDragAndDropBarrier = dragEndDropUnit is IBarrier;// _entityManager.HasComponent<BarrierComponent>(dragEndDropUnit);
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

                        bool isOccupiedBarrier = occupied is IBarrier;
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
    }
}

using Plugin.Interfaces;
using Plugin.Schemes;
using Plugin.Tools;
using System.Collections.Generic;
using System.Linq;

namespace Plugin.Runtime.Services
{
    /// <summary>
    /// Вказавши юніта, сервіс поверне всі сели, по котрим можно атакувати юніта
    /// Тобто беремо всі сели, по котрим може переміщатися юніт,
    /// і також добавляємо всі сели на висоту юніта
    /// </summary>
    public class HitAreaService
    {
        private CellWalkableService _cellWalkableService;
        private UnitsService _unitsService;
        private GridService _gridService;

        public HitAreaService(CellWalkableService cellWalkableService,
                              UnitsService unitsService,
                              GridService gridService)
        {
            _cellWalkableService = cellWalkableService;
            _unitsService = unitsService;
            _gridService = gridService;
        }

        /// <summary>
        /// Отримати всі сели, котрі знаходяться за спиною юніта
        /// на всих позиціях, де юніт може переміститися
        /// Наприклад, юніт може переміститися на 3 різні позиції,
        /// то функція перебере всі 3-и позиції, і поверне всі сели,
        /// котрі будуть за спиною юніта
        /// </summary>
        public void GetWayBodyCellsArea(IUnit unit, ref List<(int, int)> list)
        {
            _cellWalkableService.Calculate(unit);

            // Перебираємо всі селли, на котрих юніт може стати
            // Тепер потрібно перебрати всі сели на висоту і ширину юніта, для подальшої атаки
            CellWalkablePrivateScheme cellWalkablePrivateScheme = _cellWalkableService.Get(unit.GameId, unit.OwnerActorNr);
            foreach (Cell settleAreaPos in cellWalkablePrivateScheme.CellsSettleArea)
            {
                GetBodyCellsArea(settleAreaPos, unit.BodySize, ref list);
            }
        }

        /// <summary>
        /// Отримати всі сели, котрі знаходяться за спиною юніта
        /// Наприклад, розмір юніта 3 на 4, то функція поверне 12 селів
        /// </summary>
        public void GetBodyCellsArea(IUnit unit, ref List<(int, int)> list)
        {
            IGrid grid = _gridService.Get(unit.GameId, unit.OwnerActorNr);

            //var positionOnGrid = _entityManager.GetComponentData<PositionOnGridComponent>(unit.UnitEntity);

            Cell cellEntityAnchor = _gridService.GetCell(grid, unit.Position.x, unit.Position.y);
            //CellComponent cellAnchor = _entityManager.GetComponentData<CellComponent>(cellEntityAnchor);

            GetBodyCellsArea(cellEntityAnchor, unit.BodySize, ref list);
        }

        /// <summary>
        /// Отримати всі сели, котрі знаходяться за спиною юніта
        /// Наприклад, розмір юніта 3 на 4, то функція поверне 12 селів
        /// </summary>
        public void GetBodyCellsArea(Cell cellAnchor, Int2 sizeBody, ref List<(int, int)> list)
        {
            Cell cellRow = cellAnchor;

            for (int i = 0; i < sizeBody.x; i++)
            {
                Cell localCell = cellRow;

                for (int j = 0; j < sizeBody.y; j++)
                {
                    if (!list.Any(x => x.Item1 == localCell.wIndex && x.Item2 == localCell.hIndex))
                    {
                        list.Add(((int)localCell.wIndex, (int)localCell.hIndex));
                    }

                    Cell nextLocalCell = localCell.up;
                    if (nextLocalCell == null)
                        break;

                    localCell = nextLocalCell;
                }

                Cell nextRowCell = cellRow.right;
                if (nextRowCell == null)
                    break;

                cellRow = nextRowCell;
            }
        }
    }
}

using Plugin.Interfaces;
using Plugin.Runtime.Services.UnitsPath;
using Plugin.Schemes;
using Plugin.Tools;
using System.Collections.Generic;

namespace Plugin.Runtime.Services.AI.Inputs
{
    /// <summary>
    /// Інпут, котрий відповідає за дані, вказавши позицію на ігровій сітці, 
    /// отримати інформацію, скільки селів є для переміщення юніта
    /// Тобто, вказавши точку на ігровій сітці, отримати інформацію, скільки селів доступно для переміщення
    /// </summary>
    public class NextWayAreaInput : BaseInput
    {
        private GridService _gridService;
        private UnitsPathService _unitsPathService;
        private IUnit _unit;
        private Int2 _destination;

        public NextWayAreaInput(IUnit unit, Cell destination, GridService gridService, UnitsPathService unitsPathService)
        {
            _unit = unit;
            _destination = new Int2((int)destination.wIndex, (int)destination.hIndex);
            _gridService = gridService;
            _unitsPathService = unitsPathService;
        }

        public override float Size
        {
            get
            {
                IGrid grid = _gridService.Get(_unit.GameId, _unit.OwnerActorNr);

                List<(int, int)> wayList = _unitsPathService.CalculateAndGetPathUnit(_unit, _destination.x, _destination.y, grid);
                int capacity = 0;
                foreach ((int, int) pathCell in wayList)
                {
                    Cell entity = _gridService.GetCell(grid, pathCell.Item1, pathCell.Item2);
                    if (entity != null)
                    {
                        capacity += entity.binaryMask == Enums.CellMask.cellWalkLock ? 1 : 0;
                    }
                }

                return capacity;
            }
        }
    }
}

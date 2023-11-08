using Plugin.Interfaces;
using Plugin.Schemes;
using Plugin.Tools;
using System.Collections.Generic;
using System.Linq;

namespace Plugin.Runtime.Services.AI.Inputs
{
    /// <summary>
    /// Інпут, котрий відповідає за дані, дистанція до ближащого юніта
    /// </summary>
    public class DistanceToNearUnitInput : BaseInput
    {
        private UnitsService _unitsService;
        private IUnit _unit;
        private GridService _gridService;
        private Int2 _destinationWH;

        public DistanceToNearUnitInput(IUnit unit, 
                                       Cell destination, 
                                       UnitsService unitsService, 
                                       GridService gridService)
        {
            _unit = unit;
            _destinationWH = new Int2((int)destination.wIndex, (int)destination.hIndex);

            _unitsService = unitsService;
            _gridService = gridService;
        }

        public override float Size
        {
            get
            {
                IGrid grid = _gridService.Get(_unit.GameId, _unit.OwnerActorNr);

                var teamUnits = new List<IUnit>();
                _unitsService.GetAliveUnits(_unit.GameId, _unit.OwnerActorNr, ref teamUnits);

                var distanceResult = new List<float>();

                foreach (IUnit teamUnit in teamUnits)
                {
                    if (_unit.Compare(teamUnit))
                    {
                        continue;
                    }

                    float distance = Int2.Distance(_destinationWH, teamUnit.Position);
                    distanceResult.Add(distance);
                }

                if (!distanceResult.Any())
                    return 0;    // в гравця залишився тільки 1 юніт. 

                distanceResult.Sort((x, y) => x.CompareTo(y));

                return distanceResult[0];
            }
        }


    }
}

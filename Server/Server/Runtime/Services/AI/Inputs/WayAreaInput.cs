using Plugin.Interfaces;
using Plugin.Runtime.Services.UnitsPath;
using System;

namespace Plugin.Runtime.Services.AI.Inputs
{
    /// <summary>
    /// Інпут, котрий вирахує число селлів, по котрим може переміщатися вказаний юніт
    /// Чим більше селів, тим більше число
    /// </summary>
    public class WayAreaInput : BaseInput
    {
        private UnitsPathService _unitsPathService;
        private IUnit _unit;

        public WayAreaInput(IUnit unit, UnitsPathService unitsPathService)
        {
            _unit = unit;
            _unitsPathService = unitsPathService;
        }

        public override float Size => Math.Max(1, GetWayArea(_unit));

        /// <summary>
        /// Отримати кількість селлів, куди може переміститися юніт
        /// </summary>
        private float GetWayArea(IUnit unit)
        {
            return _unitsPathService.GetPathUnit(unit.GameId, unit.OwnerActorNr, unit.UnitId, unit.InstanceId).Path.Count;
        }
    }
}

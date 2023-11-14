using Plugin.Interfaces;
using Plugin.Runtime.Services.UnitsPath;
using System;

namespace Plugin.Runtime.Services.AI.Inputs
{
    /// <summary>
    /// Інпут, котрий порівняє рівень зони переміщення вказаних юнітів
    /// Якщо юніт _u1Id буде мати однаковий кількість селлів для переміщення із юнітом _u2Id, то результат Size == 0
    /// Якщо юніт 1 буде мати більше селлів для переміщення, чим у юніта 2, то результат Size буде > 0
    /// Якщо юніт 1 буде мати менше селлів для переміщення, чим у юніта 2, то результат Size буде == 0
    /// </summary>
    public class WayAreaU1BiggerU2Input : BaseInput
    {
        private UnitsPathService _unitsPathService;

        private IUnit _u1Id;
        private IUnit _u2Id;

        public WayAreaU1BiggerU2Input(IUnit u1Id, IUnit u2Id, UnitsPathService unitsPathService)
        {
            _u1Id = u1Id;
            _u2Id = u2Id;

            _unitsPathService = unitsPathService;
        }

        public override float Size
        {
            get
            {
                float u1Value = Math.Max(1, GetWayArea(_u1Id));
                float u2Value = Math.Max(1, GetWayArea(_u2Id));

                return CompareSize(u1Value, u2Value);
            }
        }

        /// <summary>
        /// Отримати кількість селлів, куди може переміститися юніт
        /// </summary>
        private float GetWayArea(IUnit unit)
        {
            return _unitsPathService.GetPathUnit(unit.GameId, unit.OwnerActorNr, unit.UnitId, unit.InstanceId).Path.Count;
        }
    }
}

using Plugin.Interfaces;
using Plugin.Models.Private;
using System.Collections.Generic;
using System.Linq;

namespace Plugin.Runtime.Services
{
    /// <summary>
    /// Сервіс, котрий створює інстанси для юнітів
    /// </summary>
    public class UnitInstanceService
    {
        private UnitsPrivateModel _unitsPrivateModel;

        public UnitInstanceService(UnitsPrivateModel unitsPrivateModel)
        {
            _unitsPrivateModel = unitsPrivateModel;
        }

        /// <summary>
        /// Отримати для вказаного юніта інстанс
        /// actorId - власник юніта
        /// unitId - ид юніта, котрого буде створено
        /// </summary>
        public int GetInstance(string gameId, int actorId, int unitId)
        {
            List<IUnit> list = _unitsPrivateModel.Items.FindAll(x => x.GameId == gameId && x.OwnerActorNr == actorId && x.UnitId == unitId);
            if (!list.Any())
                return 0;

            int maxInstance = list.Max(x => x.InstanceId);
            maxInstance++;

            return maxInstance;
        }
    }
}

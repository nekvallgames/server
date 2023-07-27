using Plugin.Builders;
using Plugin.Interfaces;
using Plugin.Models.Private;
using Plugin.Models.Public;
using Plugin.Runtime.Providers;
using Plugin.Schemes;
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
    }
}

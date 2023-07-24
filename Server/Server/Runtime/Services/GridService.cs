using Photon.Hive.Plugin;
using Plugin.Builders;
using Plugin.Interfaces;
using Plugin.Models.Private;
using Plugin.Models.Public;
using Plugin.Runtime.Providers;
using Plugin.Schemes;
using Plugin.Signals;
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
        private HostsService _hostsService;

        public GridService(PublicModelProvider publicModelProvider, 
                           PrivateModelProvider privateModelProvider, 
                           GridBuilder gridBuilder, 
                           SignalBus signalBus,
                           HostsService hostsService)
        {
            _locationsPublicModel = publicModelProvider.Get<LocationsPublicModel<LocationScheme>>();
            _gridsPrivateModel = privateModelProvider.Get<GridsPrivateModel>();
            _gridBuilder = gridBuilder;
            _hostsService = hostsService;

            signalBus.Subscrible<HostsPrivateModelSignal>(HostsModelChange);
        }

        /// <summary>
        /// Модель із даними хостів була змінена
        /// </summary>
        private void HostsModelChange(HostsPrivateModelSignal signalData )
        {
            IList<IActor> actors = _hostsService.Actors(signalData.GameId);

            foreach ( IActor actor in actors)
            {
                if (_gridsPrivateModel.Items.Any(x => x.GameId == signalData.GameId && x.OwnerActorId == actor.ActorNr))
                    continue;   // для поточного гравця вже створена ігрова сітка

                // Створити ігрову сітку для поточного гравця
                LocationScheme scheme = _locationsPublicModel.Items[0]; // TODO поки що постійно створюємо локацію за замовчуванням

                IGrid grid = _gridBuilder.Create(signalData.GameId, actor.ActorNr, scheme.SizeGrid, scheme.GridMask);

                _gridsPrivateModel.Add(grid);
            }
        }
    }
}

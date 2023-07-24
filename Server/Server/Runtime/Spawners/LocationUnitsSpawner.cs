using Plugin.Models.Public;
using Plugin.Runtime.Providers;
using Plugin.Runtime.Services;
using Plugin.Schemes;
using Plugin.Signals;

namespace Plugin.Runtime.Spawners
{
    /// <summary>
    /// Клас спавнер, котрий заспавнить юнітів, 
    /// наприклад барьєри, котрі йдуть в коплекті із локацією
    /// </summary>
    public class LocationUnitsSpawner
    {
        private LocationsPublicModel<LocationScheme> _locationsPublicModel;
        private UnitsService _unitsService;

        public LocationUnitsSpawner(PublicModelProvider publicModelProvider, 
                                    UnitsService unitsService,
                                    SignalBus signalBus)
        {
            _unitsService = unitsService;
            _locationsPublicModel = publicModelProvider.Get<LocationsPublicModel<LocationScheme>>();
            
            signalBus.Subscrible<GridsPrivateModelSignal>(OnGridsModelChange);
        }

        /// <summary>
        /// Модель із даними ігрових сіток була оновлена
        /// </summary>
        private void OnGridsModelChange(GridsPrivateModelSignal signalData)
        {
            if (signalData.Status != ModelChangeSignal.StatusType.add)
                return;

            // Створити юнітів, котрі йдуть в парі із ігровою локацією
            LocationScheme scheme = _locationsPublicModel.Items[0]; // TODO поки що постійно створюємо локацію за замовчуванням

            foreach (var unitData in scheme.LocationUnits)
            {
                _unitsService.CreateUnit(signalData.GameId, signalData.OwnerActorId, unitData.Item1, unitData.Item2, unitData.Item3);
            }
        }
    }
}

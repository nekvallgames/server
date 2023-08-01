using Plugin.Interfaces;
using Plugin.Runtime.Providers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Plugin.Runtime.Services
{
    /// <summary>
    /// Сервіс, за допомогою котрого будемо синхронізувати дані гравців із DB
    /// </summary>
    public class BackendBroadcastService
    {
        private IBackendBroadcastProvider _backendBroadcastProvider;
        private UnitLevelService _unitLevelService;

        public BackendBroadcastService(UnitLevelService unitLevelService)
        {
            _unitLevelService = unitLevelService;

            _backendBroadcastProvider = new BackendBroadcastProvider();
            _backendBroadcastProvider.Connect();
        }

        /// <summary>
        /// Синхронізувати для вказаного актора його дані із backend
        /// </summary>
        public async Task SyncActorData(IActorScheme actor)
        {
            (int, List<int>) actorData = await _backendBroadcastProvider.GetActorData(actor.ProfileId);

            // sync rating
            actor.Rating = actorData.Item1;

            // sync deck
            actor.Deck.Clear();
            foreach (int unitId in actorData.Item2)
            {
                actor.Deck.Add(unitId);
            }
        }

        /// <summary>
        /// Синхронізувати level юнітів, котрі у гравця знаходяться в колоді
        /// </summary>
        public async Task SyncLevelByDeck(IActorScheme actor)
        {
            List<(int, int)> ownCapacityUnits = await _backendBroadcastProvider.GetOwnCapacityUnits(actor.ProfileId);

            foreach (int unitId in actor.Deck)
            {
                if (ownCapacityUnits.Any(x => x.Item1 == unitId))
                {
                    int ownCapacity = ownCapacityUnits.First(x => x.Item1 == unitId).Item2;
                    actor.Levels.Add(_unitLevelService.GetLevel(unitId, ownCapacity));
                }
                else{
                    actor.Levels.Add(0);
                }
            }
        }

        /// <summary>
        /// Змінити кількість рейтинга для вказаного актора
        /// </summary>
        public async Task<int> ChangeRating(IActorScheme actor, int capacity)
        {
            actor.Rating += capacity;
            if (actor.Rating < 0)
                actor.Rating = 0;

            await _backendBroadcastProvider.SetRating(actor.ProfileId, actor.Rating);

            return actor.Rating;
        }

    }
}
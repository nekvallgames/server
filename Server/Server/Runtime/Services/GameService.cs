using Photon.Hive.Plugin;
using Plugin.Interfaces;
using Plugin.Schemes;
using Plugin.Signals;
using Plugin.Tools;
using System.Collections.Generic;
using System.Linq;

namespace Plugin.Runtime.Services
{
    public class GameService
    {
        private PlotsModelService _plotsModelService;
        private SyncProgressService _syncProgressService;
        private ActorService _actorService;
        private HostsService _hostsService;
        private ConvertService _convertService;

        public GameService(SignalBus signalBus, 
                           PlotsModelService plotsModelService, 
                           SyncProgressService syncProgressService,
                           ActorService actorService,
                           HostsService hostsService,
                           ConvertService convertService)
        {
            _plotsModelService = plotsModelService;
            _syncProgressService = syncProgressService;
            _actorService = actorService;
            _hostsService = hostsService;
            _convertService = convertService;

            signalBus.Subscrible<ActorLeftSignal>(OnActorLeft);
        }

        /// <summary>
        /// Подія, коли гравець покинув ігрову кімнату
        /// </summary>
        private async void OnActorLeft(ActorLeftSignal signalData)
        {
            IPlotModelScheme model = _plotsModelService.Get(signalData.Actor.GameId);
            IList<IActorScheme> actors = _actorService.GetActorsInRoom(signalData.Actor.GameId);

            if (model == null || model.IsAbort || model.IsGameFinished || actors.Count <= 0)
            {
                return;
            }

            model.IsAbort = true;

            // set win players, who still in room
            foreach (IActorScheme actor in actors)
            {
                if (!actor.IsLeft)
                {
                    model.WinnerActorsNr.Add(actor.ActorNr);
                }
            }

            // sync progress
            foreach (IActorScheme actor in actors)
            {
                await _syncProgressService.Sync(actor);
            }

            if (!actors.Any(x => !x.IsLeft))
            {
                // all actors left from room
                return;
            }

            // Создать коллекцию, которая будет хранить в себе данные, которые нужно синхронизировать 
            // между клиентами
            // key   - это ActorID
            // value - это StepScheme, которая имеет кучу из компонентов
            var pushData = new Dictionary<byte, object> { };

            IActorScheme actorInRoom = actors.First(x => !x.IsLeft);
            var scheme = new AbortScheme
            {
                rating = actorInRoom.Rating,
            };

            string jsonString = _convertService.SerializeObject(scheme);
            pushData.Add((byte)actorInRoom.ActorNr, jsonString);

            IPluginHost plugin = _hostsService.Get(signalData.Actor.GameId);
            plugin.BroadcastEvent(ReciverGroup.All,                   // отправить сообщение всем
                                  actorInRoom.ActorNr,                                  // номер актера, если нужно отправить уникальное сообщение
                                  0,
                                  OperationCode.abort,
                                  pushData,
                                  CacheOperations.DoNotCache);        // не кэшировать сообщение
        }
    }
}

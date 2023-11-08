using Photon.Hive.Plugin;
using Plugin.Interfaces;
using Plugin.Runtime.Services.UnitsPath;
using Plugin.Schemes;
using Plugin.Signals;
using Plugin.Tools;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Plugin.Runtime.Services
{
    public class GameService
    {
        private PlotsModelService _plotsModelService;
        private SyncProgressService _syncProgressService;
        private ActorService _actorService;
        private HostsService _hostsService;
        private ConvertService _convertService;
        private UnitsService _unitsService;
        private GridService _gridService;
        private UnitsPathService _unitsPathService;
        private CellWalkableService _cellWalkableService;

        public GameService(SignalBus signalBus, 
                           PlotsModelService plotsModelService, 
                           SyncProgressService syncProgressService,
                           ActorService actorService,
                           HostsService hostsService,
                           ConvertService convertService,
                           UnitsService unitsService,
                           GridService gridService,
                           UnitsPathService unitsPathService,
                           CellWalkableService cellWalkableService)
        {
            _plotsModelService = plotsModelService;
            _syncProgressService = syncProgressService;
            _actorService = actorService;
            _hostsService = hostsService;
            _convertService = convertService;
            _unitsService = unitsService;
            _gridService = gridService;
            _unitsPathService = unitsPathService;
            _cellWalkableService = cellWalkableService;

            signalBus.Subscrible<ActorLeftSignal>(OnActorLeft);
        }

        /// <summary>
        /// Подія, коли гравець покинув ігрову кімнату
        /// </summary>
        private async void OnActorLeft(ActorLeftSignal signalData)
        {
            IPlotModelScheme model = _plotsModelService.Get(signalData.Actor.GameId);
            IList<IActorScheme> actors = _actorService.GetActorsInRoom(signalData.Actor.GameId);

            if (model == null 
                || model.IsAbort 
                || model.IsGameFinished 
                || actors.Count <= 0
                || model.IsBeganSyncProgress
                || model.IsSyncProgressComplete
                || !model.IsStartRoom)
            {
                return;
            }

            model.IsAbort = true;

            // set win players, who still in room
            foreach (IActorScheme actor in actors){
                if (!actor.IsLeft){
                    model.WinnerActorsNr.Add(actor.ActorNr);
                }
            }

            await SyncProgress(signalData.Actor.GameId);

            if (!actors.Any(x => !x.IsLeft))
            {
                // all actors left from room
                TryToDisposeRoom(signalData.Actor.GameId);
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

        public async Task SyncProgress(string gameId)
        {
            IList<IActorScheme> actors = _actorService.GetActorsInRoom(gameId);
            IPlotModelScheme plotModel = _plotsModelService.Get(gameId);

            if (plotModel.IsBeganSyncProgress || plotModel.IsSyncProgressComplete)
                return;

            plotModel.IsBeganSyncProgress = true;

            foreach (IActorScheme actor in actors)
            {
                if (actor.IsAI)
                    continue;

                await _syncProgressService.Sync(actor);
            }

            plotModel.IsSyncProgressComplete = true;
        }

        public void CloseRoom(string gameId)
        {
            if (!_plotsModelService.IsExist(gameId))
                return;

            IPlotModelScheme plotModelScheme = _plotsModelService.Get(gameId);
            plotModelScheme.IsRoomClosed = true;

            TryToDisposeRoom(gameId);
        }

        public void TryToDisposeRoom(string gameId)
        {
            IPlotModelScheme plotModelScheme = _plotsModelService.Get(gameId);

            if (plotModelScheme.IsStartRoom && (!plotModelScheme.IsSyncProgressComplete || !plotModelScheme.IsRoomClosed))
                return;

            DisposeRoom(gameId);
        }

        private void DisposeRoom(string gameId)
        {
            _plotsModelService.RemoveIfExist(gameId);
            _unitsService.RemoveAllIfExist(gameId);
            _gridService.RemoveAllIfExist(gameId);
            _actorService.RemoveActorsInRoom(gameId);
            _unitsPathService.RemoveAllIfExist(gameId);
            _cellWalkableService.RemoveAllIfExist(gameId);
        }
    }
}

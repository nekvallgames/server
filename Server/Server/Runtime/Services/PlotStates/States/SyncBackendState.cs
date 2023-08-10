using Photon.Hive.Plugin;
using Plugin.Installers;
using Plugin.Interfaces;
using Plugin.Tools;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Plugin.Runtime.Services.PlotStates.States
{
    /// <summary>
    /// Стейт, в котрому потрібно синхронізувати backend перед початком гри
    /// Наприклад: отримати юнітів, котрими грає гравець
    /// </summary>
    public class SyncBackendState : BasePlotState
    {
        public const string NAME = "SyncBackendState";
        public override string Name => NAME;

        private BackendBroadcastService _backendBroadcastService;
        private ActorService _actorService;

        public SyncBackendState(PlotStatesService plotStatesService,
                                IPluginHost host, 
                                string nextState):base(plotStatesService, host, nextState)
        {
            var gameInstaller = GameInstaller.GetInstance();

            _backendBroadcastService = gameInstaller.backendBroadcastService;
            _actorService = gameInstaller.actorService;
        }

        public override void EnterState()
        {
            LogChannel.Log("PlotStatesService :: SyncBackendState :: EnterState()", LogChannel.Type.Plot);
            
            SyncBackend();
        }

        private async void SyncBackend()
        {
            await SyncDeck();

            plotStatesService.ChangeState(nextState);
        }

        private async Task SyncDeck()
        {
            List<IActorScheme> actors = _actorService.GetActorsInRoom(host.GameId);

            foreach (IActorScheme actor in actors)
            {
                await _backendBroadcastService.SyncActorData(actor);
                await _backendBroadcastService.SyncLevelByDeck(actor);
            }
        }
    }
}

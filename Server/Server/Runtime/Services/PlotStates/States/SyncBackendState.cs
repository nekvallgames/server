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
    public class SyncBackendState : BasePlotState, IState
    {
        public const string NAME = "SyncBackendState";
        public string Name => NAME;

        private SignalBus _signalBus;
        private IBackendBroadcastProvider _backendBroadcastProvider;
        private ActorService _actorService;

        public SyncBackendState(PlotStatesService plotStatesService,
                                IPluginHost host, 
                                string nextState):base(plotStatesService, host, nextState)
        {
            var gameInstaller = GameInstaller.GetInstance();

            _signalBus = gameInstaller.signalBus;
            _backendBroadcastProvider = gameInstaller.backendBroadcastProvider;
            _actorService = gameInstaller.actorService;
        }

        public void EnterState()
        {
            LogChannel.Log("PlotStatesService :: SyncStartState :: EnterState()", LogChannel.Type.Plot);
            
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
                List<int> deck = await _backendBroadcastProvider.GetUnitsInDeck(actor.ProfileId);

                actor.Deck.Clear();
                foreach (int unitId in deck)
                {
                    actor.Deck.Add(unitId);
                }
            }
        }

        public void ExitState()
        {
            
        }
    }
}

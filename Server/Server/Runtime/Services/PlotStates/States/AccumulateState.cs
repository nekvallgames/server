using Photon.Hive.Plugin;
using Plugin.Installers;
using Plugin.Interfaces;
using Plugin.Signals;
using Plugin.Tools;

namespace Plugin.Runtime.Services.PlotStates.States
{
    /// <summary>
    /// Стейт, в котрому чекаємо, поки кімната збере необхідну кількість гравців
    /// </summary>
    public class AccumulateState : BasePlotState, IState
    {
        public const string NAME = "AccumulateState";
        public string Name => NAME;

        private SignalBus _signalBus;
        private HostsService _hostsService;

        /// <summary>
        /// Кількість гравців, котрі потрібні для старту ігрової кімнати
        /// </summary>
        private int _countActors;

        private bool _isIgnore;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="countActors">кількість гравців, котрі потрібні для старту ігрової кімнати</param>
        /// <param name="nextState">перейти в вказаний стейт</param>
        public AccumulateState(PlotStatesService plotStatesService, 
                               IPluginHost host, 
                               int countActors, 
                               string nextState):base(plotStatesService, host, nextState)
        {
            _countActors = countActors;

            var gameInstaller = GameInstaller.GetInstance();

            _signalBus = gameInstaller.signalBus;
            _hostsService = gameInstaller.hostsService;
        }

        public void EnterState()
        {
            LogChannel.Log("PlotStatesService :: AccumulateState :: EnterState()", LogChannel.Type.Plot);
            _isIgnore = false;

            _signalBus.Subscrible<HostsPrivateModelSignal>(OnChangeHostsModel);
        }

        /// <summary>
        /// Подія, коли модель із данними хостів була оновлена
        /// </summary>
        private void OnChangeHostsModel(HostsPrivateModelSignal signalData )
        {
            // TODO добавити перевірку, якщо в кімнаті буде більше гравців, а ніж потрібно,
            // то що би гравців, котрі лишні, дісконектнуло із кімнати

            if (_isIgnore)
                return;

            if (_hostsService.GetActors(host.GameId).Count == _countActors)
            {
                plotStatesService.ChangeState(nextState);
            }
        }

        public void ExitState()
        {
            _isIgnore = true;
            _signalBus.Unsubscrible<HostsPrivateModelSignal>(OnChangeHostsModel);
        }
    }
}

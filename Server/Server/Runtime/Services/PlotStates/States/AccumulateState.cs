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
    public class AccumulateState : BasePlotState
    {
        public const string NAME = "AccumulateState";
        public override string Name => NAME;

        private SignalBus _signalBus;
        private HostsService _hostsService;

        /// <summary>
        /// Кількість гравців, котрі потрібні для старту ігрової кімнати
        /// </summary>
        private int _countActors;

        private PlotsModelService _plotsModelService;
        private object _accumulateTimer;
        private IPlotModelScheme _plotModelScheme;

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
            _plotsModelService = gameInstaller.plotsModelService;
        }

        public override void EnterState()
        {
            LogChannel.Log("PlotStatesService :: AccumulateState :: EnterState()", LogChannel.Type.Plot);
            
            _plotModelScheme = _plotsModelService.Get(host.GameId);
            _plotModelScheme.IsRoomVisible = true;

            _signalBus.Subscrible<HostsPrivateModelSignal>(OnChangeHostsModel);

            _accumulateTimer = host.CreateOneTimeTimer(OnTimerIsFinish, 1000);
        }

        private void OnTimerIsFinish()
        {
            if (!_plotModelScheme.IsRoomVisible)
                return;

            // Актор не знайшов собі противника. Підключити актора до бота
            _plotModelScheme.IsRoomVisible = false;
            _plotModelScheme.IsGameWithAI = true;
            
            plotStatesService.ChangeState(nextState);
        }

        /// <summary>
        /// Подія, коли модель із данними хостів була оновлена
        /// </summary>
        private void OnChangeHostsModel(HostsPrivateModelSignal signalData)
        {
            // TODO добавити перевірку, якщо в кімнаті буде більше гравців, а ніж потрібно,
            // то що би гравців, котрі лишні, дісконектнуло із кімнати

            if (!_plotModelScheme.IsRoomVisible)
                return;

            if (_hostsService.GetActors(host.GameId).Count == _countActors)
            {
                plotStatesService.ChangeState(nextState);
            }
        }

        public override void ExitState()
        {
            _plotModelScheme.IsRoomVisible = false;

            _signalBus.Unsubscrible<HostsPrivateModelSignal>(OnChangeHostsModel);

            host.StopTimer(_accumulateTimer);
            _accumulateTimer = null;
        }

        
    }
}

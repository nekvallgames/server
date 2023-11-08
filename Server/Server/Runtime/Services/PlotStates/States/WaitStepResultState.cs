using Photon.Hive.Plugin;
using Plugin.Installers;
using Plugin.Signals;
using Plugin.Tools;

namespace Plugin.Runtime.Services.PlotStates.States
{
    /// <summary>
    /// Состояние, в котором мы ждем, когда игроки пришлют свой шаг действия
    /// </summary>
    public class WaitStepResultState : BasePlotState
    {
        public const string NAME = "WaitStepResult";
        public override string Name => NAME;

        private SignalBus _signalBus;
        private OpStockService _opStockService;
        private PlotsModelService _plotsModelService;

        /// <summary>
        /// Кількість гравців, котрі потрібні для старту ігрової кімнати
        /// </summary>
        private int _countActors;
        private bool _isGameWithAI;

        public WaitStepResultState(PlotStatesService plotStatesService,
                                   IPluginHost host, 
                                   int countActors, 
                                   string nextState):base(plotStatesService, host, nextState)
        {
            _countActors = countActors;

            var gameInstaller = GameInstaller.GetInstance();

            _signalBus = gameInstaller.signalBus;
            _opStockService = gameInstaller.opStockService;
            _plotsModelService = gameInstaller.plotsModelService;
        }

        public override void EnterState()
        {
            LogChannel.Log("PlotService :: WaitStepResult :: EnterState()", LogChannel.Type.Plot);

            _isGameWithAI = _plotsModelService.Get(host.GameId).IsGameWithAI;

            // Слушаем сигнал о том, что клиент прислал операцию на GameServer
            _signalBus.Subscrible<OpStockPrivateModelSignal>(OpStockModelChanged);

            // TODO в майбутньому додати таймер. У гравців на виконання кроку є обмежений час
        }

        /// <summary>
        /// Модель із операціями гравця була змінена
        /// </summary>
        private void OpStockModelChanged(OpStockPrivateModelSignal signalData)
        {
            if (signalData.GameId != host.GameId)
                return;

            // ToDo я чекаю на любу операцію syncStep, а мені потрібно чекати із певним syncStep

            int opStepCount = _opStockService.GetOpCount(host.GameId, OperationCode.syncStep);

            bool result = opStepCount == _countActors;

            if (_isGameWithAI)
            {
                result = opStepCount > 0;
            }
                
            if (result)
            {
                // Оба игрока прислали свой ход. Можно не ждать окончание таймера, 
                // а перейти в следующее состояние
                plotStatesService.ChangeState(nextState);
            }
        }

        public override void ExitState()
        {
            _signalBus.Unsubscrible<OpStockPrivateModelSignal>(OpStockModelChanged);
        }
    }
}

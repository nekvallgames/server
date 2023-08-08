using Photon.Hive.Plugin;
using Plugin.Installers;
using Plugin.Interfaces;
using Plugin.Tools;

namespace Plugin.Runtime.Services.PlotStates.States
{
    /// <summary>
    /// 
    /// </summary>
    public class SyncStepsState : BasePlotState, IState
    {
        public const string NAME = "SyncStepsState";
        public string Name => NAME;

        private SyncStepService _syncStepService;
        private PlotsModelService _plotsModelService;

        public SyncStepsState(PlotStatesService plotStatesService,
                              IPluginHost host,
                              string nextStep) : base(plotStatesService, host, nextStep)
        {
            var gameInstaller = GameInstaller.GetInstance();

            _syncStepService = gameInstaller.syncStepService;
            _plotsModelService = gameInstaller.plotsModelService;
        }

        public void EnterState()
        {
            LogChannel.Log("PlotService :: SyncStepsState :: EnterState()", LogChannel.Type.Plot);

            IPlotModelScheme plotModel = _plotsModelService.Get(host.GameId);

            // Відправити клієнтам результат кроків
            _syncStepService.Sync(host, new int[] { plotModel.SyncStep - 2, plotModel.SyncStep - 1 });

            plotStatesService.ChangeState(nextState);
        }

        public void ExitState()
        {
            
        }
    }
}

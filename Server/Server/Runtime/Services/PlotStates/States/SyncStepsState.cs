using Photon.Hive.Plugin;
using Plugin.Installers;
using Plugin.Interfaces;
using Plugin.Tools;

namespace Plugin.Runtime.Services.PlotStates.States
{
    /// <summary>
    /// 
    /// </summary>
    public class SyncStepsState : BasePlotState
    {
        public const string NAME = "SyncStepsState";
        public override string Name => NAME;

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

        public override void EnterState()
        {
            LogChannel.Log("PlotService :: SyncStepsState :: EnterState()", LogChannel.Type.Plot);

            IPlotModelScheme plotModel = _plotsModelService.Get(host.GameId);

            if (plotModel.IsAbort)
            {
                return; // гра була призупинена! Можливо один із гравців покинув гру...
            }

            // Відправити клієнтам результат кроків
            _syncStepService.Sync(host, new int[] { plotModel.SyncStep - 2, plotModel.SyncStep - 1 });

            if (plotModel.IsGameFinished)
            {
                // game is finish
            }
            else 
            {
                plotStatesService.ChangeState(nextState);
            }
        }
    }
}

using Photon.Hive.Plugin;
using Plugin.Installers;
using Plugin.Interfaces;
using Plugin.Runtime.Services.PlotMode;
using Plugin.Runtime.Services.PlotMode.States.PVP;
using Plugin.Schemes;

namespace Plugin.Runtime.Services.PlotStates.States
{
    /// <summary>
    /// Стейт, в котрому ми перепровіряємо хто виграв, хто ні в PVP
    /// Зміна мода та таке інше
    /// </summary>
    public class PVPResultState : BasePlotState, IState
    {
        public const string NAME = "PVPResultState";
        public string Name => NAME;

        private PlotsModelService _plotsModelService;
        private UnitsService _unitsService;
        private ActorService _actorService;
        private PVPPlotModelScheme _plotModelScheme;
        private PlotModeService _plotModeService;
        private PlotPublicService _plotPublicService;

        public PVPResultState(PlotStatesService plotStatesService,
                              IPluginHost host,
                              string nextState) : base(plotStatesService, host, nextState)
        {
            var gameInstaller = GameInstaller.GetInstance();

            _plotsModelService = gameInstaller.plotsModelService;
            _unitsService = gameInstaller.unitsService;
            _actorService = gameInstaller.actorService;
            _plotPublicService = gameInstaller.plotPublicService;
            _plotModelScheme = (PVPPlotModelScheme)_plotsModelService.Get(host.GameId);

            _plotModeService = new PlotModeService(new ITask[] {
                                                   new PVPFightToFirstDeadMode(_plotModeService, host, _plotModelScheme, _actorService, _unitsService),
                                                   new PVPFightWithVipMode(_plotModeService, host, _plotModelScheme, _actorService, _unitsService, _plotPublicService),
                                                   new PVPDuelMode(_plotModeService, host, _plotModelScheme, _actorService, _unitsService),
                                                   new ResultMode(_plotModeService, host, _plotModelScheme)
            });
        }

        public void EnterState()
        {
            _plotModeService.ExecuteTask(_plotModelScheme.GameMode, () => 
            {
                // success
                plotStatesService.ChangeState(nextState);
            }, () => 
            {
                // fail
            });    
        }

        public void ExitState()
        {
            
        }
    }
}

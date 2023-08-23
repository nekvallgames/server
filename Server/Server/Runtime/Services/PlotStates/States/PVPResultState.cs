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
    public class PVPResultState : BasePlotState
    {
        public const string NAME = "PVPResultState";
        public override string Name => NAME;

        private PlotsModelService _plotsModelService;
        private UnitsService _unitsService;
        private ActorService _actorService;
        private PVPPlotModelScheme _plotModelScheme;
        private PlotModeService _plotModeService;

        public PVPResultState(PlotStatesService plotStatesService,
                              IPluginHost host,
                              string nextState) : base(plotStatesService, host, nextState)
        {
            var gameInstaller = GameInstaller.GetInstance();

            _plotsModelService = gameInstaller.plotsModelService;
            _unitsService = gameInstaller.unitsService;
            _actorService = gameInstaller.actorService;
        }

        public override void Initialize()
        {
            _plotModelScheme = (PVPPlotModelScheme)_plotsModelService.Get(host.GameId);

            _plotModeService = new PlotModeService();
            _plotModeService.Add(new IMode[] {
                                 new FightToFirstDeadMode(_plotModeService, host, _plotModelScheme, _actorService, _unitsService),
                                 new FightWithVipMode(_plotModeService, host, _plotModelScheme, _actorService, _unitsService),
                                 new DuelMode(_plotModeService, host, _plotModelScheme, _actorService, _unitsService),
                                 new ResultMode(host, _plotModelScheme, _actorService, _unitsService)
            });
        }

        public override void EnterState()
        {
            _plotModeService.ExecuteMode(_plotModelScheme.GameMode, () => 
            {
                // success
                plotStatesService.ChangeState(nextState);
            });    
        }
    }
}

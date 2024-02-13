using Photon.Hive.Plugin;
using Plugin.Installers;
using Plugin.Interfaces;
using Plugin.Tools;
using System;
using System.Collections.Generic;

namespace Plugin.Runtime.Services.PlotStates.States
{
    public class SyncUnitSkills : BasePlotState
    {
        public const string NAME = "SyncUnitSkills";
        public override string Name => NAME;

        private PlotsModelService _plotsModelService;
        private ActorService _actorService;
        private UnitsService _unitsService;

        public SyncUnitSkills(PlotStatesService plotStatesService,
                              IPluginHost host,
                              string nextStep) : base(plotStatesService, host, nextStep)
        {
            var gameInstaller = GameInstaller.GetInstance();

            _plotsModelService = gameInstaller.plotsModelService;
            _actorService = gameInstaller.actorService;
            _unitsService = gameInstaller.unitsService;
        }

        public override void EnterState()
        {
            LogChannel.Log("PlotService :: SyncUnitSkills :: EnterState()", LogChannel.Type.Plot);

            IPlotModelScheme plotModel = _plotsModelService.Get(host.GameId);

            if (plotModel.SyncStep != 0)
            {
                // TODO переписать эту шляпу в какой то более професиональный вид =)
                SetPoisonDamage();
            }

            plotStatesService.ChangeState(nextState);
        }

        private void SetPoisonDamage()
        {
            List<IActorScheme> actrors = _actorService.GetActorsInRoom(host.GameId);

            foreach (IActorScheme actor in actrors)
            {
                _unitsService.SetPoisonDamage(host.GameId, actor.ActorNr);
            }
        }
    }
}

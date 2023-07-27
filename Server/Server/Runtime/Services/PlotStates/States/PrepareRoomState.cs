using Photon.Hive.Plugin;
using Plugin.Installers;
using Plugin.Interfaces;
using Plugin.Tools;
using System.Collections.Generic;

namespace Plugin.Runtime.Services.PlotStates.States
{
    /// <summary>
    /// Стейт, в котрому створимо юнітів та ігрову сітку для гравців в ігровій кімнаті
    /// </summary>
    public class PrepareRoomState : BasePlotState, IState
    {
        public const string NAME = "PrepareRoomState";
        public string Name => NAME;

        private ActorService _actorService;
        private UnitsService _unitsService;
        private GridService _gridService;

        public PrepareRoomState(PlotStatesService plotStatesService,
                                IPluginHost host,
                                string nextState) : base(plotStatesService, host, nextState)
        {
            var gameInstaller = GameInstaller.GetInstance();
            _actorService = gameInstaller.actorService;
            _unitsService = gameInstaller.unitsService;
            _gridService = gameInstaller.gridService;
        }

        public void EnterState()
        {
            LogChannel.Log("PlotStatesService :: CreateUnitsState :: EnterState()", LogChannel.Type.Plot);

            List<IActorScheme> actors = _actorService.GetActorsInRoom(host.GameId);

            CreateUnits(ref actors);
            CreateGrids(ref actors);

            plotStatesService.ChangeState(nextState);
        }
        private void CreateUnits(ref List<IActorScheme> actors)
        {
            foreach (IActorScheme actor in actors)
            {
                _unitsService.RemoveAllIfExist(host.GameId, actor.ActorNr);

                foreach (int unitId in actor.Deck)
                {
                    if (unitId == -1)
                        continue;

                    _unitsService.CreateUnit(host.GameId, actor.ActorNr, unitId);
                }
            }
        }

        private void CreateGrids(ref List<IActorScheme> actors)
        {
            foreach (IActorScheme actor in actors)
            {
                if (_gridService.IsExist(host.GameId, actor.ActorNr)){
                    _gridService.Remove(host.GameId, actor.ActorNr);
                }

                _gridService.Create(host.GameId, actor.ActorNr);
            }
        }

        public void ExitState()
        {
            
        }
    }
}

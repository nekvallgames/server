using Photon.Hive.Plugin;
using Plugin.Installers;
using Plugin.Interfaces;
using Plugin.Runtime.Services.UnitsPath;
using Plugin.Tools;
using System.Collections.Generic;

namespace Plugin.Runtime.Services.PlotStates.States
{
    /// <summary>
    /// Стейт, в котрому створимо юнітів та ігрову сітку для гравців в ігровій кімнаті
    /// </summary>
    public class PrepareRoomState : BasePlotState
    {
        public const string NAME = "PrepareRoomState";
        public override string Name => NAME;

        private ActorService _actorService;
        private UnitsService _unitsService;
        private GridService _gridService;
        private UnitsPathService _unitsPathService;
        private CellWalkableService _cellWalkableService;

        public PrepareRoomState(PlotStatesService plotStatesService,
                                IPluginHost host,
                                string nextState) : base(plotStatesService, host, nextState)
        {
            var gameInstaller = GameInstaller.GetInstance();
            _actorService = gameInstaller.actorService;
            _unitsService = gameInstaller.unitsService;
            _gridService = gameInstaller.gridService;
            _unitsPathService = gameInstaller.unitsPathService;
            _cellWalkableService = gameInstaller.cellWalkableService;
        }

        public override void EnterState()
        {
            LogChannel.Log("PlotStatesService :: CreateUnitsState :: EnterState()", LogChannel.Type.Plot);

            List<IActorScheme> actors = _actorService.GetActorsInRoom(host.GameId);

            foreach (IActorScheme actor in actors)
            {
                _unitsPathService.CreateScheme(host.GameId, actor.ActorNr);
                _cellWalkableService.CreateScheme(host.GameId, actor.ActorNr);
            }

            CreateUnits(ref actors);
            CreateGrids(ref actors);

            plotStatesService.ChangeState(nextState);
        }

        private void CreateUnits(ref List<IActorScheme> actors)
        {
            foreach (IActorScheme actor in actors)
            {
                _unitsService.RemoveAllIfExist(host.GameId, actor.ActorNr);

                for (int i = 0; i < actor.Deck.Count; i++)
                {
                    int unitId = actor.Deck[i];

                    _unitsService.CreateUnit(host.GameId, actor.ActorNr, unitId, actor.Levels[i]);
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
    }
}

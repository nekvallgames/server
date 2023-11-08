using Photon.Hive.Plugin;
using Plugin.Installers;
using Plugin.Interfaces;
using Plugin.Runtime.Services.AI;
using Plugin.Runtime.Services.UnitsPath;
using Plugin.Schemes;
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
        private PlotsModelService _plotsModelService;
        private AIService _aiService;
        private ActorStepsService _actorStepsService;
        private int _countActors;

        public PrepareRoomState(PlotStatesService plotStatesService,
                                IPluginHost host,
                                int countActors,
                                string nextState) : base(plotStatesService, host, nextState)
        {
            var gameInstaller = GameInstaller.GetInstance();
            _actorService = gameInstaller.actorService;
            _unitsService = gameInstaller.unitsService;
            _gridService = gameInstaller.gridService;
            _unitsPathService = gameInstaller.unitsPathService;
            _cellWalkableService = gameInstaller.cellWalkableService;
            _plotsModelService = gameInstaller.plotsModelService;
            _aiService = gameInstaller.aiService;
            _actorStepsService = gameInstaller.actorStepsService;

            _countActors = countActors;
        }

        public override void EnterState()
        {
            LogChannel.Log("PlotStatesService :: CreateUnitsState :: EnterState()", LogChannel.Type.Plot);

            IPlotModelScheme model = _plotsModelService.Get(host.GameId);

            List<IActorScheme> actors = _actorService.GetActorsInRoom(host.GameId);

            if (model.IsGameWithAI){
                AddUpActorsToRoom(ref actors);
            }
                
            foreach (IActorScheme actor in actors)
            {
                _unitsPathService.CreateScheme(host.GameId, actor.ActorNr);
                _cellWalkableService.CreateScheme(host.GameId, actor.ActorNr);
                _actorStepsService.CreateScheme(host.GameId, actor.ActorNr);
            }

            CreateUnits(ref actors);
            CreateGrids(ref actors);

            if (model.IsGameWithAI)
            {
                int aiActorNr = _actorService.GetAiActor(host.GameId).ActorNr;
                List<IUnit> aiUnits = new List<IUnit>();
                _unitsService.GetAliveUnits(host.GameId, aiActorNr, ref aiUnits);

                IGrid grid = _gridService.Get(host.GameId, aiActorNr);

                foreach (IUnit aiUnit in aiUnits)
                {
                    aiUnit.Position = _gridService.GetFreePosition(grid, aiUnit);
                    _gridService.BusyArea(aiUnit, grid, aiUnit.Position.x, aiUnit.Position.y);
                }
            }

            plotStatesService.ChangeState(nextState);
        }

        /// <summary>
        /// Якщо гравець грає із ботами, то насипати йому в ігрову кімнату ботів
        /// </summary>
        /// <param name="actors"></param>
        private void AddUpActorsToRoom(ref List<IActorScheme> actors)
        {
            IActorScheme realActor = actors[0];

            // наповнити ігрову кімнату ботами
            while (actors.Count < _countActors)
            {
                ActorScheme actorScheme = _aiService.CreateAIActor(host.GameId);

                // ToDo
                actorScheme.Deck = realActor.Deck;
                actorScheme.Levels = realActor.Levels;

                actors.Add(actorScheme);
            }
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

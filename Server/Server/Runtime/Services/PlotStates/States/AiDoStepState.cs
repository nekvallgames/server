using Photon.Hive.Plugin;
using Plugin.Installers;
using Plugin.Interfaces;
using Plugin.Runtime.Services.AI;
using Plugin.Runtime.Services.AI.Tasks;
using Plugin.Tools;
using System.Collections.Generic;

namespace Plugin.Runtime.Services.PlotStates.States
{
    /// <summary>
    /// Якщо гра із AI, то в поточному стейті AI зробить свій крок
    /// </summary>
    public class AiDoStepState : BasePlotState
    {
        public const string NAME = "AiDoStepState";
        public override string Name => NAME;

        private PlotsModelService _plotsModelService;
        private PathService _pathService;
        private ActorService _actorService;
        private AIService _aiService;
        private UnitsService _unitsService;
        private CellTemperatureTraceService _cellTemperatureTraceService;

        public AiDoStepState(PlotStatesService plotStatesService,
                             IPluginHost host,
                             string nextState) : base(plotStatesService, host, nextState)
        {
            var gameInstaller = GameInstaller.GetInstance();

            _plotsModelService = gameInstaller.plotsModelService;
            _pathService = gameInstaller.pathService;
            _actorService = gameInstaller.actorService;
            _aiService = gameInstaller.aiService;
            _unitsService = gameInstaller.unitsService;
            _cellTemperatureTraceService = gameInstaller.cellTemperatureTraceService;
        }

        public override void EnterState()
        {
            LogChannel.Log("PlotStatesService :: AiDoStepState :: EnterState()", LogChannel.Type.Plot);

            IPlotModelScheme plotModel = _plotsModelService.Get(host.GameId);

            if (!plotModel.IsGameWithAI){
                plotStatesService.ChangeState(nextState);
                return;
            }

            // потрібно всі кроки покласти в один stepScheme

            // Виконати перший крок всих гравців в ігровій кімнаті - move
            IActorScheme aiActor = _actorService.GetAiActor(host.GameId);
            IActorScheme realActor = _actorService.GetRealActor(host.GameId);

            if (plotModel.SyncStep == 0)
            {
                // Це самий перший крок, і гравець ще не прислав свої позиції, де стоять його юніти
                // Перший раз просто рандомно виставляємо юнітів реального гравця

                // Вирахувати шлях, куди можуть переміститься юніти реального гравця
                
               
                var realUnits = new List<IUnit>();
                _unitsService.GetAliveUnits(host.GameId, realActor.ActorNr, ref realUnits);
                foreach (IUnit unit in realUnits)
                {
                    unit.Position = new Int2(1,1);
                }

                // Це самий перший крок, просто потрібно добавити позиції юнів в syncService
                _aiService.ExecuteTask(host.GameId, aiActor.ActorNr, plotModel.SyncStep, AIPositionTask.TASK_NAME);
            }
            else
            {
                List<IActorScheme> actors = _actorService.GetActorsInRoom(host.GameId);
                foreach (IActorScheme actor in actors)
                {
                    // Высчитать путь, куда может переместится каждый юнит игрока
                    if (plotModel.IsNeedToCheckOnCorrectPosition)
                    {
                        _pathService.Calculate(host.GameId, actor.ActorNr);
                    }
                }

                // Так як локальний гравець грає проти AI, для AI потрібно отсліжувати
                // температурний слід юнітів локального гравця

                int aiAliveUnitsCount = _unitsService.GetAliveUnitsCount(host.GameId, aiActor.ActorNr);

                _cellTemperatureTraceService.UpdateTemperatureTrace(host.GameId, realActor.ActorNr);

                var tasks = new List<string>
                {
                    AIHealingTask.TASK_NAME,
                    aiAliveUnitsCount > 1 ? AIMoveTask.TASK_NAME : AIRandomMoveTask.TASK_NAME
                };

                if (plotModel.GameMode == (int)Enums.PVPMode.FightWithVip)
                {
                    tasks.Add(AIVipTask.TASK_NAME);
                }

                _aiService.ExecuteTasks(host.GameId, aiActor.ActorNr, plotModel.SyncStep, tasks);
            }

            // перед атакою потрібно юнітів противника виставити в якісь точки.
            // Бо зараз юніти противника стоять в (0,0). Потрібно виставити в рандомні міста

            // виконати другий крок
            _aiService.ExecuteTasks(host.GameId, aiActor.ActorNr, plotModel.SyncStep+1, new List<string> { AIActionTask.TASK_NAME });

            plotStatesService.ChangeState(nextState);
        }
    }
}

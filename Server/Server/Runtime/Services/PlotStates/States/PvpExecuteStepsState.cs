using Photon.Hive.Plugin;
using Plugin.Installers;
using Plugin.Interfaces;
using Plugin.Interfaces.Actions;
using Plugin.Interfaces.UnitComponents;
using Plugin.Runtime.Services.AI;
using Plugin.Runtime.Services.AI.Tasks;
using Plugin.Runtime.Services.ExecuteOp;
using Plugin.Schemes;
using Plugin.Tools;
using System.Collections.Generic;

namespace Plugin.Runtime.Services.PlotStates.States
{
    /// <summary>
    /// Состояние, при котором мы обрабатываем результат шага игроков
    /// Игроки переставили своих юнитов и атаковали друг друга
    /// 
    /// Нужно сначала синхронизировать первый шаг, где игрок розставляет своих юнитов на игровой сетке
    /// И после выполнить второй шаг - где игрок уже атаковал противника
    /// </summary>
    public class PvpExecuteStepsState : BasePlotState
    {
        public const string NAME = "PvpExecuteStepsState";
        public override string Name => NAME;

        private UnitsService _unitsService;
        private ConvertService _convertService;
        private OpStockService _opStockService;
        private ExecuteOpStepSchemeService _executeOpStepService;
        private PlotsModelService _plotsModelService;
        private PathService _pathService;
        private AIService _aiService;
        private ActorService _actorService;
        private ActorStepsService _actorStepsService;
        private DeserializeStepService _deserializeStepService;

        public PvpExecuteStepsState(PlotStatesService plotStatesService,
                                    IPluginHost host, 
                                    string nextStep):base(plotStatesService, host, nextStep)
        {
            var gameInstaller = GameInstaller.GetInstance();

            _unitsService = gameInstaller.unitsService;
            _convertService = gameInstaller.convertService;
            _opStockService = gameInstaller.opStockService;
            _executeOpStepService = gameInstaller.executeOpStepService;
            _plotsModelService = gameInstaller.plotsModelService;
            _pathService = gameInstaller.pathService;
            _aiService = gameInstaller.aiService;
            _actorService = gameInstaller.actorService;
            _deserializeStepService = gameInstaller.deserializeStepService;
            _actorStepsService = gameInstaller.actorStepsService;
        }

        public override void EnterState()
        {
            LogChannel.Log("PlotService :: PvpExecuteStepsState :: EnterState()", LogChannel.Type.Plot);

            PvpPlotModelScheme plotModel = _plotsModelService.Get(host.GameId) as PvpPlotModelScheme;

            List<IActorScheme> actors = _actorService.GetActorsInRoom(host.GameId);

            foreach (IActorScheme actor in actors)
            {
                _deserializeStepService.DeserializeOp(host.GameId, actor.ActorNr);

                _unitsService.ReviveAction(host.GameId, actor.ActorNr);  // Всiм юнітам перезарядити їхні єкшени

                // Высчитать путь, куда может переместится каждый юнит игрока
                if (plotModel.IsNeedToCheckOnCorrectPosition){
                    _pathService.Calculate(host.GameId, actor.ActorNr);
                }
            }

            // Виконати перший крок всих гравців в ігровій кімнаті - move
            if (plotModel.IsGameWithAI)
            {
                IActorScheme aiActor = _actorService.GetAiActor(host.GameId);

                if (plotModel.SyncStep == 0)
                {
                    // Це самий перший крок, просто потрібно добавити позиції юнів в syncService
                    _aiService.ExecuteTask(host.GameId, aiActor.ActorNr, AIPositionTask.TASK_NAME);
                }
                else
                {
                    // Так як локальний гравець грає проти AI, для AI потрібно отсліжувати
                    // температурний слід юнітів локального гравця
                    
                    int aiAliveUnitsCount = _unitsService.GetAliveUnitsCount(host.GameId, aiActor.ActorNr);

                    // _temperatureWalkableTraceService.UpdateTemperatureTrace(targetActorId); TODO

                    var tasks = new List<string>
                    {
                        AIHealingTask.TASK_NAME,
                        aiAliveUnitsCount > 1 ? AIMoveTask.TASK_NAME : AIRandomMoveTask.TASK_NAME
                    };

                    if (plotModel.GameMode == (int)Enums.PVPMode.FightWithVip)
                    {
                        tasks.Add(AIVipTask.TASK_NAME);
                    }

                    _aiService.ExecuteTasks(host.GameId, aiActor.ActorNr, tasks);
                }
            }

            foreach (IActorScheme actor in actors)
            {
                _executeOpStepService.Execute(host.GameId, 
                                              actor.ActorNr, 
                                              plotModel.SyncStep, 
                                              _actorStepsService.Get(host.GameId, actor.ActorNr).stepScheme);
            }
            plotModel.SyncStep++;

            if (plotModel.IsGameWithAI)
            {
                IActorScheme aiActor = _actorService.GetAiActor(host.GameId);
                _aiService.ExecuteTasks(host.GameId, aiActor.ActorNr, new List<string> { AIActionTask.TASK_NAME });
            }

            // Виконати другий крок всих гравців в ігровій кімнаті - attack
            foreach (IActorScheme actor in actors)
            {
                _executeOpStepService.Execute(host.GameId,
                                              actor.ActorNr,
                                              plotModel.SyncStep,
                                              _actorStepsService.Get(host.GameId, actor.ActorNr).stepScheme);
            }
            // ExecuteSteps(host.GameId, ref actorSteps, plotModel.SyncStep);
            plotModel.SyncStep++;

            DebugStatus();

            plotStatesService.ChangeState(nextState);
        }

        private void DebugStatus()
        {
            List<IActorScheme> actors = _actorService.GetActorsInRoom(host.GameId);

            foreach (IActorScheme actor in actors)
            {
                List<IUnit> units = _unitsService.GetUnits(host.GameId, actor.ActorNr);

                LogChannel.Log("-- actorNr = " + actor.ActorNr + " ---------------------------");

                for (int i = 0; i < units.Count; i++)
                {
                    IUnit unit = units[i];
                    if (/*!(unit is IBarrierComponent) && */!unit.IsDead)
                    {
                        // LogChannel.Log($"[{i}] uId = {unit.UnitId}, h = {(unit as IHealthComponent).HealthCapacity}/{(unit as IHealthComponent).HealthCapacityMax}, c = {(unit as IDamageAction).ActionCapacity}", LogChannel.Type.Plot);
                        LogChannel.Log($"[{i}] uId = {unit.UnitId}, posW = {unit.Position.x}, powH = {unit.Position.y}, h = {(unit as IHealthComponent).HealthCapacity}/{(unit as IHealthComponent).HealthCapacityMax}", LogChannel.Type.Plot);
                    }
                }
            }

            
        }
               
        // private void ExecuteSteps(string gameId, ref List<ActorStep> actorSteps, int syncStep)
        // {
        //     foreach (ActorStep actorStep in actorSteps){
        //         _executeOpStepService.Execute(gameId, actorStep.actorId, syncStep, actorStep.stepScheme);
        //     }
        // }
    }

    /// <summary>
    /// Класс, который будет хранить в себе данные с результатами действий игрока
    /// </summary>
    /*internal struct ActorStep
    {
        /// <summary>
        /// ID игрока
        /// </summary>
        public int actorId;

        /// <summary>
        /// Схема со всеми действиями игрока
        /// Куча компонентов, которые разсортированы по спискам
        /// </summary>
        public StepScheme stepScheme;

        public ActorStep(int actorId, StepScheme stepScheme)
        {
            this.actorId = actorId;
            this.stepScheme = stepScheme;
        }
    }*/
}

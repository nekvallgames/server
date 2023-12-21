using Photon.Hive.Plugin;
using Plugin.Installers;
using Plugin.Interfaces;
using Plugin.Interfaces.UnitComponents;
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
        private ExecuteOpStepSchemeService _executeOpStepService;
        private PlotsModelService _plotsModelService;
        private ActorService _actorService;
        private ActorStepsService _actorStepsService;
        private DeserializeStepService _deserializeStepService;

        public PvpExecuteStepsState(PlotStatesService plotStatesService,
                                    IPluginHost host, 
                                    string nextStep):base(plotStatesService, host, nextStep)
        {
            var gameInstaller = GameInstaller.GetInstance();

            _unitsService = gameInstaller.unitsService;
            _executeOpStepService = gameInstaller.executeOpStepService;
            _plotsModelService = gameInstaller.plotsModelService;
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
            }

            foreach (IActorScheme actor in actors)
            {
                List<StepScheme> sss = _actorStepsService.Get(host.GameId, actor.ActorNr).steps;

                _executeOpStepService.Execute(host.GameId, 
                                              actor.ActorNr, 
                                              plotModel.SyncStep,
                                              sss[0]);
            }
            plotModel.SyncStep++;

            // Виконати другий крок всих гравців в ігровій кімнаті - attack
            foreach (IActorScheme actor in actors)
            {
                // _executeOpStepService.Execute(host.GameId,
                //                               actor.ActorNr,
                //                               plotModel.SyncStep,
                //                               _actorStepsService.Get(host.GameId, actor.ActorNr).stepScheme);
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
    }
}

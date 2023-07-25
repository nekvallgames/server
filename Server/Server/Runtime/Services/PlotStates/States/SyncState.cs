using Photon.Hive.Plugin;
using Plugin.Installers;
using Plugin.Interfaces;
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
    public class SyncState : BasePlotState, IState
    {
        public const string NAME = "SyncState";
        public string Name => NAME;

        private UnitsService _unitsService;
        private ConvertService _convertService;
        private OpStockService _opStockService;
        private ExecuteOpStepSchemeService _executeOpStepService;
        private SyncStepService _syncStepService;
        private HostsService _hostsService;
        private PlotsModelService _plotsModelService;

        private string _nextStep;

        public SyncState(PlotStatesService plotStatesService,
                         IPluginHost host, 
                         string nextStep):base(plotStatesService, host)
        {
            _nextStep = nextStep;

            var gameInstaller = GameInstaller.GetInstance();

            _unitsService = gameInstaller.unitsService;
            _convertService = gameInstaller.convertService;
            _opStockService = gameInstaller.opStockService;
            _executeOpStepService = gameInstaller.executeOpStepService;
            _syncStepService = gameInstaller.syncStepService;
            _hostsService = gameInstaller.hostsService;
            _plotsModelService = gameInstaller.plotsModelService;
        }

        public void EnterState()
        {
            LogChannel.Log("PlotService :: PVPStepResult :: EnterState()", LogChannel.Type.Plot);

            var plotModels = new List<IPlotModelScheme>();

            foreach (IActor actor in _hostsService.GetActors(host.GameId))
            {
                _unitsService.ReviveAction(host.GameId, actor.ActorNr);  // Всiм юнітам перезарядити їхні єкшени
                plotModels.Add(_plotsModelService.Get(host.GameId/*, actor.ActorNr*/));  // отримати модель даних ігрового режиму
            }

            // Десериализировать операцію StepScheme акторів, котрі вони прислали 
            var actorSteps = new List<ActorStep>();
            DeserializeOp(ref actorSteps);

            // Виконати перший крок - move
            ExecuteSteps(host.GameId, ref actorSteps, plotModels[0].SyncStep);
            IncreaseSyncStep();

            // Виконати другий крок - attack
            ExecuteSteps(host.GameId, ref actorSteps, plotModels[0].SyncStep);
            IncreaseSyncStep();


            _syncStepService.Sync(host, new int[] { plotModels[0].SyncStep - 2, plotModels[0].SyncStep - 1 });

            plotStatesService.ChangeState(_nextStep);



            void IncreaseSyncStep()
            {
                foreach (IPlotModelScheme plotModel in plotModels){
                    plotModel.SyncStep++;
                }
            }
        }

        /// <summary>
        /// Нужно десериализировать операции, которые прислал игрок на Game Server
        /// </summary>
        private void DeserializeOp(ref List<ActorStep> actorSteps)
        {
            foreach (IActor actor in _hostsService.GetActors(host.GameId))
            {
                if (!_opStockService.HasOp(host.GameId, actor.ActorNr, OperationCode.syncStep))
                    continue;

                var stepData = _opStockService.TakeOp(host.GameId, actor.ActorNr, OperationCode.syncStep);

                var stepScheme = _convertService.DeserializeObject<StepScheme>(stepData.Data.ToString());

                actorSteps.Add(new ActorStep(actor.ActorNr, stepScheme));
            }
        }

        private void ExecuteSteps(string gameId, ref List<ActorStep> actorSteps, int syncStep)
        {
            foreach (ActorStep actorStep in actorSteps){
                _executeOpStepService.Execute(gameId, actorStep.actorId, syncStep, actorStep.stepScheme);
            }
        }

        public void ExitState()
        {
            
        }
    }

    /// <summary>
    /// Класс, который будет хранить в себе данные с результатами действий игрока
    /// </summary>
    internal struct ActorStep
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
    }
}

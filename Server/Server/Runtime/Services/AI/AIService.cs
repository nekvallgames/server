using Plugin.Interfaces;
using Plugin.Runtime.Services.AI.Tasks;
using Plugin.Schemes;
using System.Collections.Generic;

namespace Plugin.Runtime.Services.AI
{
    /// <summary>
    /// Сервіс, за допомогою котрого будемо керувати роботою AI
    /// </summary>
    public class AIService
    {
        private ActorService _actorService;
        private ActorStepsService _actorStepsService;

        private Dictionary<string, IAiTask> _aiTasks;

        public AIService(ActorService actorService,
                         UnitsService unitsService,
                         IWeaponActionDecisionService weaponActionDecisionService,
                         IPoisonActionDecisionService poisonActionDecisionService,
                         PathService pathService,
                         HealingDecisionService healingDecisionService,
                         CellWalkableService cellWalkableService,
                         DestinationDecisionService destinationDecisionService,
                         VipDecisionService vipDecisionService,
                         SimulateNotificationChangeVipService simulateNotificationChangeVipService,
                         SyncRoomService syncRoomService,
                         ActorStepsService actorStepsService,
                         SimulateSyncActionService simulateSyncActionService)
        {
            _actorService = actorService;
            _actorStepsService = actorStepsService;

            _aiTasks = new Dictionary<string, IAiTask> {
                {AIActionTask.TASK_NAME, new AIActionTask(unitsService, 
                                                          weaponActionDecisionService, 
                                                          poisonActionDecisionService, 
                                                          pathService, 
                                                          actorService,
                                                          simulateSyncActionService)},
                {AIHealingTask.TASK_NAME, new AIHealingTask(unitsService, 
                                                            healingDecisionService, 
                                                            actorService)},
                {AIMoveTask.TASK_NAME, new AIMoveTask(unitsService, 
                                                      cellWalkableService, 
                                                      pathService, 
                                                      destinationDecisionService, 
                                                      actorService, 
                                                      syncRoomService)},
                {AIRandomMoveTask.TASK_NAME, new AIRandomMoveTask(unitsService, 
                                                                  cellWalkableService, 
                                                                  pathService, 
                                                                  destinationDecisionService, 
                                                                  actorService,
                                                                  syncRoomService)},
                {AIPositionTask.TASK_NAME, new AIPositionTask(unitsService, 
                                                              actorService, 
                                                              syncRoomService)},
                {AIVipTask.TASK_NAME, new AIVipTask(vipDecisionService, 
                                                    unitsService, 
                                                    simulateNotificationChangeVipService, 
                                                    actorService)}
            };
        }

        public ActorScheme CreateAIActor(string gameId)
        {
            List<IActorScheme> actors = _actorService.GetActorsInRoom(gameId);
            actors.Sort((x, y) => x.ActorNr.CompareTo(y.ActorNr));

            return _actorService.CreateActor(gameId, actors[0].ActorNr + 1, string.Empty, true);
        }

        public void ExecuteTask(string gameId, int actorNr, string taskName)
        {
            _actorStepsService.AddStepScheme(gameId, actorNr, new StepScheme());

            ExecuteTask(taskName, gameId);
        }

        public void ExecuteTasks(string gameId, int actorNr, List<string> taskNames)
        {
            _actorStepsService.AddStepScheme(gameId, actorNr, new StepScheme());

            foreach (string taskName in taskNames)
            {
                ExecuteTask(taskName, gameId);
            }
        }

        private void ExecuteTask(string taskName, string gameId)
        {
            _aiTasks[taskName].ExecuteTask(gameId);
        }
    }
}

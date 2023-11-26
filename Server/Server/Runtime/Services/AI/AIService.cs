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
                         SimulateSyncActionService simulateSyncActionService)
        {
            _actorService = actorService;

            _aiTasks = new Dictionary<string, IAiTask> {
                {AIActionTask.TASK_NAME, new AIActionTask(unitsService, 
                                                          weaponActionDecisionService, 
                                                          poisonActionDecisionService, 
                                                          pathService, 
                                                          actorService,
                                                          simulateSyncActionService)},
                {AIHealingTask.TASK_NAME, new AIHealingTask(unitsService, 
                                                            healingDecisionService, 
                                                            syncRoomService)},
                {AIMoveTask.TASK_NAME, new AIMoveTask(unitsService, 
                                                      cellWalkableService, 
                                                      pathService, 
                                                      destinationDecisionService, 
                                                      syncRoomService)},
                {AIRandomMoveTask.TASK_NAME, new AIRandomMoveTask(unitsService, 
                                                                  cellWalkableService, 
                                                                  pathService, 
                                                                  destinationDecisionService, 
                                                                  actorService,
                                                                  syncRoomService)},
                {AIPositionTask.TASK_NAME, new AIPositionTask(unitsService,  
                                                              syncRoomService)},
                {AIVipTask.TASK_NAME, new AIVipTask(vipDecisionService, 
                                                    unitsService, 
                                                    simulateNotificationChangeVipService, 
                                                    actorService,
                                                    syncRoomService)}
            };
        }

        public ActorScheme CreateAIActor(string gameId)
        {
            List<IActorScheme> actors = _actorService.GetActorsInRoom(gameId);
            actors.Sort((x, y) => x.ActorNr.CompareTo(y.ActorNr));

            return _actorService.CreateActor(gameId, actors[0].ActorNr + 1, string.Empty, true);
        }

        public void ExecuteTask(string gameId, int actorNr, int stepNumber, string taskName)
        {
            ExecuteTask(taskName, gameId, actorNr, stepNumber);
        }

        public void ExecuteTasks(string gameId, int actorNr, int stepNumber, List<string> taskNames)
        {
            foreach (string taskName in taskNames)
            {
                ExecuteTask(taskName, gameId, actorNr, stepNumber);
            }
        }

        private void ExecuteTask(string taskName, string gameId, int actorNr, int stepNumber)
        {
            _aiTasks[taskName].ExecuteTask(gameId, actorNr, stepNumber);
        }
    }
}

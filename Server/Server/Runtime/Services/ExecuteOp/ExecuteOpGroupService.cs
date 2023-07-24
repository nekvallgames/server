using Plugin.Interfaces;
using Plugin.Runtime.Services.ExecuteAction;
using Plugin.Runtime.Services.ExecuteAction.Action;
using Plugin.Runtime.Services.ExecuteAction.Additional;
using Plugin.Runtime.Services.ExecuteOp.Executors;
using System.Collections.Generic;

namespace Plugin.Runtime.Services.ExecuteOp
{
    public class ExecuteOpGroupService
    {
        /// <summary>
        /// Список выполнителей действий игрока
        /// </summary>
        private IExecuteOpGroup[] _executors;

        public ExecuteOpGroupService(UnitsService unitsService, 
                                     MoveService moveService, 
                                     VipService vipService, 
                                     ActionService actionService, 
                                     AdditionalService additionalService)
        {
            // Перетасовывая выполнители, можно выставить их приоритет выполнения
            _executors = new IExecuteOpGroup[]
            {
                new ExecuteOpGroupAction(unitsService, actionService),
                new ExecuteOpGroupAdditionalByPos(unitsService, additionalService),
                new ExecuteOpGroupPositionOnGrid(unitsService, moveService),
                new ExecuteOpGroupVip(unitsService, vipService)
            };
        }

        /// <summary>
        /// Метот принимает в себя группу из компонентов. Нужно сначала найти
        /// класс, который может выполнить действие, и после выполнить действие
        /// </summary>
        public void Execute(string gameId, int playerActorId, List<ISyncComponent> componentGroup)
        {
            foreach (IExecuteOpGroup executor in _executors)
            {
                if (!executor.CanExecute(componentGroup)){
                    continue;
                }

                executor.Execute(gameId, playerActorId, componentGroup);
            }
        }
    }
}

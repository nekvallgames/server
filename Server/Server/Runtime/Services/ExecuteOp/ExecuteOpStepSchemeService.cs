using Plugin.Interfaces;
using Plugin.Schemes;
using System.Collections.Generic;

namespace Plugin.Runtime.Services.ExecuteOp
{
    /// <summary>
    /// Сервіс, котрий виконає всі дії, котрі актор прислав в операції ActorStep
    /// </summary>
    public class ExecuteOpStepSchemeService
    {
        private ExecuteOpGroupService _executeOpGroupService;

        public ExecuteOpStepSchemeService(ExecuteOpGroupService executeOpGroupService)
        {
            _executeOpGroupService = executeOpGroupService;
        }

        public void Execute(string gameId, int actorId, int syncStep, StepScheme stepScheme)
        {
            int groupIndex = 0;

            while (true)
            {
                // 1. Вытаскиваем из кучи компонентов только ту группу, которая нам нужна, а именно: stepHistory и componentsGroup
                List<ISyncComponent> componentGroup = stepScheme.Get(syncStep, groupIndex);

                if (componentGroup == null || componentGroup.Count <= 0)
                {
                    // если список пуст, значит сортировщик не нашел групп из компонентов
                    // возможно, мы перебрали все компоненты в схеме
                    break;
                }

                // 2. Отправить группу из компонентов действий игрока на выполнение
                _executeOpGroupService.Execute(gameId, actorId, componentGroup);

                // 3. Увеличиваем шаг и обращаемся к следующей группе
                groupIndex++;
            }
        }
    }
}

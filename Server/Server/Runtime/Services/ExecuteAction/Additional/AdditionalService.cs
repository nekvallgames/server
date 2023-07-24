using Plugin.Interfaces;
using Plugin.Runtime.Services.ExecuteAction.Additional.Executors;
using Plugin.Runtime.Services.Sync;

namespace Plugin.Runtime.Services.ExecuteAction.Additional
{
    public class AdditionalService
    {
        /// <summary>
        /// Исполнители действий
        /// </summary>
        private IExecuteAction[] _executorsActions;

        public AdditionalService(SyncService syncService, UnitsService unitsService)
        {
            _executorsActions = new IExecuteAction[]
            {
                new Healing(syncService, unitsService)      // медик хочет вылечить юнита
            };
        }

        /// <summary>
        /// Выполнить для текущего юнита его действие
        /// Например: если юнит хиллер, то юнит должен вылечить какого то юнита
        /// </summary>
        /// <param name="unit"> Указать юнит, который будет выполнять действие </param>
        /// <param name="targetActorID"> Указать ID игрока, на сетке которого нужно выполнить действие </param>
        /// <param name="posW"> Позиция на игровой сетке </param>
        /// <param name="posH"> Позиция на игровой сетке </param>
        public void ExecuteAdditional(IUnit unit, string gameId, int targetActorID, int posW, int posH)
        {
            foreach (IExecuteAction executer in _executorsActions)
            {
                // Перебираем всех исполнителей действий, и проверяем, может 
                // ли какой то исполнитель выполнить действие для текущего юнита
                if (!executer.CanExecute(unit))
                {
                    continue;
                }

                // Текущим исполнителем выполнить действие для текущего юнита
                executer.Execute(unit, gameId, targetActorID, posW, posH);
                break;
            }
        }
    }
}

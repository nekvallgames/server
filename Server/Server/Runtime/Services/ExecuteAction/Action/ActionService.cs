using Plugin.Interfaces;
using Plugin.Runtime.Services.ExecuteAction.Action.Executors;
using Plugin.Runtime.Services.Sync;
using System.Diagnostics;

namespace Plugin.Runtime.Services.ExecuteAction.Action
{
    /// <summary>
    /// Сервіс, который будет выполнять основное действия юнита
    /// </summary>
    public class ActionService
    {
        /// <summary>
        /// Исполнители действий
        /// </summary>
        private IExecuteAction[] _executorsActions;

        public ActionService(SyncService syncService, UnitsService unitsService, SortTargetOnGridService sortTargetOnGridService)
        {
            _executorsActions = new IExecuteAction[]
            {
                new WaveDamageAction(syncService, unitsService, sortTargetOnGridService),    // выполнить бросок гранаты и взорвать ее
                new DamageAction(syncService, unitsService, sortTargetOnGridService)      // выстрелить 1 раз с огнестрельного оружия
            };
        }

        /// <summary>
        /// Выполнить для текущего юнита его действие
        /// Например: если юнит стрелок, то юнит должен выстрелить
        /// </summary>
        /// <param name="unit"> Указать юнит, который будет выполнять действие </param>
        /// <param name="targetActorID"> Указать ID игрока, на сетке которого нужно выполнить действие </param>
        /// <param name="posW"> Позиция на игровой сетке </param>
        /// <param name="posH"> Позиция на игровой сетке </param>
        public void ExecuteAction(IUnit unit, string gameId, int targetActorID, int posW, int posH)
        {
            foreach ( IExecuteAction executer in _executorsActions )
            {
                // Перебираем всех исполнителей действий, и проверяем, может 
                // ли какой то исполнитель выполнить действие для текущего юнита
                if (!executer.CanExecute(unit)){
                    continue;
                }

                // Текущим исполнителем выполнить действие для текущего юнита
                executer.Execute(unit, gameId, targetActorID, posW, posH);
                return;
            }

            Debug.Fail($"ActionService :: ExecuteAction() I can't execute action, because I don't know how. OwnerId = {unit.OwnerActorId}, uId = {unit.UnitId}, instanceId = {unit.InstanceId}");
        }
    }
}

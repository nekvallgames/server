using Plugin.Interfaces;
using Plugin.OpComponents;
using Plugin.Runtime.Services.ExecuteAction.Action;
using System.Collections.Generic;
using System.Diagnostics;

namespace Plugin.Runtime.Services.ExecuteOp.Executors
{
    /// <summary>
    /// Выполнить действие игрока - выполнить активное действие игрока.
    /// Например, если он стрелок, значит он выстрелил во вражеского юнита
    /// </summary>
    public class ExecuteOpGroupAction : IExecuteOpGroup
    {
        private UnitsService _unitsService;
        private ActionService _actionService;
        private ActionOpComponent _actionOpComponent;
        private UnitIdOpComponent _unitIdOpComponent;

        public ExecuteOpGroupAction(UnitsService unitsService, ActionService actionService)
        {
            _unitsService = unitsService;
            _actionService = actionService;
        }

        /// <summary>
        /// Может ли текущий класс выполнить действие игрока?
        /// </summary>
        public bool CanExecute(List<ISyncComponent> componentsGroup)
        {
            foreach (ISyncComponent component in componentsGroup){
                if (component.GetType() == typeof(ActionOpComponent)){
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Выполнить действие игрока
        /// </summary>
        public void Execute(string gameId, int actorId, List<ISyncComponent> componentsGroup)
        {
            // 1. Вытаскиваем нужные нам компоненты из списка
            Parce(componentsGroup);

            // 2. Найти юнита, который выполнил действие
            IUnit unit = _unitsService.GetUnit(gameId, actorId, _unitIdOpComponent.UnitId, _unitIdOpComponent.UnitInstance);

            if (unit == null){
                Debug.Fail($"ExecuteOpGroupService :: ExecuteOpAction :: Execute() playerActorID = {actorId}, unitID = {_unitIdOpComponent.UnitId}, instanceID = {_unitIdOpComponent.UnitInstance}. I don't find this unit for execute actions");
                return;
            }

            // 3. Отбращаемся к классу, который выполняет действия юнитов, и просим 
            // его, выполнять для текущего юнита действие
            _actionService.ExecuteAction(unit, gameId, _actionOpComponent.tai, _actionOpComponent.w, _actionOpComponent.h);
        }

        /// <summary>
        /// Распарсить входящие данные
        /// </summary>
        private void Parce(List<ISyncComponent> componentsGroup)
        {
            foreach (ISyncComponent component in componentsGroup)
            {
                if (component.GetType() == typeof(ActionOpComponent)){
                    _actionOpComponent = (ActionOpComponent)component;
                }
                else
                    if (component.GetType() == typeof(UnitIdOpComponent)){
                        _unitIdOpComponent = (UnitIdOpComponent)component;
                    }
            }
        }
    }
}

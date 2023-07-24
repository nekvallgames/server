using Plugin.Interfaces;
using Plugin.OpComponents;
using Plugin.Runtime.Services.ExecuteAction;
using System.Collections.Generic;
using System.Diagnostics;

namespace Plugin.Runtime.Services.ExecuteOp.Executors
{
    /// <summary>
    /// Выполнить действие игрока - активировать/деактивировать VIP для юнита
    /// </summary>
    public class ExecuteOpGroupVip : IExecuteOpGroup
    {
        private UnitsService _unitsService;
        private VipService _vipService;
        private VipOpComponent _vipOpComponent;
        private UnitIdOpComponent _unitIdOpComponent;

        public ExecuteOpGroupVip(UnitsService unitsService, VipService vipService)
        {
            _unitsService = unitsService;
            _vipService = vipService;
        }

        /// <summary>
        /// Может ли текущий класс выполнить действие игрока?
        /// </summary>
        public bool CanExecute(List<ISyncComponent> componentsGroup)
        {
            foreach (ISyncComponent component in componentsGroup){
                if (component.GetType() == typeof(VipOpComponent)){
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Выполнить действие игрока. Активировать/деактивировать VIP для юнита
        /// </summary>
        public void Execute(string gameId, int actorId, List<ISyncComponent> componentsGroup)
        {
            // Вытаскиваем нужные нам компоненты из списка
            Parce(componentsGroup);

            // 2. Найти юнита, который выполнил действие
            IUnit unit = _unitsService.GetUnit(gameId, actorId, _unitIdOpComponent.UnitId, _unitIdOpComponent.UnitInstance);

            if (unit == null){
                Debug.Fail($"ExecuteOpGroupService :: ExecuteVip() actorID = {actorId}, unitId = {_unitIdOpComponent.UnitId}, instanceId = {_unitIdOpComponent.UnitInstance}. I don't find this unit for execute vip");
                return;
            }

            // Обращаемся к классу, который активировать/деактивировать VIP для юнита, и просим 
            // его, выполнять для текущего юнита действие
            _vipService.ChangeVip(unit, _vipOpComponent.e);
        }

        /// <summary>
        /// Распарсить входящие данные
        /// </summary>
        private void Parce(List<ISyncComponent> componentsGroup)
        {
            foreach (ISyncComponent component in componentsGroup)
            {
                if (component.GetType() == typeof(VipOpComponent)){
                    _vipOpComponent = (VipOpComponent)component;
                }
                else
                    if (component.GetType() == typeof(UnitIdOpComponent)){
                        _unitIdOpComponent = (UnitIdOpComponent)component;
                    }
            }
        }
    }
}

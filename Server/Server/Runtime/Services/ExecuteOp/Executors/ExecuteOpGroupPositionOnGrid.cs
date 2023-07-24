using Plugin.Interfaces;
using Plugin.OpComponents;
using Plugin.Runtime.Services.ExecuteAction;
using System.Collections.Generic;
using System.Diagnostics;

namespace Plugin.Runtime.Services.ExecuteOp.Executors
{
    /// <summary>
    /// Виконати операцію клієнта, котру він прислав на Game Server
    /// Выполнить действие игрока - переместить игрового юнита на игровой сетке
    /// </summary>
    public class ExecuteOpGroupPositionOnGrid : IExecuteOpGroup
    {
        private UnitsService _unitsService;
        private MoveService _moveService;
        private UnitIdOpComponent _unitIdOpComponent;
        private PositionOnGridOpComponent _positionOnGridOpComponent;

        public ExecuteOpGroupPositionOnGrid(UnitsService unitsService, MoveService moveService)
        {
            _unitsService = unitsService;
            _moveService = moveService;
        }

        /// <summary>
        /// Может ли текущий класс выполнить действие игрока?
        /// </summary>
        public bool CanExecute( List<ISyncComponent> componentsGroup )
        {
            foreach (ISyncComponent component in componentsGroup){
                if (component.GetType() == typeof(PositionOnGridOpComponent)){
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Выполнить действие игрока. А именно позиционировать юнита на игровой сетке
        /// </summary>
        public void Execute(string gameId, int playerActorId, List<ISyncComponent> componentsGroup)
        {
            // Вытаскиваем нужные нам компоненты из списка
            Parce(componentsGroup);

            // Вытащить юнита, к которому будем применять перемещение
            IUnit unit = _unitsService.GetUnit(gameId, playerActorId, _unitIdOpComponent.UnitId, _unitIdOpComponent.UnitInstance);

            if (unit == null){
                Debug.Fail($"ExecuteOpGroupService :: ExecuteOpPositionOnGrid() playerActorID = {playerActorId}, unitID = {_unitIdOpComponent.UnitId}, instanceID = {_unitIdOpComponent.UnitInstance}, I don't find this unit for execute actions");
                return;
            }

            // Переместить юнита в указаную позицию
            _moveService.PositionOnGrid(unit, _positionOnGridOpComponent.w, _positionOnGridOpComponent.h);
        }

        /// <summary>
        /// Распарсить входящие данные
        /// </summary>
        private void Parce(List<ISyncComponent> componentsGroup)
        {
            foreach (ISyncComponent component in componentsGroup)
            {
                if (component.GetType() == typeof(PositionOnGridOpComponent)){
                    _positionOnGridOpComponent = (PositionOnGridOpComponent)component;
                }
                else
                    if (component.GetType() == typeof(UnitIdOpComponent)){
                        _unitIdOpComponent = (UnitIdOpComponent)component;
                    }
            }
        }
    }
}

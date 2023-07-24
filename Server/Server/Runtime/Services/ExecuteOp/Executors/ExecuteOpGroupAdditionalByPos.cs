using Plugin.Interfaces;
using Plugin.OpComponents;
using Plugin.Runtime.Services.ExecuteAction.Additional;
using System.Collections.Generic;
using System.Diagnostics;

namespace Plugin.Runtime.Services.ExecuteOp.Executors
{
    /// <summary>
    /// Выполнить дополнительное действие юнита - выполнить дополнительное (пассивное) действие юнита.
    /// Например, если юнит хиллер, то вылечить юнита
    /// </summary>
    public class ExecuteOpGroupAdditionalByPos : IExecuteOpGroup
    {
        private UnitsService _unitsService;
        private AdditionalService _additionalService;
        private AdditionalByPosOpComponent _additionalOpComponent;
        private UnitIdOpComponent _unitIdOpComponent;

        public ExecuteOpGroupAdditionalByPos(UnitsService unitsService, AdditionalService additionalService)
        {
            _unitsService = unitsService;
            _additionalService = additionalService;
        }

        /// <summary>
        /// Может ли текущий класс выполнить действие игрока?
        /// </summary>
        public bool CanExecute(List<ISyncComponent> componentsGroup)
        {
            foreach (ISyncComponent component in componentsGroup)
            {
                if (component.GetType() == typeof(AdditionalByPosOpComponent)){
                    return true;
                }
            }

            return false;
        }


        /// <summary>
        /// Выполнить дополнительное (пассивное) действия юнита
        /// </summary>
        public void Execute(string gameId, int playerActorId, List<ISyncComponent> componentsGroup)
        {
            // Вытаскиваем нужные нам компоненты из списка
            Parce(componentsGroup);

            // Найти юнита, который выполнил действие
            IUnit unit = _unitsService.GetUnit(gameId, playerActorId, _unitIdOpComponent.UnitId, _unitIdOpComponent.UnitInstance);

            if (unit == null){
                Debug.Fail($"ExecuteOpGroupService :: ExecuteOpAdditional :: Execute() playerActorID = {playerActorId}, unitID = {_unitIdOpComponent.UnitId}, instanceID = {_unitIdOpComponent.UnitInstance}. I don't find this unit for execute actions");
                return;
            }

            // Отбращаемся к классу, который выполняет действия юнитов, и просим 
            // его, выполнять для текущего юнита действие
            _additionalService.ExecuteAdditional(unit, gameId, _additionalOpComponent.tai, _additionalOpComponent.w, _additionalOpComponent.h);
        }

        /// <summary>
        /// Распарсить входящие данные
        /// </summary>
        private void Parce(List<ISyncComponent> componentsGroup)
        {
            foreach (ISyncComponent component in componentsGroup)
            {
                if (component.GetType() == typeof(AdditionalByPosOpComponent)){
                    _additionalOpComponent = (AdditionalByPosOpComponent)component;
                }
                else
                    if (component.GetType() == typeof(UnitIdOpComponent)){
                        _unitIdOpComponent = (UnitIdOpComponent)component;
                    }
            }
        }
    }
}

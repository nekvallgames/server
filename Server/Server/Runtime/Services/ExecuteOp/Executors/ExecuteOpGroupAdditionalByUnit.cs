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
    public class ExecuteOpGroupAdditionalByUnit : IExecuteOpGroup
    {
        private UnitsService _unitsService;
        private AdditionalService _additionalService;
        private AdditionalByUnitOpComponent _additionalOpComponent;
        private UnitIdOpComponent _unitIdOpComponent;

        public ExecuteOpGroupAdditionalByUnit(UnitsService unitsService, AdditionalService additionalService)
        {
            _unitsService = unitsService;
            _additionalService = additionalService;
        }

        /// <summary>
        /// Может ли текущий класс выполнить действие игрока?
        /// </summary>
        public virtual bool CanExecute(List<ISyncComponent> componentsGroup)
        {
            foreach (ISyncComponent component in componentsGroup)
            {
                if (component.GetType() == typeof(AdditionalByUnitOpComponent))
                {
                    return true;
                }
            }

            return false;
        }


        /// <summary>
        /// Выполнить дополнительное (пассивное) действия юнита
        /// </summary>
        public virtual void Execute(string gameId, int playerActorNr, List<ISyncComponent> componentsGroup)
        {
            // Вытаскиваем нужные нам компоненты из списка
            Parce(componentsGroup);

            // Найти юнита, который выполнил действие
            IUnit unit = _unitsService.GetUnit(gameId, playerActorNr, _unitIdOpComponent.UnitId, _unitIdOpComponent.UnitInstance);

            if (unit == null)
            {
                Debug.Fail($"ExecuteOpGroupService :: ExecuteOpAdditional :: Execute() playerActorNr = {playerActorNr}, unitId = {_unitIdOpComponent.UnitId}, instanceId = {_unitIdOpComponent.UnitInstance}. I don't find this unit for execute additional actions");
                return;
            }

            // Отбращаемся к классу, который выполняет действия юнитов, и просим 
            // его, выполнять для текущего юнита действие
            _additionalService.ExecuteAdditionalByUnit(unit, gameId, _additionalOpComponent.tai, _additionalOpComponent.uid, _additionalOpComponent.iid);
        }

        /// <summary>
        /// Распарсить входящие данные
        /// </summary>
        protected virtual void Parce(List<ISyncComponent> componentsGroup)
        {
            foreach (ISyncComponent component in componentsGroup)
            {
                if (component.GetType() == typeof(AdditionalByUnitOpComponent))
                {
                    _additionalOpComponent = (AdditionalByUnitOpComponent)component;
                }
                else
                    if (component.GetType() == typeof(UnitIdOpComponent))
                {
                    _unitIdOpComponent = (UnitIdOpComponent)component;
                }
            }
        }
    }
}

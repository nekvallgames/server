using Plugin.Interfaces;
using Plugin.Interfaces.UnitComponents;
using Plugin.Runtime.Services.Sync;
using Plugin.Runtime.Services.Sync.Groups;
using Plugin.Tools;
using System.Collections.Generic;
using System.Diagnostics;

namespace Plugin.Runtime.Services.ExecuteAction.Additional.Executors
{
    /// <summary>
    /// Выполнить дополнительное (пассивное) действие юнита
    /// 
    /// Текущий класс выполняет лечение юнитов
    /// 
    /// То есть, что мы делаем:
    /// 1. проверяем, есть ли у юнита медика хилки для лечения
    /// 2. целимся, находим цель в точке действия
    /// 3. лечим
    /// 4. забираем у игрока 1 хилку
    /// 
    /// </summary>
    public class Healing : IExecuteAction
    {
        private SyncService _syncService;
        private UnitsService _unitsService;

        public Healing(SyncService syncService, UnitsService unitsService)
        {
            _syncService = syncService;
            _unitsService = unitsService;
        }

        /// <summary>
        /// Может ли текущий класс выполнить действие для юнита?
        /// </summary>
        public bool CanExecute(IUnit unit)
        {
            return unit is IHealingAdditionalComponent;
        }

        /// <summary>
        /// Выполнить действие
        /// </summary>
        public void ExecuteByPos(IUnit unit, string gameId, int targetActorId, int posW, int posH)
        {
            IHealingAdditionalComponent unitMedic = GetHealingComponent(unit);
            if (unitMedic == null){
                Debug.Fail($"ExecuteAdditionalService :: Healing :: Execute() ownerActorNr = {unit.OwnerActorNr}, unitId = {unit.UnitId}, instanceId = {unit.InstanceId}, targetActorId = {targetActorId}, posH = {posH}, I can't healing, maybe I don't have ammunition.");
                return;
            }

            unitMedic.SpendAdditional();     // юнит вылечил когото. Забрать 1 аптечку

            // Синхронизировать выполненное действие юнита на игровой сетке
            ISyncGroupComponent syncAdditional = new SyncAdditionalByPosGroup(unit,
                                                                              targetActorId,
                                                                              posW,
                                                                              posH);
            _syncService.Add(gameId, unit.OwnerActorNr, syncAdditional);

            Int2[] additionalArea = unitMedic.GetAdditionalArea();

            foreach (Int2 area in additionalArea)
            {
                int targetW = posW + area.x;
                int targetH = posH + area.y;

                // Находим всех юнитоа, в которых мы тапнули, что бы их вылечить
                List<IUnit> unitTargets = _unitsService.GetUnitsUnderThisPosition(gameId,
                                                                                  targetActorId,
                                                                                  targetW,
                                                                                  targetH);

                if (unitTargets.Count <= 0){
                    return;                     // в текущий координатах тапа некого лечить
                }

                // викинути із списка юнітів, котрих не можно лікувати (наприклад це барр'єр)
                List<IUnit> units = unitTargets.FindAll(x => x is IHealthComponent);

                if (units.Count <= 0){
                    return;                     // в текущий координатах тапа некого лечить
                }

                int healthPower = unitMedic.GetHealthPower(); // сила лечения

                // Вылечить юнита
                _unitsService.Healing(units[0], healthPower);
            }
        }

        public void ExecuteByUnit(IUnit unit, string gameId, int targetActorId, int targetUnitId, int targetInstanceId)
        {
            IHealingAdditionalComponent unitMedic = GetHealingComponent(unit);
            if (unitMedic == null){
                Debug.Fail($"ExecuteAdditionalService :: Healing :: Execute() ownerActorNr = {unit.OwnerActorNr}, unitId = {unit.UnitId}, instanceId = {unit.InstanceId}, targetActorId = {targetActorId}, targetUnitId = {targetUnitId}, targteInstanceId = {targetInstanceId}. I can't healing, maybe I don't have ammunition.");
                return;
            }

            unitMedic.SpendAdditional();     // юнит вылечил когото. Забрать 1 аптечку

            // Синхронизировать выполненное действие юнита на игровой сетке
            ISyncGroupComponent syncAdditional = new SyncAdditionalByUnitGroup(unit,
                                                                               targetActorId,
                                                                               targetUnitId,
                                                                               targetInstanceId);
            _syncService.Add(gameId, unit.OwnerActorNr, syncAdditional);

            IUnit targetUnit = _unitsService.GetUnit(gameId, targetActorId, targetUnitId, targetInstanceId);
            if (targetUnit == null)
                return;

            int healthPower = unitMedic.GetHealthPower(); // сила лечения

            // Вылечить юнита
            _unitsService.Healing(targetUnit, healthPower);
        }

        private IHealingAdditionalComponent GetHealingComponent(IUnit unit)
        {
            // Проверяем, может ли юнит вылечить?
            IHealingAdditionalComponent unitMedic = (IHealingAdditionalComponent)unit;
            return unitMedic.CanExecuteAdditional() ? unitMedic : null;
        }
    }
}

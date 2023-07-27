using Plugin.Installers;
using Plugin.Interfaces;
using Plugin.Interfaces.Actions;
using Plugin.Runtime.Services.Sync;
using Plugin.Runtime.Services.Sync.Groups;
using Plugin.Tools;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Plugin.Runtime.Services.ExecuteAction.Action.Executors
{
    /// <summary>
    /// Выполнить действие юнита
    /// 
    /// Текущий класс выполняет один выстрел с огнестрельного оружия
    /// 
    /// То есть, что мы делаем:
    /// 1. проверяем, есть ли у юнита патрон для выстрела
    /// 2. целимся, находим цель в точке выстрела
    /// 3. стреляем, снимаем урон
    /// 4. забираем у игрока 1 патрон
    /// 
    /// </summary>
    public class DamageAction : IExecuteAction
    {
        private SyncService _syncService;
        private UnitsService _unitsService;
        private SortTargetOnGridService _sortTargetOnGridService;

        public DamageAction(SyncService syncService, UnitsService unitsService, SortTargetOnGridService sortTargetOnGridService)
        {
            _syncService = syncService;
            _unitsService = unitsService;
            _sortTargetOnGridService = sortTargetOnGridService;
        }

        /// <summary>
        /// Может ли текущий класс выполнить действие для юнита?
        /// </summary>
        public bool CanExecute(IUnit unit)
        {
            return typeof(IDamageAction).IsAssignableFrom(unit.GetType());
        }

        /// <summary>
        /// Выполнить действие
        /// </summary>
        public void Execute( IUnit unit, string gameId, int targetActorId, int posW, int posH )
        {
            // Проверяем, может ли юнит вытсрелить?
            var damageAction = (IDamageAction)unit;

            if (!damageAction.CanExecuteAction()){
                Debug.Fail($"ExecuteActionService :: WeaponShot :: Execute() ownerID = {unit.OwnerActorNr}, unitID = {unit.UnitId}, instanceID = {unit.InstanceId}, targetActorID = {targetActorId}, posW = {posW}, posH = {posH}, I can't make damage action. Maybe I don't have ammunition.");
            }

            damageAction.SpendAction();     // делаем выстрел. Юнит тратит 1 патрон

            // Синхронизировать выполненное действие юнита на игровой сетке
            var syncData = new SyncActionGroup(unit,
                                               targetActorId,
                                               posW,
                                               posH);
            _syncService.Add(gameId, unit.OwnerActorNr, syncData);


            foreach (Int2 area in damageAction.DamageActionArea)
            {
                int targetW = posW + area.x;
                int targetH = posH + area.y;

                // Находим всех противников, в которых мы выстрелили
                List<IUnit> targets = _sortTargetOnGridService.SortTargets(_unitsService.GetUnitsUnderThisPosition(gameId, targetActorId, targetW, targetH));

                LogChannel.Log($"ActionService :: DamageAction() ownerId = {unit.OwnerActorNr}, unitId = {unit.UnitId}, instanceId = {unit.InstanceId}, cellW = {targetW}, cellH = {targetH}");

                if (targets.Count <= 0){
                    continue;                     // игрок выстрелил мимо!
                }

                int damage = damageAction.Power;   // получить урон, который игрок нанес выстрелом

                _unitsService.SetDamage(targets[0], damage);
            }
        }
    }
}

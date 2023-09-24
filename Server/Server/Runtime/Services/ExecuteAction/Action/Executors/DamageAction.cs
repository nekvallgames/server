using Plugin.Installers;
using Plugin.Interfaces;
using Plugin.Interfaces.Actions;
using Plugin.Runtime.Services.Sync;
using Plugin.Runtime.Services.Sync.Groups;
using Plugin.Schemes;
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
        private SortHitOnGridService _sortTargetOnGridService;
        private BodyDamageConverterService _bodyDamageConverterService;
        private GridService _gridService;

        public DamageAction(SyncService syncService, 
                            UnitsService unitsService, 
                            SortHitOnGridService sortTargetOnGridService,
                            BodyDamageConverterService bodyDamageConverterService,
                            GridService gridService)
        {
            _syncService = syncService;
            _unitsService = unitsService;
            _sortTargetOnGridService = sortTargetOnGridService;
            _bodyDamageConverterService = bodyDamageConverterService;
            _gridService = gridService;
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
        public void Execute(IUnit unit, string gameId, int targetActorNr, int posW, int posH)
        {
            // Проверяем, может ли юнит вытсрелить?
            var damageAction = (IDamageAction)unit;

            if (!damageAction.CanExecuteAction()){
                Debug.Fail($"ExecuteActionService :: WeaponShot :: Execute() ownerID = {unit.OwnerActorNr}, unitID = {unit.UnitId}, instanceID = {unit.InstanceId}, targetActorNr = {targetActorNr}, posW = {posW}, posH = {posH}, I can't make damage action. Maybe I don't have ammunition.");
            }

            damageAction.SpendAction();     // делаем выстрел. Юнит тратит 1 патрон

            // Синхронизировать выполненное действие юнита на игровой сетке
            var syncData = new SyncActionGroup(unit,
                                               targetActorNr,
                                               posW,
                                               posH);
            _syncService.Add(gameId, unit.OwnerActorNr, syncData);

            IGrid grid = _gridService.Get(gameId, targetActorNr);

            foreach (Int2 area in damageAction.ActionArea)
            {
                int targetW = posW + area.x;
                int targetH = posH + area.y;

                // Находим всех противников, в которых мы выстрелили
                List<IUnit> targets = _sortTargetOnGridService.SortTargets(_unitsService.GetUnitsUnderThisPosition(gameId, targetActorNr, targetW, targetH));

                // LogChannel.Log($"ActionService :: DamageAction() ownerId = {unit.OwnerActorNr}, unitId = {unit.UnitId}, instanceId = {unit.InstanceId}, cellW = {targetW}, cellH = {targetH}");

                if (targets.Count <= 0)
                {
                    // куля попала мимо юніта. Перевіряємо, чи попала куля в селл
                    Cell cell = _gridService.GetCell(grid, targetW, targetH);
                    if (cell != null)
                    {
                        cell.binaryMask = Enums.CellMask.cellWalkLock;
                    }

                    continue;                     
                }

                int damage = damageAction.Damage;   // получить урон, который игрок нанес выстрелом
                IUnit unitTarget = targets[0];      // отримати юніта, по котрому нанесемо урон

                // Конвертировать урон по части тела
                int damageByBody = _bodyDamageConverterService.Converter(unitTarget, damage, targetW, targetH);

                _unitsService.SetDamage(unitTarget, damageByBody);
            }
        }
    }
}

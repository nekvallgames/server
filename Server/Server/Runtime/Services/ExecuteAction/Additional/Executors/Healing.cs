﻿using Plugin.Interfaces;
using Plugin.Runtime.Services.Sync;
using Plugin.Runtime.Services.Sync.Groups;
using Plugin.Tools;
using System;
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
            return unit is IHealingAdditional;
        }

        /// <summary>
        /// Выполнить действие
        /// </summary>
        public void Execute(IUnit unit, string gameId, int targetActorId, int posW, int posH)
        {
            // Проверяем, может ли юнит вылечить?
            IHealingAdditional unitMedic = (IHealingAdditional)unit;

            if (!unitMedic.CanExecuteAdditional()){
                Debug.Fail($"ExecuteAdditionalService :: Healing :: Execute() ownerId = {unit.OwnerActorId}, unitId = {unit.UnitId}, instanceId = {unit.InstanceId}, targetActorId = {targetActorId}, posH = {posH}, I can't healing, maybe I don't have ammunition.");
            }

            unitMedic.SpendAdditional();     // юнит вылечил когото. Забрать 1 аптечку

            // Синхронизировать выполненное действие юнита на игровой сетке
            ISyncGroupComponent syncOnGrid = new SyncAdditionalGroup(unit,
                                                                     targetActorId,
                                                                     posW,
                                                                     posH);
            _syncService.Add(gameId, unit.OwnerActorId, syncOnGrid);


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

                if (unitTargets.Count <= 0)
                {
                    return;                     // в текущий координатах тапа некого лечить
                }

                int healthPower = unitMedic.GetHealthPower(); // сила лечения

                // Вылечить юнита
                _unitsService.Healing(unitTargets[0], healthPower);
            }
        }
    }
}

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
    /// Текущий класс выполняет бросок и взрыв гранаты
    /// 
    /// То есть, что мы делаем:
    /// 1. проверяем, есть ли у юнита граната для выстрела
    /// 2. целимся, находим цель в точке выстрела
    /// 3. бросаем, наносим урон
    /// 4. забираем у игрока 1-у гранату
    /// 
    /// </summary>
    public class WaveDamageAction : IExecuteAction
    {
        private SyncService _syncService;
        private UnitsService _unitsService;
        private SortHitOnGridService _sortTargetOnGridService;

        public WaveDamageAction(SyncService syncService, UnitsService unitsService, SortHitOnGridService sortTargetOnGridService)
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
            return typeof(IWaveDamageAction).IsAssignableFrom(unit.GetType());
        }

        /// <summary>
        /// Выполнить действие
        /// </summary>
        public void Execute(IUnit unit, string gameId, int targetActorId, int posW, int posH)
        {
            // Проверяем, может ли юнит выстрелить?
            var waveDamageAction = (IWaveDamageAction)unit;

            if (!waveDamageAction.CanExecuteAction()){
                Debug.Fail($"ExecuteActionService :: GrenadeShot :: Execute() ownerID = {unit.OwnerActorNr}, unitID = {unit.UnitId}, instanceID = {unit.InstanceId}, targetActorID = {targetActorId}, posW = {posW}, posH = {posH}, I can't shot, maybe I don't have ammunition.");
            }

            waveDamageAction.SpendAction();     // делаем бросок гранаты. Юнит тратит 1-у гранату

            // Синхронизировать выполненное действие юнита на игровой сетке
            var syncOnGrid = new SyncActionGroup(unit,
                                                 targetActorId,
                                                 posW,
                                                 posH);
            _syncService.Add(gameId, unit.OwnerActorNr, syncOnGrid);


            // 2 2 2 2 2
            // 2 1 1 1 2
            // 2 1 0 1 2
            // 2 1 1 1 2
            // 2 2 2 2 2
            Int2[] actionArea = waveDamageAction.ActionArea;

            // Перебираем каждую ячейку поля взрыва
            foreach (Int2 area in actionArea)
            {
                // Найти урон, взависимости от волны взрыва гранаты
                int waveIndex = CalculateWave(area.x, area.y);
                int damage = CalculateDamage(waveDamageAction.Damage, waveDamageAction, waveIndex);

                int targetW = posW + area.x;
                int targetH = posH + area.y;

                // Находим всех противников, в которых мы выстрелили
                List<IUnit> enemyTargets = _sortTargetOnGridService.SortTargets(_unitsService.GetUnitsUnderThisPosition(gameId, targetActorId, targetW, targetH));

                LogChannel.Log($"ActionService :: WaveDamageAction() ownerId = {unit.OwnerActorNr}, unitId = {unit.UnitId}, instanceId = {unit.InstanceId}, cellW = {targetW}, cellH = {targetH}");

                if (enemyTargets.Count <= 0){
                    continue;                     // игрок выстрелил мимо!
                }

                _unitsService.SetDamage(enemyTargets[0], damage);
            }
        }

        /// <summary>
        /// Посчитать, какая это волна в взрыве
        /// Например: в точке клика, это волна == 0
        /// </summary>
        private int CalculateWave(int positionOnGridW, int positionOnGridH)
        {
            return Math.Max(Math.Abs(positionOnGridW), Math.Abs(positionOnGridH));
        }

        /// <summary>
        /// Высчитать урон для текущей волны
        /// weaponDamage - главный урон от взрыва гранаты (это в позиции клика)
        /// grenadeParamComponent - в компоненте находятся параметры в уроном от волны
        /// waveIndex - текущий волна
        /// </summary>
        private int CalculateDamage(int weaponDamage, IWaveDamageAction unitWithGrenade, int waveIndex)
        {
            switch (waveIndex)
            {
                case 0:
                    {
                        return weaponDamage;
                    }

                case 1:
                    {
                        return unitWithGrenade.GetWaveDamage()[0];
                    }

                case 2:
                    {
                        return unitWithGrenade.GetWaveDamage()[1];
                    }

                default:
                case 3:
                    {
                        return unitWithGrenade.GetWaveDamage()[2];
                    }
            }
        }
    }
}

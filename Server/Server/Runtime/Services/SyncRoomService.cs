﻿using Plugin.Builders;
using Plugin.Interfaces;
using Plugin.Schemes;
using System.Collections.Generic;

namespace Plugin.Runtime.Services
{
    /// <summary>
    /// Сервіс, котрий зберігає дії гравців для подальшої синхронізації із Game Server
    /// </summary>
    public class SyncRoomService : ISyncRoomService
    {
        private UnitsService _unitsService;
        private ActorStepsService _actorStepsService;

        public SyncRoomService(UnitsService unitsService, ActorStepsService actorStepsService)
        {
            _unitsService = unitsService;
            _actorStepsService = actorStepsService;
        }

        /// <summary>
        /// Покласти на склад збережену дію гравця для подальшої синхронізації із Game Server
        /// </summary>
        public void AddSync(string gameId, int actorNr, int stepNumber, IGroupSyncComponents components)
        {
            //IPlotModelScheme plotModelScheme = _plotsModelService.Get(gameId);

            // Перед тим, як покласти группу із компонентів на склад для подальшої синхронізації
            // проставляємо кожному компоненту GroupIndex та SyncStep
            ActorStepScheme actorStepScheme = _actorStepsService.Get(gameId, actorNr);

            // ActorSyncScheme actorSyncScheme = _syncPrivateModel.Items.Find(x => x.ActorId == actorId);
            //int syncCount = actorSyncScheme.GetSyncByStep(_plotPrivateModelProvider.Model<PlotPVPPrivateModel>().SyncStep).Count;

            int groupIndex = actorStepScheme.GetNextGroupIndex(stepNumber);

            foreach (ISyncComponent component in components.SyncComponents)
            {
                component.SyncStep = stepNumber;
                component.GroupIndex = groupIndex;

                actorStepScheme.steps[stepNumber].Add(component);
            }
            
            // actorSyncScheme.AddSync(components, _plotPrivateModelProvider.Model<PlotPVPPrivateModel>().SyncStep); 
        }

        /// <summary>
        /// Зберегти позиції всіх юнітів актора
        /// </summary>
        public void SyncPositionOnGrid(string gameId, int actorNr, int stepNumber)
        {
            List<IUnit> units = _unitsService.GetUnits(gameId, actorNr);

            foreach (IUnit unit in units)
            {
                SyncPositionOnGrid(unit, stepNumber);
            }
        }

        /// <summary>
        /// Зберегти позицію вказаного юніта актора 
        /// </summary>
        public void SyncPositionOnGrid(string gameId, int actorNr, int stepNumber, int unitId, int unitInstanceId)
        {
            SyncPositionOnGrid(_unitsService.GetUnit(gameId, actorNr, unitId, unitInstanceId), stepNumber);
        }

        /// <summary>
        /// Получить историю с экшенами, действия игрока в комнате
        /// </summary>
        // public List<IGroupSyncComponents> GetSyncByStep(string gameId, int actorNr, int syncStep)
        // {
        //     return _actorStepsService.Get(gameId, actorNr).stepScheme.GetSyncByStep(syncStep);
        // }

        /// <summary>
        /// Зберегти зміну віпа вказаного юніта
        /// </summary>
        /// <param name="actorId">актор, володарь дії</param>
        /// <param name="vipUnit"></param>
        public void SyncVip(string gameId, int actorNr, int stepNumber, IUnit vipUnit)
        {
            IGroupSyncComponents groupSync = new GroupSyncBuilder().CreateVip(vipUnit);
            AddSync(gameId, actorNr, stepNumber, groupSync);
        }

        /// <summary>
        /// Симулювати синхронізацію віпа для вказаного юніта
        /// </summary>
        /// <param name="actorId">актор, володарь дії</param>
        /// <param name="unit">юніт, до котрого потрібно примінити синхронізацію</param>
        /// <param name="isVip">вказати юніт стане віпом чи ні</param>
        public void SimulateSyncVip(string gameId, int actorNr, int stepNumber, IUnit unit, bool isVip)
        {
            IGroupSyncComponents groupSync = new GroupSyncBuilder().CreateSimulateVip(unit, isVip);
            AddSync(gameId, actorNr, stepNumber, groupSync);
        }

        /// <summary>
        /// Зберегти дію актора для подальшої синхронізації із сервером
        /// </summary>
        /// <param name="actorId">актор, володарь дії</param>
        /// <param name="unitId">юніт, котрий виконує дію</param>
        /// <param name="unitInstanceId">інстанс юніта</param>
        /// <param name="targetActorId">ід актора, із сіткой котрого ми взаємодіємо</param>
        /// <param name="targetPosW">позіція тача на ігровій сітці</param>
        /// <param name="targetPosH">позіція тача на ігровій сітці</param>
        public void SyncAction(string gameId, 
                               int actorNr,
                               int stepNumber,
                               int unitId, 
                               int unitInstanceId, 
                               int targetActorId, 
                               int targetPosW, 
                               int targetPosH)
        {
            IGroupSyncComponents groupSync = new GroupSyncBuilder().CreateAction(unitId,
                                                                                 unitInstanceId,
                                                                                 targetActorId,
                                                                                 targetPosW,
                                                                                 targetPosH);
            AddSync(gameId, actorNr, stepNumber, groupSync);
        }

        /// <summary>
        /// Зберегти додаткову дію актора по координатам для подальшої синхронізації із сервером
        /// </summary>
        /// <param name="ownerActorId">актор, володарь дії</param>
        /// <param name="unitId">юніт, котрий виконує дію</param>
        /// <param name="unitInstanceId">інстанс юніта</param>
        /// <param name="targetActorNr">ід актора, із сіткой котрого ми взаємодіємо</param>
        /// <param name="targetPosW">позіція тача на ігровій сітці</param>
        /// <param name="targetPosH">позіція тача на ігровій сітці</param>
        public void SyncAdditionalByPos(string gameId,
                                        int actorNr,
                                        int stepNumber,
                                        int unitId,
                                        int unitInstanceId,
                                        int targetActorNr,
                                        int targetPosW,
                                        int targetPosH)
        {
            IGroupSyncComponents groupSync = new GroupSyncBuilder().CreateAdditionalByPos(unitId,
                                                                                          unitInstanceId,
                                                                                          targetActorNr,
                                                                                          targetPosW,
                                                                                          targetPosH);

            AddSync(gameId, actorNr, stepNumber, groupSync);
        }

        /// <summary>
        /// Зберегти додаткову дію актора для вказаного юніта для подальшої синхронізації із сервером
        /// </summary>
        /// <param name="ownerActorId">актор, володарь дії</param>
        /// <param name="unitId">юніт, котрий виконує дію</param>
        /// <param name="unitInstanceId">інстанс юніта</param>
        /// <param name="targetActorId">ід актора, із сіткой котрого ми взаємодіємо</param>
        /// <param name="targetUnitId">вказати юніт Id, до котрого буде примінена додаткова дія</param>
        /// <param name="targetUnitInstanceId">вказати юніт інстанс Id, до котрого буде примінена додаткова дія</param>
        public void SyncAdditionalByUnit(string gameId, 
                                         int actorNr,
                                         int stepNumber,
                                         int unitId,
                                         int unitInstanceId,
                                         int targetActorId,
                                         int targetUnitId,
                                         int targetUnitInstanceId)
        {
            IGroupSyncComponents groupSync = new GroupSyncBuilder().CreateAdditionalByUnit(unitId,
                                                                                           unitInstanceId,
                                                                                           targetActorId,
                                                                                           targetUnitId,
                                                                                           targetUnitInstanceId);

            AddSync(gameId, actorNr, stepNumber, groupSync);
        }

        /// <summary>
        /// Зберегти позіцію юніта для подальшої синхронізації із сервером
        /// </summary>
        public void SyncPositionOnGrid(IUnit unit, int stepNumber)
        {
            IGroupSyncComponents groupSync = new GroupSyncBuilder().CreatePositionOnGrid(unit);
            AddSync(unit.GameId, unit.OwnerActorNr, stepNumber, groupSync);
        }

        public void SyncPositionOnGrid(IUnit unit, int stepNumber, int positionInGridW, int positionInGridH)
        {
            IGroupSyncComponents groupSync = new GroupSyncBuilder().CreatePositionOnGrid(unit,
                                                                                         positionInGridW,
                                                                                         positionInGridH);
            AddSync(unit.GameId, unit.OwnerActorNr, stepNumber, groupSync);
        }
    }
}

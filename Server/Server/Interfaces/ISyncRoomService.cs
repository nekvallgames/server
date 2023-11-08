using System.Collections.Generic;

namespace Plugin.Interfaces
{
    public interface ISyncRoomService
    {
        /// <summary>
        /// Покласти на склад збережену дію гравця для подальшої синхронізації із Game Server
        /// </summary>
        void AddSync(string gameId, int actorNr, IGroupSyncComponents components);

        /// <summary>
        /// Зберегти позиції всіх юнітів актора
        /// </summary>
        void SyncPositionOnGrid(string gameId, int actorNr);

        /// <summary>
        /// Зберегти позицію вказаного юніта актора 
        /// </summary>
        void SyncPositionOnGrid(string gameId, int actorNr, int unitId, int unitInstanceId);

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
        void SyncVip(string gameId, int actorNr, IUnit vipUnit);

        /// <summary>
        /// Симулювати синхронізацію віпа для вказаного юніта
        /// </summary>
        /// <param name="actorId">актор, володарь дії</param>
        /// <param name="unit">юніт, до котрого потрібно примінити синхронізацію</param>
        /// <param name="isVip">вказати юніт стане віпом чи ні</param>
        void SimulateSyncVip(string gameId, int actorNr, IUnit unit, bool isVip);

        /// <summary>
        /// Зберегти дію актора для подальшої синхронізації із сервером
        /// </summary>
        /// <param name="actorId">актор, володарь дії</param>
        /// <param name="unitId">юніт, котрий виконує дію</param>
        /// <param name="unitInstanceId">інстанс юніта</param>
        /// <param name="targetActorId">ід актора, із сіткой котрого ми взаємодіємо</param>
        /// <param name="targetPosW">позіція тача на ігровій сітці</param>
        /// <param name="targetPosH">позіція тача на ігровій сітці</param>
        void SyncAction(string gameId,
                               int actorNr,
                               int unitId,
                               int unitInstanceId,
                               int targetActorId,
                               int targetPosW,
                               int targetPosH);

        /// <summary>
        /// Зберегти додаткову дію актора по координатам для подальшої синхронізації із сервером
        /// </summary>
        /// <param name="ownerActorId">актор, володарь дії</param>
        /// <param name="unitId">юніт, котрий виконує дію</param>
        /// <param name="unitInstanceId">інстанс юніта</param>
        /// <param name="targetActorId">ід актора, із сіткой котрого ми взаємодіємо</param>
        /// <param name="targetPosW">позіція тача на ігровій сітці</param>
        /// <param name="targetPosH">позіція тача на ігровій сітці</param>
        void SyncAdditionalByPos(string gameId,
                                        int actorNr,
                                        int unitId,
                                        int unitInstanceId,
                                        int targetActorId,
                                        int targetPosW,
                                        int targetPosH);

        /// <summary>
        /// Зберегти додаткову дію актора для вказаного юніта для подальшої синхронізації із сервером
        /// </summary>
        /// <param name="ownerActorId">актор, володарь дії</param>
        /// <param name="unitId">юніт, котрий виконує дію</param>
        /// <param name="unitInstanceId">інстанс юніта</param>
        /// <param name="targetActorId">ід актора, із сіткой котрого ми взаємодіємо</param>
        /// <param name="targetUnitId">вказати юніт Id, до котрого буде примінена додаткова дія</param>
        /// <param name="targetUnitInstanceId">вказати юніт інстанс Id, до котрого буде примінена додаткова дія</param>
        void SyncAdditionalByUnit(string gameId,
                                         int actorNr,
                                         int unitId,
                                         int unitInstanceId,
                                         int targetActorId,
                                         int targetUnitId,
                                         int targetUnitInstanceId);

        /// <summary>
        /// Зберегти позіцію юніта для подальшої синхронізації із сервером
        /// </summary>
        void SyncPositionOnGrid(IUnit unit);

        void SyncPositionOnGrid(IUnit unit, int positionInGridW, int positionInGridH);
    }
}

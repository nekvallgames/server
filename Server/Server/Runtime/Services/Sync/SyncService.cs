using Plugin.Interfaces;
using Plugin.Models.Private;
using Plugin.Schemes;
using System.Linq;

namespace Plugin.Runtime.Services.Sync
{
    /// <summary>
    /// Сервіс, котрий зберігає в собі виконані дії юнітів
    /// Що би в подальшому синхронізувати дії юнітів на стороні сервера із діями на стороні клієнту
    /// </summary>
    public class SyncService
    {
        private SyncPrivateModel _syncPrivateModel;
        private PlotsModelService _plotsModelService;

        public SyncService(SyncPrivateModel syncPrivateModel, PlotsModelService plotsModelService)
        {
            _syncPrivateModel = syncPrivateModel;
            _plotsModelService = plotsModelService;
        }

        /// <summary>
        /// Зберегти дію гравця, щоб потім синхронувати цю дію між гравцями
        /// </summary>
        public void Add(string gameId, int actorId, ISyncGroupComponent syncData )
        {
            int plotStep = _plotsModelService.Get(gameId, actorId).SyncStep;   // витягуємо із моделі ігрового сценарія поточний крок ігрового сценарія
  
            var syncStep = Get(gameId, actorId, plotStep);

            // Нужно перебрать все компоненты в syncAction,
            // и проставить глобальный шаг и обьединить все компоненты в группу
            foreach (ISyncComponent syncComponent in syncData.SyncElements)
            {
                syncComponent.SyncStep = plotStep;                   // глобальный шаг, которому принадлежит синхронизация
                syncComponent.GroupIndex = syncStep.SyncGroups.Count;  // обьединить все компоненты в группу
            }
            syncStep.SyncGroups.Add(syncData);
        }

        public SyncScheme Get(string gameId, int actorId, int syncStep)
        {
            if (_syncPrivateModel.Items.Any(x => x.GameId == gameId && x.ActorId == actorId && x.SyncStep == syncStep)){
                return _syncPrivateModel.Items.Find(x => x.GameId == gameId && x.ActorId == actorId && x.SyncStep == syncStep);
            }

            var syncScheme = new SyncScheme(gameId, actorId, syncStep);
            _syncPrivateModel.Add(syncScheme);

            return syncScheme;
        }
    }
}

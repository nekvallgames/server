using Plugin.Interfaces;
using System;
using System.Collections.Generic;

namespace Plugin.Schemes
{
    public class ActorStepScheme
    {
        public string GameId { get; }
        public int OwnerActorId { get; }

        /// <summary>
        /// Список с экшенами, действия игрока в комнате
        /// </summary>
        //private Dictionary<int, List<IGroupSyncComponents>> _syncList = new Dictionary<int, List<IGroupSyncComponents>>();

        /// <summary>
        /// Схема со всеми действиями игрока
        /// Куча компонентов, которые разсортированы по спискам
        /// </summary>
        public StepScheme stepScheme;

        public ActorStepScheme(string gameId, int actorId)
        {
            GameId = gameId;
            OwnerActorId = actorId;
        }

        public int GetNextGroupIndex()
        {
            return stepScheme.syncUnitId.Count;
        }

        /// <summary>
        /// Зберегти дії актора
        /// </summary>
        /*public void AddSync(IGroupSyncComponents groupSyncComponents, int syncStep)
        {
            if (!_syncList.ContainsKey(syncStep))
            {
                _syncList.Add(syncStep, new List<IGroupSyncComponents>());
            }

            _syncList[syncStep].Add(groupSyncComponents);
        }

        /// <summary>
        /// Отримати список дій актора
        /// </summary>
        public List<IGroupSyncComponents> GetSyncByStep(int syncStep)
        {
            if (!_syncList.ContainsKey(syncStep))
            {
                _syncList.Add(syncStep, new List<IGroupSyncComponents>());
            }

            return _syncList[syncStep];
        }*/
    }
}

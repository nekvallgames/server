using Plugin.Interfaces;
using System.Collections.Generic;

namespace Plugin.Schemes
{
    /// <summary>
    /// Зберігає список дій, котрі гравець виконав на поточному кроці 
    /// </summary>
    public struct SyncScheme
    {
        /// <summary>
        /// GameId ігрової кімнати, которому принадлежит синхронизация
        /// </summary>
        public string GameId { get; }

        /// <summary>
        /// ActorId игрока, которому принадлежит синхронизация
        /// </summary>
        public int ActorId { get; }

        /// <summary>
        /// Крок синхронізації
        /// </summary>
        public int SyncStep { get; }

        /// <summary>
        /// Список дій, котрі гравець виконав на поточному кроці
        /// </summary>
        public List<ISyncGroupComponent> SyncGroups;


        public SyncScheme( string gameId, int actorId, int syncStep )
        {
            GameId = gameId;
            ActorId = actorId;
            SyncStep = syncStep;

            SyncGroups = new List<ISyncGroupComponent>();
        }
    }
}

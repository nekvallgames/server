using System.Collections.Generic;

namespace Plugin.Schemes
{
    public struct ActorUnitsPathPrivateScheme
    {
        /// <summary>
        /// Вказати id ігрової кімнати, в котрій знаходиться актер
        /// </summary>
        public string GameId { get; private set; }

        /// <summary>
        /// Владелец игровой сетки
        /// </summary>
        public int OwnerActorNr { get; private set; }

        public List<UnitPathPrivateScheme> unitsPath;

        public ActorUnitsPathPrivateScheme(string gameId, int ownerActorNr)
        {
            GameId = gameId;
            OwnerActorNr = ownerActorNr;

            unitsPath = new List<UnitPathPrivateScheme>();
        }
    }
}

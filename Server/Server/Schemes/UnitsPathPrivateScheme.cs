using System.Collections.Generic;

namespace Plugin.Schemes
{
    public struct UnitsPathPrivateScheme
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

        public UnitsPathPrivateScheme(string gameId, int ownerActorNr)
        {
            GameId = gameId;
            OwnerActorNr = ownerActorNr;

            unitsPath = new List<UnitPathPrivateScheme>();
        }
    }
}

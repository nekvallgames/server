using Plugin.Interfaces;
using System.Collections.Generic;

namespace Plugin.Schemes
{
    public struct ActorScheme : IActorScheme
    {
        public string GameId { get; private set; }
        public int ActorNr { get; private set; }
        public string ProfileId { get; private set; }
        public List<int> Deck { get; set; }

        public ActorScheme(string gameId, int actorId, string profileId)
        {
            GameId = gameId;
            ActorNr = actorId;
            ProfileId = profileId;

            Deck = new List<int>();
        }
    }
}

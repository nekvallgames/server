using System.Collections.Generic;

namespace Plugin.Interfaces
{
    public interface IActorScheme
    {
        string GameId { get; }
        int ActorNr { get; }
        string ProfileId { get; }
        List<int> Deck { get; set; }
    }
}

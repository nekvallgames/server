using Plugin.Interfaces;
using Plugin.Schemes;
using Plugin.Templates;
using System.Linq;

namespace Plugin.Models.Private
{
    public class ActorUnitsPathPrivateModel : BaseModel<ActorUnitsPathPrivateScheme>, IPrivateModel
    {
        public bool Has(string gameId, int actorId)
        {
            return Items.Any(x => x.GameId == gameId && x.OwnerActorNr == actorId);
        }

        public ActorUnitsPathPrivateScheme Get(string gameId, int actorId)
        {
            return Items.Find(x => x.GameId == gameId && x.OwnerActorNr == actorId);
        }
    }
}

using Plugin.Interfaces;
using Plugin.Schemes;
using Plugin.Templates;

namespace Plugin.Models.Private
{
    /// <summary>
    /// Модель із даними, котра зберігає в собі схему із селими, по котрим юніти може переміщатися.
    /// Сели на котрих може стояти
    /// </summary>
    public class CellWalkablePrivateModel : BaseModel<CellWalkablePrivateScheme>, IPrivateModel
    {
        public CellWalkablePrivateScheme Get(string gameId, int ownerActorNr)
        {
            return Items.Find(x => x.GameId == gameId && x.OwnerActorNr == ownerActorNr);
        }
    }
}

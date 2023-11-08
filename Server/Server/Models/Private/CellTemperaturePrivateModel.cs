using Plugin.Interfaces;
using Plugin.Schemes;
using Plugin.Templates;
using System.Linq;

namespace Plugin.Models.Private
{
    /// <summary>
    /// Модель із даними, котра буде зберігати в собі температурний слід переміщення ігрових персонажів
    /// </summary>
    public class CellTemperaturePrivateModel : BaseModel<CellTemperatureTracesScheme>, IPrivateModel
    {
        public bool Has(string gameId, int actorNr)
        {
            return Items.Any(x => x.GameId == gameId && x.ActorNr == actorNr);
        }

        public CellTemperatureTracesScheme Get(string gameId, int actorNr)
        {
            return Items.Find(x => x.GameId == gameId && x.ActorNr == actorNr);
        }
    }
}

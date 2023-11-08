using Plugin.Interfaces;
using System.Collections.Generic;

namespace Plugin.Schemes
{
    /// <summary>
    /// Схема, котра зберігає в собі температурний слід переміщення юнітів актора
    /// </summary>
    public class CellTemperatureTracesScheme : ITemperatureWalkableTraceScheme
    {
        public string GameId { get; private set; }
        public int ActorNr { get; private set; }
        public List<CellTemperatureScheme> Temperatures { get; set; } = new List<CellTemperatureScheme>();

        public CellTemperatureTracesScheme(string gameId, int actorNr)
        {
            GameId = gameId;
            ActorNr = actorNr;
        }
    }
}

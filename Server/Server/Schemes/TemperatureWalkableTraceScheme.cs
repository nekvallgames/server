using Plugin.Interfaces;
using System.Collections.Generic;

namespace Plugin.Schemes
{
    /// <summary>
    /// Схема, котра зберігає в собі температурний слід переміщення юнітів актора
    /// </summary>
    public class TemperatureWalkableTraceScheme : ITemperatureWalkableTraceScheme
    {
        public string GameId { get; private set; }
        public int ActorNr { get; private set; }
        public List<CellTemperatureScheme> Temperatures { get; set; } = new List<CellTemperatureScheme>();

        public TemperatureWalkableTraceScheme(string gameId, int actorNr)
        {
            GameId = gameId;
            ActorNr = actorNr;
        }
    }
}

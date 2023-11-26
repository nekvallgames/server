namespace Plugin.Schemes
{
    public class ActorStepScheme
    {
        public string GameId { get; }
        public int OwnerActorId { get; }

        /// <summary>
        /// Схема со всеми действиями игрока
        /// Куча компонентов, которые разсортированы по спискам
        /// </summary>
        public StepScheme stepScheme;

        public ActorStepScheme(string gameId, int actorId)
        {
            GameId = gameId;
            OwnerActorId = actorId;
        }

        public int GetNextGroupIndex(int stepNumber)
        {
            я тут закінчив

            return stepScheme.syncUnitId.Count;
        }
    }
}

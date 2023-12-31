﻿using System.Collections.Generic;

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
        public List<StepScheme> steps = new List<StepScheme>();

        public ActorStepScheme(string gameId, int actorId)
        {
            GameId = gameId;
            OwnerActorId = actorId;
        }

        public int GetNextGroupIndex(int stepNumber)
        {
            if (steps.Count == 0){
                steps.Add(new StepScheme());
            }

            while (steps.Count <= stepNumber){
                steps.Add(new StepScheme());
            }

            return steps[stepNumber].syncUnitId.Count;
        }
    }
}

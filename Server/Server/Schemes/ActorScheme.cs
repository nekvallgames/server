﻿using Plugin.Interfaces;
using System.Collections.Generic;

namespace Plugin.Schemes
{
    public class ActorScheme : IActorScheme
    {
        public string GameId { get; private set; }
        public int ActorNr { get; private set; }
        public string ProfileId { get; private set; }
        public List<int> Deck { get; set; }
        public List<int> Levels { get; set; }
        public int Rating { get; set; }
        public bool IsLeft { get; set; }
        public bool IsAI { get; private set; }

        public ActorScheme(string gameId, int actorId, string profileId, bool isAI)
        {
            GameId = gameId;
            ActorNr = actorId;
            ProfileId = profileId;

            Rating = 0;
            Deck = new List<int>();
            Levels = new List<int>();
            IsLeft = false;
            IsAI = isAI;
        }
    }
}

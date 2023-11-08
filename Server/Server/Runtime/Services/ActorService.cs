using Plugin.Interfaces;
using Plugin.Models.Private;
using Plugin.Schemes;
using Plugin.Signals;
using System;
using System.Collections.Generic;

namespace Plugin.Runtime.Services
{
    public class ActorService
    {
        private ActorsPrivateModel _model;
        private SignalBus _signalBus;

        public ActorService(ActorsPrivateModel model, SignalBus signalBus)
        {
            _model = model;
            _signalBus = signalBus;
        }

        public ActorScheme CreateActor(string gameId, int actorNr, string profileId, bool isAI)
        {
            var actorScheme = new ActorScheme(gameId, actorNr, profileId, isAI);
            _model.Add(actorScheme);

            return actorScheme;
        }

        public void RemoveActor(string gameId, int actorNr)
        {
            IActorScheme actor = _model.Items.Find(x => x.GameId == gameId && x.ActorNr == actorNr);
            if (actor != null)
            {
                _model.Items.Remove(actor);
            }
        }

        /// <summary>
        /// Actor left from room
        /// </summary>
        public void ActorLeft(string gameId, int actorNr)
        {
            IActorScheme actor = _model.Items.Find(x => x.GameId == gameId && x.ActorNr == actorNr);
            if (actor != null)
            {
                actor.IsLeft = true;

                _signalBus.Fire(new ActorLeftSignal(actor));
            }
        }

        public void RemoveActorsInRoom(string gameId)
        {
            List<IActorScheme> actors = _model.Items.FindAll(x => x.GameId == gameId);
            foreach (IActorScheme actor in actors)
            {
                _model.Items.Remove(actor);
            }
        }

        public List<IActorScheme> GetActorsInRoom(string gameId)
        {
            return _model.Items.FindAll(x => x.GameId == gameId);
        }

        public IActorScheme GetRealActor(string gameId)
        {
            List<IActorScheme> actors = GetActorsInRoom(gameId);

            return actors.Find(x => !x.IsAI);
        }

        public IActorScheme GetAiActor(string gameId)
        {
            List<IActorScheme> actors = GetActorsInRoom(gameId);

            return actors.Find(x => x.IsAI);
        }
    }
}

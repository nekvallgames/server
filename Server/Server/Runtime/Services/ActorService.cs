using Plugin.Interfaces;
using Plugin.Models.Private;
using Plugin.Schemes;
using Plugin.Signals;
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

        public void CreateActor(string gameId, int actorNr, string profileId)
        {
            _model.Add(new ActorScheme(gameId, actorNr, profileId));
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
    }
}

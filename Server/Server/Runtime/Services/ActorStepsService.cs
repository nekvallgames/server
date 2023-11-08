using Plugin.Models.Private;
using Plugin.Schemes;
using System;
using System.Linq;

namespace Plugin.Runtime.Services
{
    public class ActorStepsService
    {
        private ActorStepsPrivateModel _model;

        public ActorStepsService(ActorStepsPrivateModel model)
        {
            _model = model;
        }

        public void CreateScheme(string gameId, int actorNr)
        {
            if (_model.Items.Any(x => x.GameId == gameId && x.OwnerActorId == actorNr))
            {
                return;
            }

            _model.Add(new ActorStepScheme(gameId, actorNr));
        }

        public void AddStepScheme(string gameId, int actorNr, StepScheme stepScheme)
        {
            if (!_model.Items.Any(x => x.GameId == gameId && x.OwnerActorId == actorNr))
                return;

            ActorStepScheme actorStepScheme = _model.Items.Find(x => x.GameId == gameId && x.OwnerActorId == actorNr);
            actorStepScheme.stepScheme = stepScheme;
        }

        public ActorStepScheme Get(string gameId, int actorNr)
        {
            if (!_model.Items.Any(x => x.GameId == gameId && x.OwnerActorId == actorNr))
                return null;

            return _model.Items.Find(x => x.GameId == gameId && x.OwnerActorId == actorNr);
        }
    }
}

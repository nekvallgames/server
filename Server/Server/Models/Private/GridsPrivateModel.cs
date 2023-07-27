using Plugin.Interfaces;
using Plugin.Runtime.Services;
using Plugin.Signals;
using Plugin.Templates;

namespace Plugin.Models.Private
{
    public class GridsPrivateModel : BaseModel<IGrid>, IPrivateModel
    {
        private SignalBus _signalBus;

        public GridsPrivateModel(SignalBus signalBus)
        {
            _signalBus = signalBus;
        }

        protected override void AfterAddHook(IGrid item)
        {
            _signalBus.Fire(new GridsPrivateModelSignal(item.GameId, item.OwnerActorNr, GridsPrivateModelSignal.StatusType.add));
        }

        protected override void AfterRemoveHook(IGrid item)
        {
            _signalBus.Fire(new GridsPrivateModelSignal(item.GameId, item.OwnerActorNr, GridsPrivateModelSignal.StatusType.remove));
        }
    }
}

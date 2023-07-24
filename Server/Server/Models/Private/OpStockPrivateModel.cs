using Plugin.Interfaces;
using Plugin.Runtime.Services;
using Plugin.Signals;
using Plugin.Templates;

namespace Plugin.Models.Private
{
    /// <summary>
    /// Модель із даними, котра зберігає в собі операції, котрі клієнти присилають to Game Server
    /// </summary>
    public class OpStockPrivateModel : BaseModel<IOpStockItem>, IPrivateModel
    {
        private SignalBus _signalBus;

        public OpStockPrivateModel(SignalBus signalBus)
        {
            _signalBus = signalBus;
        }

        protected override void AfterAddHook(IOpStockItem item)
        {
            _signalBus.Fire(new OpStockPrivateModelSignal(item.GameId,
                                                          item.ActorId, 
                                                          item.OpCode, 
                                                          OpStockPrivateModelSignal.StatusType.add));
        }

        protected override void AfterRemoveHook(IOpStockItem item) 
        {
            _signalBus.Fire(new OpStockPrivateModelSignal(item.GameId,
                                                          item.ActorId,
                                                          item.OpCode,
                                                          OpStockPrivateModelSignal.StatusType.remove));
        }
    }
}

using Photon.Hive.Plugin;
using Plugin.Signals;
using Plugin.Tools;
using System.Collections.Generic;

namespace Plugin.Runtime.Services
{
    /// <summary>
    /// Сервис, который занимается тем, что в момент, когда игрок изменит своего VIP,
    /// то об этом нужно уведомить противника.
    ///
    /// В игровом состоянии локальный игрок может изменять
    /// своего VIP безконечное количество раз, сколько хочет.
    /// Но отправить уведомление об изменении VIP, может только 1 раз. 
    /// А работа сервера, это сообщение перенаправить противнику.
    /// </summary>
    public class NotificationChangeVipService
    {
        private OpStockService _opStockService;
        private HostsService _hostsService;

        public NotificationChangeVipService(HostsService hostsService, OpStockService opStockService, SignalBus signalBus)
        {
            _opStockService = opStockService;
            _hostsService = hostsService;

            signalBus.Subscrible<OpStockPrivateModelSignal>(OnOpStockModelChange);
        }

        private void OnOpStockModelChange(OpStockPrivateModelSignal signalData)
        {
            if (signalData.OpCode == OperationCode.changeVip 
                && signalData.Status == OpStockPrivateModelSignal.StatusType.add)
            {
                IPluginHost plugin = _hostsService.Get(signalData.GameId);
                IList<IActor> roomActors = _hostsService.GetActors(plugin.GameId);

                foreach ( IActor actor in roomActors)
                {
                    if (_opStockService.HasOp(plugin.GameId, actor.ActorNr, OperationCode.changeVip))
                    {
                        // На склад операцій упала операція OperationCode.changeVip
            
                        _opStockService.TakeOp(plugin.GameId, actor.ActorNr, signalData.OpCode);  // видалити операцію зі складу
            
                        // Створити массив із акторів, кому ми відправимо цей івент
                        var actors = ((List<IActor>)roomActors).FindAll(x => x.ActorNr != actor.ActorNr);
                        var actorsId = new List<int>();
                        foreach (IActor __actor in actors){
                            actorsId.Add(__actor.ActorNr);
                        }
            
                        // Відправити всім участникам цей івент
                        plugin.BroadcastEvent(actorsId,
                                             signalData.ActorId,
                                             OperationCode.changeVip,
                                             null,
                                             CacheOperations.DoNotCache); // не кэшировать сообщение
                    } 
                }
            }
        }
    }
}

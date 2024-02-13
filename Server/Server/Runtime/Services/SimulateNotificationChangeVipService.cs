using Photon.Hive.Plugin;
using Plugin.Tools;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Plugin.Runtime.Services
{
    /// <summary>
    /// Сервіс, котрий імітує факт, що гравець противник змінив свого VIP
    /// Це потрібно для AI та режиму PVE. 
    /// </summary>
    public class SimulateNotificationChangeVipService
    {
        private HostsService _hostsService;
        private ActorService _actorService;

        private const int MIN_WAIT_BEFORE_SHOW = 15000;    // 15 sec
        private const int MAX_WAIT_BEFORE_SHOW = 20000;   // 20 sec

        public SimulateNotificationChangeVipService(ActorService actorService, HostsService hostsService)
        {
            _actorService = actorService;
            _hostsService = hostsService;
        }

        public async void Execute(string gameId)
        {
            Random random = new Random();

            await Task.Delay(random.Next(MIN_WAIT_BEFORE_SHOW, MAX_WAIT_BEFORE_SHOW));

            // Відправити всім участникам цей івент
            int targetActorNr = _actorService.GetRealActor(gameId).ActorNr;
            IPluginHost plugin = _hostsService.Get(gameId);

            plugin.BroadcastEvent(ReciverGroup.All,
                                 targetActorNr,
                                 0,
                                 OperationCode.changeVip,
                                 null,
                                 CacheOperations.DoNotCache); // не кэшировать сообщение
        }
    }
}

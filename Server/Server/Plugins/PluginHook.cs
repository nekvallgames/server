using Photon.Hive.Plugin;
using Plugin.Installers;
using Plugin.Runtime.Services;
using Plugin.Runtime.Services.PlotStates;
using Plugin.Schemes;
using Plugin.Signals;
using System.Collections.Generic;

namespace Plugin.Plugins
{
    /// <summary>
    /// Клас котрий буде перехвачувати сигнали Game Server
    /// </summary>
    public abstract class PluginHook : PluginBase
    {
        protected IPluginHost host;
        /// <summary>
        /// Сервіс, котрий зберігає в собі стейти для роботи ігрового сценарія
        /// </summary>
        protected PlotStatesService plotService;
        /// <summary>
        /// Сервіс, котрий зберігає модель із даними сюжета, для поточної кімнати
        /// </summary>
        protected PlotsModelService plotsModelService;

        private OpStockService _opStockService;
        private SignalBus _signalBus;
        private ActorService _actorService;
        private UnitsService _unitsService;
        private GridService _gridService;

        public override bool SetupInstance(IPluginHost host, Dictionary<string, string> config, out string errorMsg)
        {
            this.host = host;
            
            var gameInstaller = GameInstaller.GetInstance();

            _opStockService = gameInstaller.opStockService;
            _signalBus = gameInstaller.signalBus;
            _actorService = gameInstaller.actorService;
            _unitsService = gameInstaller.unitsService;
            _gridService = gameInstaller.gridService;

            plotsModelService = gameInstaller.plotsModelService;

            gameInstaller.hostsService.Add(host);

            PluginImplementation();

            return base.SetupInstance(host, config, out errorMsg);
        }

        /// <summary>
        /// Реалізувати логіку плагіна
        /// </summary>
        protected abstract void PluginImplementation();

        /// <summary>
        /// Игрок на стороне клиента отправил ивент "Создать комнату" и при успешном создании комнаты на 
        /// стороне GameServer выполнится текущий метод
        /// </summary>
        public override void OnCreateGame(ICreateGameCallInfo info)
        {
            _actorService.CreateActor(host.GameId,
                                      !info.IsJoin ? 1 : info.Request.ActorNr,
                                      info.Request.ActorProperties["id"].ToString());

            _signalBus.Fire(new HostsPrivateModelSignal(host.GameId, HostsPrivateModelSignal.StatusType.change));

            info.Continue();
        }

        /// <summary>
        /// Игрок на стороне клиента отправил ивент "Присоединится к текущей комнате" и при успешном 
        /// присоеденении к комнате на стороне GameServer выполнится текущий метод
        /// </summary>
        public override void OnJoin(IJoinGameCallInfo info)
        {            
            _actorService.CreateActor(host.GameId,
                                      info.ActorNr,
                                      info.Request.ActorProperties["id"].ToString());

            _signalBus.Fire(new HostsPrivateModelSignal(host.GameId, HostsPrivateModelSignal.StatusType.change));

            info.Continue();
        }

        /// <summary>
        /// Игрок на стороне клиента отправил какой то ивент
        /// </summary>
        public override void OnRaiseEvent(IRaiseEventCallInfo info)
        {
            _opStockService.Add(new OpStockItem(host.GameId,
                                                info.ActorNr,
                                                info.Request.EvCode,
                                                info.Request.Data));

            info.Continue();
        }

        /// <summary>
        /// Игрок покинул игровую комнаты на стороне Game Server
        /// </summary>
        public override void OnLeave(ILeaveGameCallInfo info)
        {
            _actorService.RemoveActor(host.GameId, info.ActorNr);

            info.Continue();
        }

        /// <summary>
        /// Виконається при закритті кімнати
        /// </summary>
        public override void BeforeCloseGame(IBeforeCloseGameCallInfo info)
        {
            // Видалити модель із даними сюжета, котра була створена для поточної кімнати
            plotsModelService.RemoveIfExist(host.GameId);
            // Видалити всіх юнітів, котрі були створені для ігрової кімнати
            _unitsService.RemoveAllIfExist(host.GameId);
            // Видалити всі ігрові сітки, котрі були створені для поточної кімнати
            _gridService.RemoveAllIfExist(host.GameId);
            // Видалити всіх юнітів, котри належать поточній кімнаті
            _actorService.RemoveActorsInRoom(host.GameId);

            info.Continue();
        }
    }
}

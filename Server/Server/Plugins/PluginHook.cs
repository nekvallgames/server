using Photon.Hive.Plugin;
using Plugin.Installers;
using Plugin.Interfaces;
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
        private GameService _gameService;

        public override bool SetupInstance(IPluginHost host, Dictionary<string, string> config, out string errorMsg)
        {
            this.host = host;
            
            var gameInstaller = GameInstaller.GetInstance();

            _opStockService = gameInstaller.opStockService;
            _signalBus = gameInstaller.signalBus;
            _actorService = gameInstaller.actorService;
            _gameService = gameInstaller.gameService;

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
                                      info.Request.ActorProperties["id"].ToString(), 
                                      false);

            _signalBus.Fire(new HostsPrivateModelSignal(host.GameId, HostsPrivateModelSignal.StatusType.change));

            info.Continue();
        }

        /// <summary>
        /// Игрок на стороне клиента отправил ивент "Присоединится к текущей комнате" и при успешном 
        /// присоеденении к комнате на стороне GameServer выполнится текущий метод
        /// </summary>
        public override void OnJoin(IJoinGameCallInfo info)
        {
            IPlotModelScheme plotModelScheme = plotsModelService.Get(host.GameId);
            if (plotModelScheme == null || !plotModelScheme.IsRoomVisible)
            {
                info.Fail();
                return;
            }

            _actorService.CreateActor(host.GameId,
                                      info.ActorNr,
                                      info.Request.ActorProperties["id"].ToString(), 
                                      false);

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
            _actorService.ActorLeft(host.GameId, info.ActorNr);

            info.Continue();
        }

        /// <summary>
        /// Виконається при закритті кімнати
        /// </summary>
        public override void BeforeCloseGame(IBeforeCloseGameCallInfo info)
        {
            _gameService.CloseRoom(host.GameId);
            plotService.Dispose();

            info.Continue();
        }
    }
}

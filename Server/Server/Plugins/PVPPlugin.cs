using Photon.Hive.Plugin;
using Plugin.Interfaces;
using Plugin.Runtime.Services.PlotStates;
using Plugin.Runtime.Services.PlotStates.States;
using Plugin.Schemes;

namespace Plugin.Plugins.PVP
{
    /// <summary>
    /// Плагин для режиму PVP
    /// </summary>
    public class PVPPlugin : PluginHook
    {
        public const string NAME = "PVPPlugin";

        /// <summary>
        /// Имя плагина. Что бы текущий плагин выполнял логику созданой комнаты, 
        /// нужно имя плагина передать в параметрах, при создании комнаты
        /// </summary>
        public override string Name => NAME; // anything other than "Default" or "ErrorPlugin"

        /// <summary>
        /// Реалізувати логіку плагіна
        /// </summary>
        protected override void PluginImplementation()
        {
            // Для поточної ігрової кімнати створити стейт машину із стейтами,
            // котрі потрібні для ігрового режиму 
            plotService = new PlotStatesService();
            plotService.Add(new IState[] { new AccumulateState(plotService, host, 2, SyncBackendState.NAME),
                                           new SyncBackendState(plotService, host, PrepareRoomState.NAME),
                                           new PrepareRoomState(plotService, host, StartGameState.NAME),
                                           new StartGameState(plotService, host, WaitStepResult.NAME),
                                           
                                           new WaitStepResult(plotService, host, 2, SyncState.NAME),
                                           new SyncState(plotService, host, WaitStepResult.NAME)
                                          });

            // запустити ігровий сценарій
            plotService.ChangeState(AccumulateState.NAME);
        }

        /// <summary>
        /// Игрок на стороне клиента отправил ивент "Создать комнату" и при успешном создании комнаты на 
        /// стороне GameServer выполнится текущий метод
        /// </summary>
        public override void OnCreateGame(ICreateGameCallInfo info)
        {
            base.OnCreateGame(info);

            plotsModelService.RemoveIfExist(host.GameId);
            plotsModelService.Add(new PVPPlotModelScheme(host.GameId));
        }
    }
}

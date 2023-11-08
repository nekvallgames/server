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
            plotService.Add(new IPlotState[] { 
                                            new AccumulateState(plotService, host, 2, SyncBackendState.NAME),
                                            new SyncBackendState(plotService, host, PrepareRoomState.NAME),
                                            new PrepareRoomState(plotService, host, 2, StartGameState.NAME),
                                            new StartGameState(plotService, host, WaitStepResultState.NAME),
                                                            
                                            new WaitStepResultState(plotService, host, 2, PvpExecuteStepsState.NAME),
                                            new PvpExecuteStepsState(plotService, host, PvpResultState.NAME),
                                            new PvpResultState(plotService, host, SyncProgressState.NAME),
                                            new SyncProgressState(plotService, host, SyncStepsState.NAME),
                                            new SyncStepsState(plotService, host, StopRoomState.NAME),
                                            new StopRoomState(plotService, host, WaitStepResultState.NAME)
                                          });
        }

        /// <summary>
        /// Игрок на стороне клиента отправил ивент "Создать комнату" и при успешном создании комнаты на 
        /// стороне GameServer выполнится текущий метод
        /// </summary>
        public override void OnCreateGame(ICreateGameCallInfo info)
        {
            base.OnCreateGame(info);

            plotsModelService.RemoveIfExist(host.GameId);
            plotsModelService.Add(new PvpPlotModelScheme(host.GameId));


            foreach (IPlotState state in plotService.States)
                state.Initialize();

            // запустити ігровий сценарій
            plotService.ChangeState(AccumulateState.NAME);
        }
    }
}

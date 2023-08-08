using Photon.Hive.Plugin;
using Plugin.Installers;
using Plugin.Interfaces;

namespace Plugin.Runtime.Services.PlotStates.States
{
    /// <summary>
    /// Стейт, в котрому потрібно запинити стейт машину ігрових стейтів, якщо гра закінчилася
    /// </summary>
    public class StopRoomState : BasePlotState, IState
    {
        public const string NAME = "StopRoomState";
        public string Name => NAME;

        public StopRoomState(PlotStatesService plotStatesService,
                                IPluginHost host,
                                string nextState) : base(plotStatesService, host, nextState)
        {
            var gameInstaller = GameInstaller.GetInstance();
        }

        public void EnterState()
        {
            plotStatesService.ChangeState(nextState);
        }

        public void ExitState()
        {

        }
    }
}

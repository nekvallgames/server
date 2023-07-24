using Photon.Hive.Plugin;

namespace Plugin.Runtime.Services.PlotStates.States
{
    /// <summary>
    /// Базовий клас для всіх стейтів стейт машини PlotStateService
    /// </summary>
    public abstract class BasePlotState
    {
        protected PlotStatesService plotStatesService;
        protected IPluginHost host;

        public BasePlotState(PlotStatesService plotStatesService, IPluginHost host)
        {
            this.plotStatesService = plotStatesService;
            this.host = host;
        }
    }
}

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
        /// <summary>
        /// Наступний стейт, в котрий перейдемо після поточного стейту
        /// </summary>
        protected string nextState;

        public BasePlotState(PlotStatesService plotStatesService, IPluginHost host, string nextState)
        {
            this.plotStatesService = plotStatesService;
            this.host = host;
            this.nextState = nextState;
        }
    }
}

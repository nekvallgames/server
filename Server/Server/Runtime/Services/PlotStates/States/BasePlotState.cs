using Photon.Hive.Plugin;
using Plugin.Interfaces;

namespace Plugin.Runtime.Services.PlotStates.States
{
    /// <summary>
    /// Базовий клас для всіх стейтів стейт машини PlotStateService
    /// </summary>
    public abstract class BasePlotState : IPlotState
    {
        protected PlotStatesService plotStatesService;
        protected IPluginHost host;

        public abstract string Name { get; }

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

        public virtual void Initialize()
        {
            
        }

        public virtual void EnterState()
        {
           
        }

        public virtual void ExitState()
        {
            
        }
    }
}

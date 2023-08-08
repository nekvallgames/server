using Photon.Hive.Plugin;
using Plugin.Installers;
using Plugin.Interfaces;

namespace Plugin.Runtime.Services.PlotStates.States
{
    /// <summary>
    /// Стейт, в котрому ми перевіримо модель із даними, 
    /// і якщо потрібно начислити гравцям нагороду, то начисляємо
    /// </summary>
    public class GiveRewardsState : BasePlotState, IState
    {
        public const string NAME = "GiveRewardsState";
        public string Name => NAME;

        public GiveRewardsState(PlotStatesService plotStatesService,
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

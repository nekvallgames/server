using Photon.Hive.Plugin;
using Plugin.Installers;

namespace Plugin.Runtime.Services.PlotStates.States
{
    /// <summary>
    /// Стейт, в котрому ми перевіримо модель із даними, 
    /// і якщо потрібно начислити гравцям нагороду, то начисляємо
    /// </summary>
    public class GiveRewardsState : BasePlotState
    {
        public const string NAME = "GiveRewardsState";
        public override string Name => NAME;

        public GiveRewardsState(PlotStatesService plotStatesService,
                                IPluginHost host,
                                string nextState) : base(plotStatesService, host, nextState)
        {
            var gameInstaller = GameInstaller.GetInstance();
        }

        public override void EnterState()
        {
            plotStatesService.ChangeState(nextState);
        }
    }
}

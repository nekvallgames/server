using Plugin.Interfaces;
using System.Diagnostics;
using System.Linq;

namespace Plugin.Runtime.Services.PlotStates
{
    /// <summary>
    /// Сервіс, за допомогою котрого будемо переключати стейти ігрового сценарія
    /// </summary>
    public class PlotStatesService
    {
        public IPlotState[] States { get; private set; }

        /// <summary>
        /// Текущее игровое состояние
        /// </summary>
        public IState CurrState { get; private set; }

        public void Add(IPlotState[] states)
        {
            States = states;
        }

        public void ChangeState(string nextStateName)
        {
            if (!States.Any(x => x.Name == nextStateName))
            {
                Debug.Fail($"PlotStatesService :: ChangeState() I don't know this state {nextStateName}");
                return;
            }

            if (CurrState != null){
                CurrState.ExitState();
            }

            CurrState = States.First(x => x.Name == nextStateName);

            CurrState.EnterState();
        }
    }
}

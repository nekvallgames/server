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
        private IState[] _states;

        /// <summary>
        /// Текущее игровое состояние
        /// </summary>
        public IState CurrState { get; private set; }

        public PlotStatesService(IState[] states)
        {
            _states = states;
        }

        public void ChangeState(string nextStateName)
        {
            if (!_states.Any(x => x.Name == nextStateName))
            {
                Debug.Fail($"PlotStatesService :: ChangeState() I don't know this state {nextStateName}");
                return;
            }

            if (CurrState != null){
                CurrState.ExitState();
            }

            CurrState = _states.First(x => x.Name == nextStateName);

            CurrState.EnterState();
        }
    }
}

using Plugin.Interfaces;
using System;
using System.Linq;

namespace Plugin.Runtime.Services.PlotMode
{
    public class PlotModeService
    {
        private ITask[] _modes;
        
        public PlotModeService(ITask[] modes)
        {
            _modes = modes;
        }

        public void ExecuteTask(string gameMode, Action onComplete, Action onFail)
        {
            ITask task = _modes.First(x => x.Name == gameMode);
            task.EnterTask(onComplete, onFail);
        }
    }
}

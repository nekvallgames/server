using Plugin.Interfaces;
using System;
using System.Linq;

namespace Plugin.Runtime.Services.PlotMode
{
    public class PlotModeService
    {
        private IMode[] _modes;
        
        public void Add(IMode[] modes)
        {
            _modes = modes;
        }

        public void ExecuteMode(int modeId, Action success)
        {
            IMode task = _modes.First(x => x.ModeId == modeId);
            task.ExecuteMode(success);
        }
    }
}

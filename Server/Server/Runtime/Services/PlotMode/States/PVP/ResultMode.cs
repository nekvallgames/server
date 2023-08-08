using Photon.Hive.Plugin;
using Plugin.Interfaces;
using Plugin.Schemes;
using System;

namespace Plugin.Runtime.Services.PlotMode.States.PVP
{
    /// <summary>
    /// В этом игровом этапе, отображаем выиграш или проиграш игрока локального игрока
    /// </summary>
    public class ResultMode : ITask
    {
        public const string NAME = "ResultMode";
        public string Name => NAME;

        private PlotModeService _plotModeService;
        private IPluginHost _host;
        private PVPPlotModelScheme _model;

        public ResultMode(PlotModeService plotModeService, IPluginHost host, PVPPlotModelScheme model)
        {
            _plotModeService = plotModeService;
            _host = host;
            _model = model;
        }

        public void EnterTask(Action taskIsDone, Action taskIsFail)
        {

        }

        public void ExitTask()
        {

        }
    }
}

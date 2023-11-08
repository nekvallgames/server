using Photon.Hive.Plugin;
using Plugin.Installers;
using Plugin.Interfaces;
using System;

namespace Plugin.Runtime.Services.PlotStates.States
{
    /// <summary>
    /// Стейт, в котрому, якщо гра закінчилася, синхронізувати прогресс гравця із базою данних
    /// </summary>
    public class SyncProgressState : BasePlotState
    {
        public const string NAME = "SyncProgressState";
        public override string Name => NAME;

        private PlotsModelService _plotsModelService;
        private ActorService _actorService;
        private SyncProgressService _syncProgressService;
        private GameService _gameService;

        private IPlotModelScheme _plotModel;

        public SyncProgressState(PlotStatesService plotStatesService,
                                IPluginHost host,
                                string nextState) : base(plotStatesService, host, nextState)
        {
            var gameInstaller = GameInstaller.GetInstance();

            _plotsModelService = gameInstaller.plotsModelService;
            _actorService = gameInstaller.actorService;
            _syncProgressService = gameInstaller.syncProgressService;
            _gameService = gameInstaller.gameService;
        }

        public override void EnterState()
        {
            _plotModel = _plotsModelService.Get(host.GameId);

            if (_plotModel.IsAbort)
                return;

            if (_plotModel.IsGameFinished)
            {
                SyncProgress(host.GameId, () => {
                    _gameService.TryToDisposeRoom(host.GameId);
                    plotStatesService.ChangeState(nextState);
                });
            }
            else
            {
                plotStatesService.ChangeState(nextState);
            }
        }

        private async void SyncProgress(string gameId, Action success)
        {
            await _gameService.SyncProgress(gameId);
            success?.Invoke();
        }
    }
}

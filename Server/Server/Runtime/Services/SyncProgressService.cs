using Plugin.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace Plugin.Runtime.Services
{
    public class SyncProgressService
    {
        private BackendBroadcastService _backendBroadcastService;
        private PlotsModelService _plotsModelService;
        private PlotPublicService _plotPublicService;

        public SyncProgressService(BackendBroadcastService backendBroadcastService, 
                                   PlotsModelService plotsModelService,
                                   PlotPublicService plotPublicService)
        {
            _backendBroadcastService = backendBroadcastService;
            _plotsModelService = plotsModelService;
            _plotPublicService = plotPublicService;
        }

        public async Task Sync(IActorScheme actor)
        {
            IPlotModelScheme plotModel = _plotsModelService.Get(actor.GameId);

            bool isWin = plotModel.WinnerActorsNr.Any(x => x == actor.ActorNr);

            int rating = isWin
                ? _plotPublicService.Data.IncreaseRating
                : -_plotPublicService.Data.DecreaseRating;

            await _backendBroadcastService.ChangeRating(actor, rating);
        }
    }
}

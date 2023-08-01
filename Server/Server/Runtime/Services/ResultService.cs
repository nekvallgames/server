using Plugin.Interfaces;
using System.Threading.Tasks;

namespace Plugin.Runtime.Services
{
    /// <summary>
    /// Сервіс, котрий начислить нагороду або програш вказаному гравцю
    /// </summary>
    public class ResultService
    {
        private const int RATING = 32;

        private BackendBroadcastService _backendBroadcastService;

        public ResultService(BackendBroadcastService backendBroadcastService)
        {
            _backendBroadcastService = backendBroadcastService;
        }

        public async Task<int> Win(IActorScheme actor)
        {
            return await _backendBroadcastService.ChangeRating(actor, RATING);
        }

        public async Task<int> Lose(IActorScheme actor)
        {
            return await _backendBroadcastService.ChangeRating(actor, -RATING);
        }
    }
}

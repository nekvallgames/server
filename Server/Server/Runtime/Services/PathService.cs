using Plugin.Runtime.Services.UnitsPath;

namespace Plugin.Runtime.Services
{
    /// <summary>
    /// Сервіс, котрий перерахую дані переміщення юнітів гравця
    /// </summary>
    public class PathService
    {
        private UnitsPathService _unitsPathService;
        private CellWalkableService _cellWalkableService;

        public PathService(UnitsPathService unitsPathService,
                            CellWalkableService cellWalkableService)
        {
            _unitsPathService = unitsPathService;
            _cellWalkableService = cellWalkableService;
        }

        public void Calculate(string gameId, int actorId)
        {
            _unitsPathService.CalculateAndSavePathForUnits(gameId, actorId);
            _cellWalkableService.Calculate(gameId, actorId);
        }
    }
}

using Plugin.Interfaces;
using Plugin.Models.Private;
using Plugin.Models.Public;
using Plugin.Schemes;
using Plugin.Tools;
using System.Collections.Generic;
using System.Linq;

namespace Plugin.Runtime.Services.UnitsPath
{
    /// <summary>
    /// Сервіс, который будет высчитывать путь, по которому юниты, которые принадлежат игроку, могут перемещатся
    /// То есть, этот класс хранит в себе данные, куда юниты могут пойти в состоянии перемещения юнитов сценария
    /// </summary>
    public class UnitsPathService
    {
        private NavigationMapPublicModel _navigationMapPublicModel;
        private ActorUnitsPathPrivateModel _actorUnitsPathPrivateModel;
        private GridService _gridService;
        private UnitsService _unitService;
        private FindPathWorker _findPathWorker;

        public UnitsPathService(NavigationMapPublicModel navigationMapPublicModel,
                                ActorUnitsPathPrivateModel actorUnitsPathPrivateModel,
                                GridService gridService,
                                UnitsService unitService)
        {
            _navigationMapPublicModel = navigationMapPublicModel;
            _actorUnitsPathPrivateModel = actorUnitsPathPrivateModel;
            _gridService = gridService;
            _unitService = unitService;

            // Класс помощник, который будет искать путь для каждого юнита
            // То есть, PathPlayerController хранит в себе юнитов и их высчитаные пути. 
            // А вот FindPathWorker все высчитывает
            _findPathWorker = new FindPathWorker(_navigationMapPublicModel, _gridService);
        }

        public void CreateScheme(string gameId, int actorNr)
        {
            if (_actorUnitsPathPrivateModel.Has(gameId, actorNr))
            {
                LogChannel.Log($"UnitsPathService :: CreateScheme() I can't create scheme for actorUnitsPathPrivateModel, because scheme already created for gameId = {gameId}, actorNr = {actorNr}.", LogChannel.Type.Error);
                return;
            }

            _actorUnitsPathPrivateModel.Add(new ActorUnitsPathPrivateScheme(gameId, actorNr));
        }

        /// <summary>
        /// Посчитать и сохранить все высчитаные пути, куда могут пойти юниты игрока
        /// Все данные хранить в UnitsPathPrivateModel
        /// </summary>
        public void CalculateAndSavePathForUnits(string gameId, int actorId)
        {
            if (!_actorUnitsPathPrivateModel.Has(gameId, actorId)){
                LogChannel.Log($"UnitsPathService :: CalculateAndSavePathForUnits() I can't find scheme in actorUnitsPathPrivateModel for gameId = {gameId}, actorId = {actorId}.", LogChannel.Type.Error);
                return;
            }

            ActorUnitsPathPrivateScheme actorUnitsPathPrivateScheme = _actorUnitsPathPrivateModel.Get(gameId, actorId);

            // 1. Чистим старый список
            actorUnitsPathPrivateScheme.unitsPath.Clear();

            IGrid gridUnderUnit = _gridService.Get(gameId, actorId);

            // 2. Перебираем всех юнитов игрока, и высчитываем путь для каждого
            List<IUnit> units = _unitService.GetUnits(gameId, actorId);
            foreach (IUnit unit in units)
            {
                if (!(unit is IWalkableComponent)) { 
                    continue;
                }
                    
                if (unit.IsDead){
                    continue;  // с мертвыми не работаем!
                }

                Int2 positionOnGrid = unit.Position;

                UnitPathPrivateScheme unitPathScheme = CalculatePath(unit,
                                                                     positionOnGrid.x,
                                                                     positionOnGrid.y,
                                                                     gridUnderUnit);
                actorUnitsPathPrivateScheme.unitsPath.Add(unitPathScheme);
            }
        }

        private UnitPathPrivateScheme CalculatePath(IUnit unit, int positionOnGridW, int positionOnGridH, IGrid grid)
        {
            bool hasGodModeMovement = ((IWalkableComponent)unit).IsGodModeMovement;

            var path = new List<PathCellPrivateScheme>();

            if (hasGodModeMovement)
            {
                // У юнита свободное перемещение.
                // Найти на игровой сетке все свободные селлы, по которым юнит может перемещатся  
                path = _findPathWorker.GetAllMovementCells(unit,
                                                           positionOnGridW,
                                                           positionOnGridH,
                                                           grid);
            }
            else
            {
                // Получить зону перемещение, куда может переместиться текущий юнит
                path = _findPathWorker.CalculatePath(unit,
                                                     positionOnGridW,
                                                     positionOnGridH,
                                                     grid);
            }

            // Витянути із ігрової сітки селли, по котрим юніт зможе переміщатися
            var cells = new List<Cell>();
            foreach (PathCellPrivateScheme pathCell in path)
            {
                cells.Add(_gridService.GetCell(grid, pathCell.positionW, pathCell.positionH));
            }

            // Создать данные с путем для юнита
            var pathSchemeUnit = new UnitPathPrivateScheme(unit.OwnerActorNr,
                                                            unit.UnitId,
                                                            unit.InstanceId,
                                                            path,
                                                            cells);

            return pathSchemeUnit;
        }

        /// <summary>
        /// Получить селлы, по которым может перемещатся указаный юнит
        /// </summary>
        public UnitPathPrivateScheme GetPathUnit(string gameId, int actorId, int unitId, int instanceId)
        {
            List<UnitPathPrivateScheme> paths = _actorUnitsPathPrivateModel.Get(gameId, actorId).unitsPath;

            if (!paths.Any()){
                LogChannel.Log($"UnitsPathService :: GetPathUnit() paths don't has any path. ActorId = {actorId}, unitId = {unitId}, instanceId = {instanceId}", LogChannel.Type.Plot);
            }

            return paths.Find(x => x.UnitId == unitId && x.InstanceId == instanceId);
        }

        /// <summary>
        /// Просто посчитать путь и нигде не сохраняя
        /// Получить путь перемещения для юнита, но указав координаты,
        /// где он сейчас позиционируется на игровой сетке
        /// Это нужно, что бы показать игроку, где он будет находится,
        /// например в следующем игровом шаге
        /// </summary>
        public List<PathCellPrivateScheme> CalculateAndGetPathUnit(IUnit unit, int positionOnGridW, int positionOnGridH, IGrid grid)
        {
            // Получить зону перемещения, куда может переместиться текущий юнит
            return _findPathWorker.CalculatePath(unit, positionOnGridW, positionOnGridH, grid);
        }

        public void RemoveAllIfExist(string gameId)
        {
            List<ActorUnitsPathPrivateScheme> schemes = _actorUnitsPathPrivateModel.Items.FindAll(x => x.GameId == gameId);
            foreach (ActorUnitsPathPrivateScheme scheme in schemes)
            {
                _actorUnitsPathPrivateModel.Items.Remove(scheme);
            }
        }
    }
}

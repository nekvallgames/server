using Plugin.Interfaces;
using Plugin.OpComponents;
using Plugin.Runtime.Services.ExecuteAction;
using Plugin.Runtime.Services.UnitsPath;
using System.Collections.Generic;
using System.Diagnostics;

namespace Plugin.Runtime.Services.ExecuteOp.Executors
{
    /// <summary>
    /// Виконати операцію клієнта, котру він прислав на Game Server
    /// Выполнить действие игрока - переместить игрового юнита на игровой сетке
    /// </summary>
    public class ExecuteOpGroupPositionOnGrid : IExecuteOpGroup
    {
        private UnitsService _unitsService;
        private MoveService _moveService;
        private UnitIdOpComponent _unitIdOpComponent;
        private PositionOnGridOpComponent _positionOnGridOpComponent;
        private UnitsPathService _unitsPathService;
        private PlotsModelService _plotsModelService;
        private CellWalkableService _cellWalkableService;
        private GridService _gridService;

        public ExecuteOpGroupPositionOnGrid(UnitsService unitsService, 
                                            MoveService moveService, 
                                            UnitsPathService unitsPathService,
                                            PlotsModelService plotsModelService,
                                            CellWalkableService cellWalkableService,
                                            GridService gridService)
        {
            _unitsService = unitsService;
            _moveService = moveService;
            _unitsPathService = unitsPathService;
            _plotsModelService = plotsModelService;
            _cellWalkableService = cellWalkableService;
            _gridService = gridService;
        }

        /// <summary>
        /// Может ли текущий класс выполнить действие игрока?
        /// </summary>
        public bool CanExecute(List<ISyncComponent> componentsGroup)
        {
            foreach (ISyncComponent component in componentsGroup){
                if (component.GetType() == typeof(PositionOnGridOpComponent)){
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Выполнить действие игрока. А именно позиционировать юнита на игровой сетке
        /// </summary>
        public void Execute(string gameId, int playerActorNr, List<ISyncComponent> componentsGroup)
        {
            // Вытаскиваем нужные нам компоненты из списка
            Parce(componentsGroup);

            // Вытащить юнита, к которому будем применять перемещение
            IUnit unit = _unitsService.GetUnit(gameId, playerActorNr, _unitIdOpComponent.UnitId, _unitIdOpComponent.UnitInstance);

            if (unit == null){
                Debug.Fail($"ExecuteOpGroupService :: ExecuteOpPositionOnGrid() playerActorNr = {playerActorNr}, unitID = {_unitIdOpComponent.UnitId}, instanceID = {_unitIdOpComponent.UnitInstance}, I don't find this unit for execute actions");
                return;
            }

            bool canStay = true;
            int positionW = _positionOnGridOpComponent.w;
            int positionH = _positionOnGridOpComponent.h;

            IPlotModelScheme plotModel = _plotsModelService.Get(gameId);

            if (plotModel.IsNeedToCheckOnCorrectPosition)
            {
                _cellWalkableService.Calculate(unit);

                canStay = _cellWalkableService.CanStayHere(unit, positionW, positionH);
            }

            if (!canStay)
            {
                positionW = unit.Position.x;
                positionH = unit.Position.y;
            }

            // Очистити сели, на котрих стояв юніт
            IGrid grid = _gridService.Get(gameId, playerActorNr);
            _gridService.FreeArea(unit, grid, unit.Position.x, unit.Position.y);

            // Переместить юнита в указаную позицию
            _moveService.PositionOnGrid(unit, positionW, positionH);

            // Заняти сели, на котрих став юніт
            _gridService.BusyArea(unit, grid, unit.Position.x, unit.Position.y);
        }

        /// <summary>
        /// Распарсить входящие данные
        /// </summary>
        private void Parce(List<ISyncComponent> componentsGroup)
        {
            foreach (ISyncComponent component in componentsGroup)
            {
                if (component.GetType() == typeof(PositionOnGridOpComponent)){
                    _positionOnGridOpComponent = (PositionOnGridOpComponent)component;
                }
                else
                    if (component.GetType() == typeof(UnitIdOpComponent)){
                        _unitIdOpComponent = (UnitIdOpComponent)component;
                    }
            }
        }
    }
}

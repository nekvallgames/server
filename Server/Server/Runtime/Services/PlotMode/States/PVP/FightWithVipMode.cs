using Photon.Hive.Plugin;
using Plugin.Interfaces;
using Plugin.Interfaces.Actions;
using Plugin.Schemes;
using Plugin.Tools;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Plugin.Runtime.Services.PlotMode.States.PVP
{
    /// <summary>
    /// В текущем игровом этапе, мы проверяем, мертв ли VIP юнит у игроков.
    /// Также проверяем, если оба VIP-а мертвы, с двух сторон, и у каждого юнита есть еще кем играть,
    /// то переназначить VIP-ов на других юнитов
    /// </summary>
    public class FightWithVipMode : IMode
    {
        public const Enums.PVPMode Mode = Enums.PVPMode.FightWithVip;
        public int ModeId => (int)Mode;

        private PlotModeService _plotModeService;
        private IPluginHost _host;
        private PVPPlotModelScheme _model;
        private ActorService _actorService;
        private UnitsService _unitsService;
        private Action _success;
        private List<IActorScheme> _actors;

        /// <summary>
        /// Список с дополнительными проверками для текущего этапа игры
        /// </summary>
        private List<ICondition> _additionalConditions = new List<ICondition>();

        public FightWithVipMode(PlotModeService plotModeService, 
                                IPluginHost host, 
                                PVPPlotModelScheme model, 
                                ActorService actorService,
                                UnitsService unitsService)
        {
            _plotModeService = plotModeService;
            _host = host;
            _model = model;
            _actorService = actorService;
            _unitsService = unitsService;
        }

        public void ExecuteMode(Action success)
        {
            LogChannel.Log("PlotModeService :: FightWithVipMode :: EnterTask()", LogChannel.Type.Plot);

            _success = success;
            _model.GameMode = (int)Mode;
            _model.IsNeedToCheckOnCorrectPosition = true;

            _actors = _actorService.GetActorsInRoom(_host.GameId);

            if (!_model.WasPreparedVipMode)
            {
                CreateAdditionalConditions();
                ExecuteAdditionalCondition();
                PrepareVipMode();
                _model.WasPreparedVipMode = true;

                _success?.Invoke();
            }
            else
            {
                CheckingConditionMode();
            }
        }

        private void CheckingConditionMode()
        {
            // Проверяем условия выиграша или проиграша
            bool isAliveVipActor0 = _unitsService.VipIsAlive(_host.GameId, _actors[0].ActorNr)
                ? true
                : false;
            bool isAliveVipActor1 = _unitsService.VipIsAlive(_host.GameId, _actors[1].ActorNr)
                ? true
                : false;

            if (isAliveVipActor0 && isAliveVipActor1)
            {
                // Vip-ы игроков живы. Но! перед тем, как продолжить игру,
                // проверяем, если осталось только по одному юниту
                // из каждой из сторон, то активировать режим дуэль
                if (_unitsService.GetAliveUnitsCountWhoWillBeAbleToVip(_host.GameId, _actors[0].ActorNr) == 1 &&
                    _unitsService.GetAliveUnitsCountWhoWillBeAbleToVip(_host.GameId, _actors[1].ActorNr) == 1)
                {
                    _plotModeService.ExecuteMode((int)DuelMode.Mode, _success);
                    return;
                }

                // Vip-ы игроков живы. Продолжаем игру далее
                ExecuteAdditionalCondition();
                _success?.Invoke();
                return;
            }

            _plotModeService.ExecuteMode((int)ResultMode.Mode, _success);
        }

        /// <summary>
        /// Сделать VIP-ом юнитов c обеих сторон
        /// </summary>
        private void PrepareVipMode()
        {
            foreach (ActorScheme actor in _actors)
            {
                var candidatesForVip = new List<IUnit>();
                _unitsService.GetAliveUnitsWhoWillBeAbleToVip(_host.GameId, actor.ActorNr, ref candidatesForVip);

                // Сделать VIP-ом по умолчанию
                _unitsService.MakeVip(candidatesForVip[0], true);
            }
        }

        /// <summary>
        /// Создать дополнительные проверки для текущего этапа игры
        /// </summary>
        private void CreateAdditionalConditions()
        {
            foreach (ActorScheme actor in _actors)
            {
                _additionalConditions.Add(new IncreaseDamageBuffAllTeam(_host.GameId, actor.ActorNr, _unitsService));
                _additionalConditions.Add(new IncreaseDamageLastUnit(_host.GameId, actor.ActorNr, _model, _unitsService));
            }
        }

        /// <summary>
        /// Условие на смерть випа не было выполенно.
        /// Продолжаем играть в текущем игровом этапе
        /// </summary>
        private void ExecuteAdditionalCondition()
        {
            // Выполнить список дополнительных условий
            foreach (ICondition additionalCondition in _additionalConditions)
            {
                additionalCondition.Execute();
            }
        }
    }

    /// <summary>
    /// Дополнительное проверка для игрового этапа FightWithVipStage
    /// В текущей проверке нужно увеличить кількість патронів для кожного юніта
    /// </summary>
    internal class IncreaseDamageBuffAllTeam : ICondition
    {
        private string _gameId;
        private int _actorNr;
        private UnitsService _unitsService;

        public IncreaseDamageBuffAllTeam(string gameId, int actorNr, UnitsService unitsService)
        {
            _gameId = gameId;
            _actorNr = actorNr;
            _unitsService = unitsService;
        }

        public void Execute()
        {
            var aliveUnits = new List<IUnit>();
            _unitsService.GetAliveUnits(_gameId, _actorNr, ref aliveUnits);

            foreach (IUnit unit in aliveUnits)
            {
                (unit as IDamageAction).OriginalActionCapacity++;
                (unit as IDamageAction).ReviveAction();
            }
        }
    }

    /// <summary>
    /// Дополнительное проверка для игрового этапа FightWithVipStage
    /// В текущей проверке нужно увеличить один раз урон юниту, если он остался последним в команде
    /// </summary>
    internal class IncreaseDamageLastUnit : ICondition
    {
        private string _gameId;
        private int _actorNr;
        private PVPPlotModelScheme _model;
        private UnitsService _unitsService;

        public IncreaseDamageLastUnit(string gameId, 
                                      int actorNr,  
                                      PVPPlotModelScheme model,
                                      UnitsService unitsService)
        {
            _gameId = gameId;
            _actorNr = actorNr;
            _model = model;
            _unitsService = unitsService;
        }

        public void Execute()
        {
            if (_model.HasLastUnitsHelp.Any(x => x == _actorNr))
                return;
                
            // Если у игрока остался 1 юнит, то дать этому последнему юниту последнюю помощь
            // (Помощь заключается в супер уроне и перемещении по всей карте)
            if (_unitsService.GetAliveUnitsCountWhoWillBeAbleToVip(_gameId, _actorNr) == 1)
            {
                // Бафф на урон
                IUnit unit = _unitsService.GetAnyAliveUnitWhoWillBeAbleToVip(_gameId, _actorNr);

                // Изменить путь перемещения юнита
                // _entityManager.AddComponentData(unit.UnitEntity, new GodModeMovementComponent { });

                _model.HasLastUnitsHelp.Add(_actorNr);
            }
        }
    }

    internal interface ICondition
    {
        void Execute();
    }
}

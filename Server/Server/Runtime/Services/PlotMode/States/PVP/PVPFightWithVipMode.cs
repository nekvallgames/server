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
    public class PVPFightWithVipMode : ITask
    {
        public const string NAME = "PVPFightWithVipMode";
        public string Name => NAME;

        private PlotModeService _plotModeService;
        private IPluginHost _host;
        private PVPPlotModelScheme _model;
        private ActorService _actorService;
        private UnitsService _unitsService;
        private Action _taskIsDone;
        private Action _taskIsFail;
        private List<IActorScheme> _actors;
        private bool _isCreatedAdditionalConditions;

        /// <summary>
        /// Список с дополнительными проверками для текущего этапа игры
        /// </summary>
        private List<ICondition> _additionalConditions = new List<ICondition>();

        public PVPFightWithVipMode(PlotModeService plotModeService, 
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

        public void EnterTask(Action taskIsDone, Action taskIsFail)
        {
            LogChannel.Log("PlotModeService :: PVPFightWithVipMode :: EnterTask()", LogChannel.Type.Plot);

            _actors = _actorService.GetActorsInRoom(_host.GameId);

            if (!_isCreatedAdditionalConditions)
            {
                _isCreatedAdditionalConditions = true;
                CreateAdditionalConditions();
            }

            _taskIsDone = taskIsDone;
            _taskIsFail = taskIsFail;
            _model.GameMode = Name;

            if (!_model.WasPreparedVipMode)
            {
                PrepareVipMode();
                _model.WasPreparedVipMode = true;
            }
            else
            {
                CheckingConditionMode();
            }
        }

        private void CheckingConditionMode()
        {
            // Проверяем условия выиграша или проиграша
            bool isAliveActor0 = _unitsService.VipIsAlive(_host.GameId, _actors[0].ActorNr)
                ? true
                : false;
            bool isAliveActor1 = _unitsService.VipIsAlive(_host.GameId, _actors[1].ActorNr)
                ? true
                : false;

            if (isAliveActor0 && isAliveActor1)
            {
                // Vip-ы игроков живы. Но! перед тем, как продолжить игру,
                // проверяем, если осталось только по одному юниту
                // из каждой из сторон, то активировать режим дуэль
                if (_unitsService.GetAliveUnitsCountWhoWillBeAbleToVip(_host.GameId, _actors[0].ActorNr) == 1 &&
                    _unitsService.GetAliveUnitsCountWhoWillBeAbleToVip(_host.GameId, _actors[1].ActorNr) == 1)
                {
                    _plotModeService.ExecuteTask(PVPDuelMode.NAME, _taskIsDone, _taskIsFail);
                    return;
                }

                // Vip-ы игроков живы. Продолжаем игру далее
                ExecuteAdditionalCondition();
                _taskIsDone?.Invoke();
                return;
            }

            if (!isAliveActor0 && !isAliveActor1)
            {
                // Vip-ы обоих игроков мертвы
                BothVipsAreDead();
                return;
            }

            _plotModeService.ExecuteTask(ResultMode.NAME, _taskIsDone, _taskIsFail);
        }

        /// <summary>
        /// Оба Vip-а мертвы. Проверяем, можем ли переназначить
        /// VIP на другого юнита у каждого из игроков
        ///
        /// То есть, у двоиз игроков vip юнит мертв. Если можем переназначить VIP одному игроку и другому игроку,
        /// то переназначаем, и продолжаем игру дальше
        /// </summary>
        private void BothVipsAreDead()
        {
            IUnit actor0HasAliveUnits = _unitsService.GetAnyAliveUnitWhoWillBeAbleToVip(_host.GameId, _actors[0].ActorNr);
            IUnit actor1HasAliveUnits = _unitsService.GetAnyAliveUnitWhoWillBeAbleToVip(_host.GameId, _actors[1].ActorNr);

            if (actor0HasAliveUnits == null && actor1HasAliveUnits == null)
            {
                // Оба VIP-а мертвы, и дополнительных юнитов с обеих сторон нет.
                // Нужно перейти в состояния дуэли
                _plotModeService.ExecuteTask(PVPDuelMode.NAME, _taskIsDone, _taskIsFail);
                return;
            }

            if (actor0HasAliveUnits != null && actor1HasAliveUnits != null)
            {
                // Оба VIP-а мертвы, но игроки имеют еще юнитов в команде.
                // Назначить этих юнитов VIP-ами
                // RemoveVipFromDeadUnits();
                PrepareVipMode();
                return;
            }

            if (actor0HasAliveUnits != null || actor1HasAliveUnits != null)
            {
                // Оба VIP-а мертвы, но с одной стороны есть еще выжившие юниты.
                // То есть, у одного игрока мертвы все юниты, а у другого игрока еще есть живой юнит
                _plotModeService.ExecuteTask(ResultMode.NAME, _taskIsDone, _taskIsFail);
                return;
            }
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
                _unitsService.MakeVip(candidatesForVip[0]);
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

        public void ExitTask()
        {

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
                (unit as IDamageAction).OriginalDamageCapacity++;
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

using Photon.Hive.Plugin;
using Plugin.Interfaces;
using Plugin.Interfaces.Actions;
using Plugin.Interfaces.UnitComponents;
using Plugin.Schemes;
using Plugin.Tools;
using System;
using System.Collections.Generic;

namespace Plugin.Runtime.Services.PlotMode.States.PVP
{
    public class DuelMode : IMode
    {
        public const Enums.PVPMode Mode = Enums.PVPMode.Duel;
        public int ModeId => (int)Mode;

        private PlotModeService _plotModeService;
        private IPluginHost _host;
        private PvpPlotModelScheme _model;
        private ActorService _actorService;
        private UnitsService _unitsService;

        // id юнітів гравців, котрі приймають участь в ігровому режимі дуєль
        private IUnit _actor0DuelUnit;
        private IUnit _actor1DuelUnit;

        private List<IActorScheme> _actors;

        public DuelMode(PlotModeService plotModeService, 
                        IPluginHost host, 
                        PvpPlotModelScheme model, 
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
            LogChannel.Log("PlotModeService :: DuelMode :: EnterTask()", LogChannel.Type.Plot);
            _model.GameMode = (int)Mode;
            _model.IsNeedToCheckOnCorrectPosition = true;

            _actors = _actorService.GetActorsInRoom(_host.GameId);

            if (!_model.WasPreparedDuel)
            {
                _model.WasPreparedDuel = true;

                _actor0DuelUnit = RegisterDuelUnit(_host.GameId, _actors[0].ActorNr);
                (_actor0DuelUnit as IVipComponent).IsVip = true;

                _actor1DuelUnit = RegisterDuelUnit(_host.GameId, _actors[1].ActorNr);
                (_actor1DuelUnit as IVipComponent).IsVip = true;

                // 9. Востановити юнітів, котрі будуть приймати участь в дуєлі
                ReviveUnit(_actor0DuelUnit);
                ReviveUnit(_actor1DuelUnit);

                foreach (ActorScheme actor in _actors)
                {
                    _unitsService.RemoveAllMedicHealing(_host.GameId, actor.ActorNr);
                }

                ChangeWayForDuel(_actor0DuelUnit);
                ChangeWayForDuel(_actor1DuelUnit);

                // Перебрати юнітів гравців, і відключити режим GodMode для переміщення по всій ігровій сітці
                if (_actor0DuelUnit is IWalkableComponent){
                    (_actor0DuelUnit as IWalkableComponent).IsGodModeMovement = false;
                }
                if (_actor1DuelUnit is IWalkableComponent){
                    (_actor1DuelUnit as IWalkableComponent).IsGodModeMovement = false;
                }
            }
            else
            {
                // Проверяем условия выиграша или проиграша
                bool IsAliveVipActor0 = _unitsService.VipIsAlive(_host.GameId, _actors[0].ActorNr) 
                    ? true 
                    : false;
                
                bool IsAliveVipActor1 = _unitsService.VipIsAlive(_host.GameId, _actors[1].ActorNr)
                    ? true
                    : false;

                if (IsAliveVipActor0 && IsAliveVipActor1)
                {
                    // Vip-ы игроков живы. Продолжаем игру далее
                    success?.Invoke();
                    return;
                }

                if (!IsAliveVipActor0 && !IsAliveVipActor1)
                {
                    // Vip-ы обоих игроков мертвы
                    BothVipsAreDead();
                    success?.Invoke();
                    return;
                }

                // Изменить игровой этап
                _plotModeService.ExecuteMode((int)ResultMode.Mode, success);
                return;
            }

            success?.Invoke();
        }

        /// <summary>
        /// Зареєструвати юніта, котрий буде приймати участь в дуєлі
        /// </summary>
        private IUnit RegisterDuelUnit(string gameId, int actorNr)
        {
            // Перебираем всех игроков
            // Вытащить у игрока юнита с VIP
            IUnit unit = _unitsService.GetVipUnit(gameId, actorNr);

            if (unit == null){
                unit = _unitsService.GetAnyAliveUnitWhoWillBeAbleToVip(gameId, actorNr);
            }

            if (unit == null){
                unit = _unitsService.GetAnyUnitWhoWillBeAbleToVip(gameId, actorNr);
            }

            return unit;
        }

        /// <summary>
        /// Воскресить юнита
        /// </summary>
        private void ReviveUnit(IUnit unit)
        {
            unit.IsDead = false;
            ((IHealthComponent)unit).HealthCapacity = 1;
            ((IDamageAction)unit).ActionCapacity = 1;
            ((IDamageAction)unit).OriginalActionCapacity = 1;
        }

        /// <summary>
        /// Оба Vip-а мертвы.
        /// </summary>
        private void BothVipsAreDead()
        {
            foreach (ActorScheme actor in _actors)
            {
                //_gridService.ClearGrids(actor.ActorId);
                //ReviveUnit(actor.IsLocal ? _localDuelUnit : _enemyDuelUnit);
            }

            ReviveUnit(_actor0DuelUnit);
            ReviveUnit(_actor1DuelUnit);

            // Продолжаем игру дальше
        }

        /// <summary>
        /// Для дуэли юнитам нужно сменить карту навигации
        /// </summary>
        private void ChangeWayForDuel(IUnit unit)
        {
            if (!(unit is IWalkableComponent) && !(unit is INavigationWayForDuelComponent))
                return;

            (unit as IWalkableComponent).NavigationWay = (unit as INavigationWayForDuelComponent).NavigationWayForDuel;
        }
    }
}

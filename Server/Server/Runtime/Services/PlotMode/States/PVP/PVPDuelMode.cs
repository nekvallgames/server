﻿using Photon.Hive.Plugin;
using Plugin.Interfaces;
using Plugin.Interfaces.Actions;
using Plugin.Interfaces.UnitComponents;
using Plugin.Schemes;
using Plugin.Tools;
using System;
using System.Collections.Generic;

namespace Plugin.Runtime.Services.PlotMode.States.PVP
{
    public class PVPDuelMode : ITask
    {
        public const string NAME = "PVPDuelMode";
        public string Name => NAME;

        private PlotModeService _plotModeService;
        private IPluginHost _host;
        private PVPPlotModelScheme _model;
        private ActorService _actorService;
        private UnitsService _unitsService;

        // id юнітів гравців, котрі приймають участь в ігровому режимі дуєль
        private IUnit _actor0DuelUnit;
        private IUnit _actor1DuelUnit;

        private List<IActorScheme> _actors;

        public PVPDuelMode(PlotModeService plotModeService, 
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
            LogChannel.Log("PlotModeService :: PVPDuelMode :: EnterTask()", LogChannel.Type.Plot);
            _model.GameMode = Name;

            _actors = _actorService.GetActorsInRoom(_host.GameId);

            if (!_model.WasPreparedDuel)
            {
                _model.WasPreparedDuel = true;

                _actor0DuelUnit = RegisterDuelUnit(_host.GameId, _actors[0].ActorNr);
                _actor1DuelUnit = RegisterDuelUnit(_host.GameId, _actors[1].ActorNr);

                // 9. Востановити юнітів, котрі будуть приймати участь в дуєлі
                ReviveUnit(_actor0DuelUnit);
                ReviveUnit(_actor1DuelUnit);

                foreach (ActorScheme actor in _actors)
                {
                    _unitsService.RemoveAllMedicHealing(_host.GameId, actor.ActorNr);
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

                // Дополнительное условие, если дуэль началась раньше, чем были назначены VIP-ы
                if (!IsAliveVipActor0 && !IsAliveVipActor1)
                {
                    bool IsAliveAnyActor0 = _unitsService.HasAliveUnit(_host.GameId, _actors[0].ActorNr)
                        ? true
                        : false;
                    bool IsAliveAnyActor1 = _unitsService.HasAliveUnit(_host.GameId, _actors[1].ActorNr)
                        ? true
                        : false;

                    if (IsAliveAnyActor0 || IsAliveAnyActor1)
                    {
                        IsAliveVipActor0 = IsAliveAnyActor0;
                        IsAliveVipActor1 = IsAliveAnyActor1;
                    }
                }

                if (IsAliveVipActor0 && IsAliveVipActor1)
                {
                    // Vip-ы игроков живы. Продолжаем игру далее
                    taskIsDone?.Invoke();
                    return;
                }

                if (!IsAliveVipActor0 && !IsAliveVipActor1)
                {
                    // Vip-ы обоих игроков мертвы
                    BothVipsAreDead();
                    taskIsDone?.Invoke();
                    return;
                }

                // Изменить игровой этап
                _plotModeService.ExecuteTask(ResultMode.NAME, taskIsDone, taskIsFail);
                return;
            }

            taskIsDone?.Invoke();
        }

        /// <summary>
        /// Зареєструвати юніта, котрий буде приймати участь в дуєлі
        /// </summary>
        private IUnit RegisterDuelUnit(string gameId, int actorNr)
        {
            // Перебираем всех игроков
            // Вытащить у игрока юнита с VIP
            IUnit unit = _unitsService.GetVipUnit(gameId, actorNr);

            if (unit == null)
            {
                unit = _unitsService.GetAnyAliveUnitWhoWillBeAbleToVip(gameId, actorNr);
            }

            return unit;
        }

        /// <summary>
        /// Воскресить юнита
        /// </summary>
        private void ReviveUnit(IUnit unit)
        {
            unit.IsDead = false;
            ((IHealthComponent)unit).Capacity = 1;
            ((IDamageAction)unit).Capacity = 1;
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
        public void ExitTask()
        {

        }
    }
}
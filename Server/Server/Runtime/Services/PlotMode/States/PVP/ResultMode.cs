using Photon.Hive.Plugin;
using Plugin.Interfaces;
using Plugin.Schemes;
using Plugin.Tools;
using System;
using System.Collections.Generic;

namespace Plugin.Runtime.Services.PlotMode.States.PVP
{
    /// <summary>
    /// В этом игровом этапе, отображаем выиграш или проиграш игрока локального игрока
    /// </summary>
    public class ResultMode : IMode
    {
        public const Enums.PVPMode Mode = Enums.PVPMode.Result;
        public int ModeId => (int)Mode;

        private IPluginHost _host;
        private PVPPlotModelScheme _model;
        private ActorService _actorService;
        private UnitsService _unitsService;

        public ResultMode(IPluginHost host, 
                          PVPPlotModelScheme model,
                          ActorService actorService,
                          UnitsService unitsService)
        {
            _host = host;
            _model = model;
            _actorService = actorService;
            _unitsService = unitsService;
        }

        public void ExecuteMode(Action success)
        {
            LogChannel.Log("PlotModeService :: ResultMode :: EnterTask()", LogChannel.Type.Plot);

            _model.GameMode = (int)Mode;

            List<IActorScheme> actors = _actorService.GetActorsInRoom(_host.GameId);

            // Проверяем условия выиграша или проиграша
            bool IsAliveVipActor0 = _unitsService.VipIsAlive(_host.GameId, actors[0].ActorNr)
                ? true
                : false;
            bool IsAliveVipActor1 = _unitsService.VipIsAlive(_host.GameId, actors[1].ActorNr)
                ? true
                : false;

            int winnerActorNr = -1;

            if (IsAliveVipActor0 || IsAliveVipActor1)
            {
                // Один из VIP-ов остался вживых
                winnerActorNr = IsAliveVipActor0 ? actors[0].ActorNr : actors[1].ActorNr;
            }
            else
            {
                // Оба VIP-а мертвы. Делаем победителя того, у кого есть хоть какие то выжившие юниты
                bool hasActor0AliveUnits = (_unitsService.GetAnyAliveUnit(_host.GameId, actors[0].ActorNr) != null ? true : false);
                bool hasActor1AliveUnits = (_unitsService.GetAnyAliveUnit(_host.GameId, actors[1].ActorNr) != null ? true : false);

                if (hasActor0AliveUnits && hasActor1AliveUnits)
                {
                    // Оба гравці мають мертвих віпів, але при цьому мають і живих юнітів
                    // Не робимо переможцем ніодного із гравців
                }
                else if (hasActor0AliveUnits || hasActor1AliveUnits)
                      {
                          winnerActorNr = hasActor0AliveUnits ? actors[0].ActorNr : actors[1].ActorNr;
                      }
            }

            _model.IsGameFinished = true;
            _model.WinnerActorsNr.Add(winnerActorNr);

            success?.Invoke();
        }
    }
}

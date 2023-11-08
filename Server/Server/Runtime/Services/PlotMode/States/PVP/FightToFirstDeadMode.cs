using Photon.Hive.Plugin;
using Plugin.Interfaces;
using Plugin.Schemes;
using Plugin.Tools;
using System;
using System.Collections.Generic;

namespace Plugin.Runtime.Services.PlotMode.States.PVP
{
    /// <summary>
    /// В первом игровом этапе, игроки перестреливаются до первой смерти юнита
    /// Как только первый юнит погибнет, нужно перейти в этап FightWithVipStage 
    /// </summary>
    public class FightToFirstDeadMode : IMode
    {
        public const Enums.PVPMode Mode = Enums.PVPMode.FightToFirstDead;
        public int ModeId => (int)Mode;

        private IPluginHost _host;
        private PvpPlotModelScheme _model;
        private ActorService _actorService;
        private UnitsService _unitsService;
        private PlotModeService _plotModeService;

        public FightToFirstDeadMode(PlotModeService plotModeService,
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
            LogChannel.Log("PlotModeService :: FightToFirstDeadMode :: EnterTask()", LogChannel.Type.Plot);

            _model.GameMode = (int)Mode;
            _model.IsNeedToCheckOnCorrectPosition = true;

            List<IActorScheme> actors = _actorService.GetActorsInRoom(_host.GameId);

            var actorsData = new List<ActorData>();
            foreach (IActorScheme actor in actors)
            {
                actorsData.Add(new ActorData
                {
                    hasAnyDeadUnit = _unitsService.HasAnyDeadUnit(_host.GameId, actor.ActorNr),
                    countAliveUnits = _unitsService.GetAliveUnitsCount(_host.GameId, actor.ActorNr),
                    hasAliveUnitWhoWillBeAbleToVip = _unitsService.GetAnyAliveUnitWhoWillBeAbleToVip(_host.GameId, actor.ActorNr),
                    countAliveUnitWhoWillBeAbleToVip = _unitsService.GetAliveUnitsCountWhoWillBeAbleToVip(_host.GameId, actor.ActorNr)
                });
            }

            // Один із гравців має мертвого юніта
            if (actorsData[0].hasAnyDeadUnit || actorsData[1].hasAnyDeadUnit)
            {
                if ((actorsData[0].countAliveUnits <= 0 && actorsData[1].countAliveUnits <= 0)  // Гравці не мають жодних живих юнітів. Абсолютно всі мертві.
                    || (actorsData[0].hasAliveUnitWhoWillBeAbleToVip == null && actorsData[1].hasAliveUnitWhoWillBeAbleToVip == null)  // Обидва гравці не мають живих юнітів, котрих можно зробити VIP
                    || (actorsData[0].hasAliveUnitWhoWillBeAbleToVip != null && actorsData[1].hasAliveUnitWhoWillBeAbleToVip == null)  // Один із гравців має живого юніта, котрий може стати VIP. A другий гравець не має живого юніта, котрий може стати VIP.
                    || (actorsData[0].hasAliveUnitWhoWillBeAbleToVip == null && actorsData[1].hasAliveUnitWhoWillBeAbleToVip != null)) // Один із гравців має живого юніта, котрий може стати VIP. A другий гравець не має живого юніта, котрий може стати VIP.
                {
                    _plotModeService.ExecuteMode((int)ResultMode.Mode, success);
                    return;
                }

                if (actorsData[0].countAliveUnitWhoWillBeAbleToVip == 1 && actorsData[1].countAliveUnitWhoWillBeAbleToVip == 1)
                {
                    // Гравці мають лише по одному юніту, та ці юніти можуть бути VIP, отже go to duel mode
                    _plotModeService.ExecuteMode((int)DuelMode.Mode, success);
                    return;
                }

                _plotModeService.ExecuteMode((int)FightWithVipMode.Mode, success);
                return;
            }
            
            success?.Invoke();
        }
    }

    internal struct ActorData
    {
        public bool hasAnyDeadUnit;
        public int countAliveUnits;
        public IUnit hasAliveUnitWhoWillBeAbleToVip;
        public int countAliveUnitWhoWillBeAbleToVip;

        public ActorData(bool hasAnyDeadUnit,
                         int countAliveUnits,
                         IUnit hasAliveUnitWhoWillBeAbleToVip,
                         int countAliveUnitWhoWillBeAbleToVip)
        {
            this.hasAnyDeadUnit = hasAnyDeadUnit;
            this.countAliveUnits = countAliveUnits;
            this.hasAliveUnitWhoWillBeAbleToVip = hasAliveUnitWhoWillBeAbleToVip;
            this.countAliveUnitWhoWillBeAbleToVip = countAliveUnitWhoWillBeAbleToVip;
        }
    }
}

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
    public class PVPFightToFirstDeadMode : ITask
    {
        public const string NAME = "PVPFightToFirstDeadMode";
        public string Name => NAME;

        private IPluginHost _host;
        private PVPPlotModelScheme _model;
        private ActorService _actorService;
        private UnitsService _unitsService;
        private PlotModeService _plotModeService;

        public PVPFightToFirstDeadMode(PlotModeService plotModeService,
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
            LogChannel.Log("PlotModeService :: PVPFightToFirstDeadMode :: EnterTask()", LogChannel.Type.Plot);

            _model.GameMode = Name;

            List<IActorScheme> actors = _actorService.GetActorsInRoom(_host.GameId);

            var actorsData = new List<ActorData>();
            foreach (IActorScheme actor in actors)
            {
                actorsData.Add(new ActorData
                {
                    hasAnyDeadUnit = _unitsService.HasAnyDeadUnit(_host.GameId, actor.ActorNr),
                    countAliveUnits = _unitsService.GetAliveUnitsCount(_host.GameId, actor.ActorNr),
                    hasAliveUnitWhoWillBeAbleToVip = _unitsService.GetAnyAliveUnitWhoWillBeAbleToVip(_host.GameId, actor.ActorNr),
                    countAliveUnitWhoWillBeAbleToVip = _unitsService.GetAliveUnitsCountWhoWillBeAbleToVip(_host.GameId, actor.ActorNr),
                    hasUnitWhoWillBeAbleToVip = _unitsService.GetAnyUnitWhoWillBeAbleToVip(_host.GameId, actor.ActorNr)
                });
            }

            if (actorsData[0].hasAnyDeadUnit || actorsData[1].hasAnyDeadUnit)
            {
                if (actorsData[0].hasAliveUnitWhoWillBeAbleToVip == null &&
                    actorsData[1].hasAliveUnitWhoWillBeAbleToVip == null)
                {
                    // Гравці не мають живих юнітів, котрих можно зробити VIP або піти в дуєль

                    if (actorsData[0].countAliveUnits <= 0 && actorsData[1].countAliveUnits <= 0)
                    {
                        // Гравці не мають взагалі живих юнітів
                        if (actorsData[0].hasUnitWhoWillBeAbleToVip != null && actorsData[1].hasUnitWhoWillBeAbleToVip != null)
                        {
                            // Гравці не мають вживих ніодного юніта, але в списку мертвих юнітів є юніти,
                            // котрі могли б стати VIP і зіграти в дуєль моді
                            LogChannel.Log("IPlotModeService :: FightToFirstDeadMode() change game mode. Case 1", LogChannel.Type.Plot);
                            _plotModeService.ExecuteTask(PVPDuelMode.NAME, taskIsDone, taskIsFail);
                            return;
                        }
                        else
                        {
                            // один із акторів не має юніта, котрого можно зробити VIP для гри в дуєлі
                            LogChannel.Log("IPlotModeService :: FightToFirstDeadMode() change game mode. Case 2", LogChannel.Type.Plot);
                            _plotModeService.ExecuteTask(ResultMode.NAME, taskIsDone, taskIsFail);
                            return;
                        }
                    }
                    else if (actorsData[0].countAliveUnits > 0 && actorsData[1].countAliveUnits > 0)
                    {
                        // Оба гравця мають юнітів в команді, але ці юніти не може бути VIP, наприклад турель, або тотем ісцеленія
                        LogChannel.Log("IPlotModeService :: FightToFirstDeadMode() change game mode. Case 3", LogChannel.Type.Plot);
                        _plotModeService.ExecuteTask(ResultMode.NAME, taskIsDone, taskIsFail);
                        return;
                    }
                    else
                    {
                        // Один із гравців все таки має живого юніта, але цей юніт не може бути VIP,
                        // наприклад турель, або тотем ісцеленія
                        LogChannel.Log("IPlotModeService :: FightToFirstDeadMode() change game mode. Case 4", LogChannel.Type.Plot);
                        _plotModeService.ExecuteTask(ResultMode.NAME, taskIsDone, taskIsFail);
                        return;
                    }
                }

                if (actorsData[0].hasAliveUnitWhoWillBeAbleToVip != null &&
                    actorsData[1].hasAliveUnitWhoWillBeAbleToVip != null)
                {
                    // Оба гравця мають вживих юнітів, котрі можуть стати VIP
                    if (actorsData[0].countAliveUnitWhoWillBeAbleToVip == 1 &&
                        actorsData[1].countAliveUnitWhoWillBeAbleToVip == 1)
                    {
                        // Гравці мають лише по одному юніту, та ці юніти можуть бути VIP, отже go to duel mode
                        LogChannel.Log("IPlotModeService :: FightToFirstDeadMode() change game mode. Case 5", LogChannel.Type.Plot);
                        _plotModeService.ExecuteTask(PVPDuelMode.NAME, taskIsDone, taskIsFail);
                        return;
                    }
                    else
                    {
                        LogChannel.Log("IPlotModeService :: FightToFirstDeadMode() change game mode. Case 6", LogChannel.Type.Plot);
                        _plotModeService.ExecuteTask(PVPFightWithVipMode.NAME, taskIsDone, taskIsFail);
                        return;
                    }
                }

                if (actorsData[0].hasAliveUnitWhoWillBeAbleToVip != null ||
                    actorsData[1].hasAliveUnitWhoWillBeAbleToVip != null)
                {
                    // Один із гравців має юніта, котрий може стати VIP, а другий гравець ні
                    LogChannel.Log("IPlotModeService :: FightToFirstDeadMode() change game mode. Case 7", LogChannel.Type.Plot);
                    _plotModeService.ExecuteTask(ResultMode.NAME, taskIsDone, taskIsFail);
                    return;
                }
            }
        }

        public void ExitTask()
        {

        }
    }

    internal struct ActorData
    {
        public bool hasAnyDeadUnit;
        public int countAliveUnits;
        public IUnit hasAliveUnitWhoWillBeAbleToVip;
        public int countAliveUnitWhoWillBeAbleToVip;
        public IUnit hasUnitWhoWillBeAbleToVip;

        public ActorData(bool hasAnyDeadUnit,
                         int countAliveUnits,
                         IUnit hasAliveUnitWhoWillBeAbleToVip,
                         int countAliveUnitWhoWillBeAbleToVip,
                         IUnit hasUnitWhoWillBeAbleToVip)
        {
            this.hasAnyDeadUnit = hasAnyDeadUnit;
            this.countAliveUnits = countAliveUnits;
            this.hasAliveUnitWhoWillBeAbleToVip = hasAliveUnitWhoWillBeAbleToVip;
            this.countAliveUnitWhoWillBeAbleToVip = countAliveUnitWhoWillBeAbleToVip;
            this.hasUnitWhoWillBeAbleToVip = hasUnitWhoWillBeAbleToVip;
        }
    }
}

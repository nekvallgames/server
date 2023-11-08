using Plugin.Interfaces;
using Plugin.Runtime.Services.AI.Outputs;
using Plugin.Schemes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Plugin.Runtime.Services.AI.Tasks
{
    /// <summary>
    /// Виконувач AI, котрий прийме рішення і виконає дії, чи потрібно змінювати VIP юніту
    /// </summary>
    public class AIVipTask : IAiTask
    {
        public const string TASK_NAME = "AIVipTask";
        public string Name => TASK_NAME;

        private int CHANGE_TO_FAIL_NOTIFICATION = 30;

        private VipDecisionService _vipDecisionService;
        private UnitsService _unitsService;
        private SimulateNotificationChangeVipService _simulateNotificationChangeVipService;
        private ActorService _actorService;

        public AIVipTask(VipDecisionService vipDecisionService,
                         UnitsService unitsService,
                         SimulateNotificationChangeVipService simulateNotificationChangeVipService,
                         ActorService actorService)
        {
            _vipDecisionService = vipDecisionService;
            _unitsService = unitsService;
            _simulateNotificationChangeVipService = simulateNotificationChangeVipService;
            _actorService = actorService;
        }

        public void ExecuteTask(string gameId)
        {
            int aiActorNr = _actorService.GetAiActor(gameId).ActorNr;

            IUnit unitVip = _unitsService.GetVipUnit(gameId, aiActorNr);

            // Отримати список кандидатів, котрі можуть стати VIP-ами
            var candidates = new List<IUnit>();
            _unitsService.GetAliveUnitsForVip(gameId, aiActorNr, ref candidates);

            if (candidates.Count <= 0 || unitVip == null)
            {
                return;    // в гравця не має взагалі VIP або залишився тільки 1
            }

            var decisions = new List<IUnitDecision>();
            foreach (IUnit candidate in candidates)
            {
                // Перебираємо кожного кандидата, і для кожного знаходимо рішення, чи можно вказаного кандитата зробити VIP-ом
                decisions.Add(_vipDecisionService.Decision<UnitDecisionScheme>(candidate, unitVip));
            }

            var allowOutputs = decisions.FindAll(x => x.Output.OutputId == AllowOutput.OUTPUT_ID);
            if (!allowOutputs.Any())
            {
                Random random = new Random();

                if (random.Next(0, 100) < CHANGE_TO_FAIL_NOTIFICATION)
                {
                    _simulateNotificationChangeVipService.Execute();
                }

                return;    // не має рішень, які дозволяють прийняти рішення
            }

            allowOutputs.Sort((x, y) => x.Output.Sum.CompareTo(y.Output.Sum));
            allowOutputs.Reverse();

            // Зробити VIP із найбільшою сумою output
            //_syncRoomService.SimulateSyncVip(aiActorNr, allowOutputs[0].Units[0], true);
            //_simulateNotificationChangeVipService.Execute();
        }

        public void ExitTask()
        {

        }
    }
}

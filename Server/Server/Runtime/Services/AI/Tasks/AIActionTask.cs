using Plugin.Interfaces;
using Plugin.Schemes;
using System.Collections.Generic;

namespace Plugin.Runtime.Services.AI.Tasks
{
    /// <summary>
    /// Виконувач AI, котрий прийме рішення і виконає дії,
    /// як саме і кого потрібно атакувати в стейті FirstAction
    /// </summary>
    public class AIActionTask : IAiTask
    {
        public const string TASK_NAME = "AIActionTask";
        public string Name => TASK_NAME;

        private UnitsService _unitsService;
        private PathService _pathService;

        private IActionDecisionService[] _decisionServices;

        private int _targetActorNr;
        private int _aiActorNr;
        private ActorService _actorService;
        private SimulateSyncActionService _simulateSyncActionService;

        public AIActionTask(UnitsService unitsService,
                             IWeaponActionDecisionService weaponActionDecisionService,
                             IPoisonActionDecisionService poisonActionDecisionService,
                             PathService pathService,
                             ActorService actorService,
                             SimulateSyncActionService simulateSyncActionService)
        {
            _unitsService = unitsService;
            _pathService = pathService;
            _actorService = actorService;
            _simulateSyncActionService = simulateSyncActionService;

            _decisionServices = new IActionDecisionService[]
            {
                (IActionDecisionService)poisonActionDecisionService,
                (IActionDecisionService)weaponActionDecisionService
            };
        }

        public void ExecuteTask(string gameId)
        {
            _targetActorNr = _actorService.GetRealActor(gameId).ActorNr;
            _aiActorNr = _actorService.GetAiActor(gameId).ActorNr;

            // Список юнітів, котрі будуть атакувати 
            var hunterUnits = new List<IUnit>();
            _unitsService.GetAliveUnits(gameId, _aiActorNr, ref hunterUnits);

            // Список юнітів, котрих будуть атакувати
            var targetUnits = new List<IUnit>();
            _unitsService.GetAliveUnits(gameId, _targetActorNr, ref targetUnits);

            if (hunterUnits.Count <= 0 || targetUnits.Count <= 0){
                return;    // не має юнітів, котрі можуть атакувати, або нікого атакувати
            }

            // Высчитать путь, куда могут переместится юнити игрока, которого будем атаковать
            _pathService.Calculate(gameId, _targetActorNr);

            // Перебираємо кожного юніта, котрий буде атакувати і знайти рішення, кого саме юніт буде атакувати
            var decisions = new List<IUnitDecision>();
            foreach (IUnit hunterUnit in hunterUnits)
            {
                foreach (IActionDecisionService decision in _decisionServices)
                {
                    if (decision.CanDecision(hunterUnit))
                    {
                        decisions.Add(decision.Decision<UnitDecisionScheme>(hunterUnit, gameId, _targetActorNr));
                        break;
                    }
                }
            }

            // в decisions ми маємо список юнітів котрі атакують і юнітів, котрих атакують
            foreach (IUnitDecision decision in decisions)
            {
                IUnit unitHunter = decision.Units[0];
                IUnit unitTarget = decision.Units[1];
                _simulateSyncActionService.SimulateAllActions(unitHunter, unitTarget);
            }
        }

        public void ExitTask()
        {

        }
    }
}

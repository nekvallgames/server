using Plugin.Interfaces;
using Plugin.Runtime.Services.AI.Inputs;
using Plugin.Runtime.Services.AI.Outputs;
using Plugin.Runtime.Services.UnitsPath;
using System;
using System.Collections.Generic;

namespace Plugin.Runtime.Services.AI
{
    /// <summary>
    /// Сервіс, робота котрого приймати рішення, якого юніта краще зробити VIP-ом
    /// </summary>
    public class VipDecisionService : BaseDecision
    {
        // Allow
        private float _i1_w1 = 0.5f;
        private float _i2_w1 = 1f;
        private float _i3_w1 = 1f;
        private float _i4_w1 = 1f;
        // Forbidden
        private float _i1_w2 = 0.5f;
        private float _i2_w2 = 1f;
        private float _i3_w2 = 1f;
        private float _i4_w2 = 1f;


        private const int INPUT_SIZE = 4;
        /// <summary>
        /// Логіювати прийняте рішення
        /// </summary>
        private const bool _isLog = false;

        private UnitsService _unitsService;
        private UnitsPathService _unitsPathService;

        public VipDecisionService(UnitsService unitsService, UnitsPathService unitsPathService)
        {
            _unitsService = unitsService;
            _unitsPathService = unitsPathService;
        }

        /// <summary>
        /// Вказавши 2 юніта, отримати рішення, чи можно змінити поточного юніта із VIP на вказаного юніта
        /// </summary>
        /// <param name="candidateUnit">кандидат в VIP</param>
        /// <param name="currUnit">юніт, котрий є поточним VIP</param>
        public T Decision<T>(IUnit candidateUnit, IUnit currUnit) where T : IUnitDecision
        {
            // Отримати список із інпутів, котрі дозволяють змінити старого VIP-а на нового
            var allowInputs = new float[INPUT_SIZE];
            CalculateInputsSize(candidateUnit, currUnit, ref allowInputs);

            // Отримати список із інпутів, котрі забороняють змінити старого VIP-а на нового
            var forbidInputs = new float[INPUT_SIZE];
            CalculateInputsSize(currUnit, candidateUnit, ref forbidInputs);

            // Створюємо всі рішення, котрі потрібно прийняти на основі даних, котрі прилетять в inputs
            var outputs = new List<IOutput>()
            {
                new AllowOutput(allowInputs,
                                new float[INPUT_SIZE]{_i1_w1,_i2_w1,_i3_w1,_i4_w1},
                                0f,
                                _isLog),
                new ForbidOutput(forbidInputs,
                                new float[INPUT_SIZE]{_i1_w2,_i2_w2,_i3_w2,_i4_w2},
                                0f,
                                _isLog),
            };

            // if (_isLog)
            // {
            //     Debug.Log($"Decision to change VIP. Candidate uId = {candidateUnit.UnitId}, vipUId = {currUnit.UnitId}. Allow = {outputs[0].Sum}, Forbidden = {outputs[1].Sum}");
            // }

            // Якщо є прийняте рішення, то знайти рішення із максимальною сумою інпутів
            T decision = Activator.CreateInstance<T>();
            decision.Output = GetDecision(outputs);
            decision.Units = new List<IUnit>() { candidateUnit, currUnit };

            return decision;
        }

        /// <summary>
        /// Отримати список із інпутів
        /// </summary>
        private void CalculateInputsSize(IUnit unit1, IUnit unit2, ref float[] inputs)
        {
            // Створюємо всі інпути, котрі будуть впливати на рішення
            var allowInputs = new IInput[INPUT_SIZE]
            {
                new SizeBodyU1LowerU2Input(unit1, unit2),
                new HealthU1BiggerU2Input(unit1, unit2, _unitsService),
                new ArmorU1BiggerU2Input(unit1, unit2),
                new WayAreaU1BiggerU2Input(unit1, unit2, _unitsPathService),
            };

            // Створити массив із вагів інпута
            for (int i = 0; i < allowInputs.Length; i++)
            {
                inputs[i] = allowInputs[i].Size;
            }
        }


    }
}

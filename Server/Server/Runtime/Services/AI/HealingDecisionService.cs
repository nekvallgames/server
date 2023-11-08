using Plugin.Interfaces;
using Plugin.Runtime.Services.AI.Inputs;
using Plugin.Runtime.Services.AI.Outputs;
using System;
using System.Collections.Generic;

namespace Plugin.Runtime.Services.AI
{
    /// <summary>
    /// Сервіс, робота котрого приймати рішення, якого юніта краще вилікувати хілкою
    /// </summary>
    public class HealingDecisionService : BaseDecision
    {
        // Allow
        private float _i1_w1 = 1f;
        private float _i2_w1 = 1f;
        private float _i3_w1 = 1f;
        // Forbidden
        private float _i1_w2 = 0f;
        private float _i2_w2 = 0f;
        private float _i3_w2 = 0f;

        private const int INPUT_SIZE = 3;
        /// <summary>
        /// Логіювати прийняте рішення
        /// </summary>
        private const bool _isLog = true;

        private UnitsService _unitService;

        public HealingDecisionService(UnitsService unitsService)
        {
            _unitService = unitsService;
        }
        
        /// <summary>
        /// Вказавши юніта, що б отримати рішення, чи потрібно поточного юніта лікувати
        /// Якщо не потрібно, то отримаємо output - ForbidOutput.
        /// Якщо вилікувати потрібно, то отриваємо output - AllowOutput та вагу приорітета лікування
        /// </summary>
        public T Decision<T>(IUnit unit) where T : IUnitDecision
        {
            // Отримати список із інпутів, котрі дозволяють змінити старого VIP-а на нового
            var allowInputs = new float[INPUT_SIZE];
            CalculateInputsSize(unit, ref allowInputs);

            // Отримати список із інпутів, котрі забороняють змінити старого VIP-а на нового
            var forbidInputs = new float[INPUT_SIZE];
            CalculateInputsSize(unit, ref forbidInputs);

            // Створюємо всі рішення, котрі потрібно прийняти на основі даних, котрі прилетять в inputs
            var outputs = new List<IOutput>()
            {
                new AllowOutput(allowInputs,
                                new float[INPUT_SIZE]{_i1_w1,_i2_w1,_i3_w1},
                                0f,
                                _isLog),
                new ForbidOutput(forbidInputs,
                                 new float[INPUT_SIZE]{_i1_w2,_i2_w2,_i3_w1},
                                 0f,
                                 _isLog),
            };

           //if (_isLog)
           //{
           //    Debug.Log($"Decision to health candidate uId = {unit.UnitId}. Allow = {outputs[0].Sum}, Forbidden = {outputs[1].Sum}");
           //}

            // Якщо є прийняте рішення, то знайти рішення із максимальною сумою інпутів
            T decision = Activator.CreateInstance<T>();
            decision.Output = GetDecision(outputs);
            decision.Units = new List<IUnit>() { unit };

            return decision;
        }

        /// <summary>
        /// Отримати список із інпутів
        /// </summary>
        private void CalculateInputsSize(IUnit unit, ref float[] inputs)
        {
            // Створюємо всі інпути, котрі будуть впливати на рішення
            var allowInputs = new IInput[INPUT_SIZE]
            {
                new NeedToHealInput(unit, _unitService),
                new IsVipInput(unit),
                new HasPoisonActionInput(unit),
            };

            // Створити массив із вагів інпута
            for (int i = 0; i < allowInputs.Length; i++)
            {
                inputs[i] = allowInputs[i].Size;
            }
        }

    }
}

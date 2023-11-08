using Plugin.Interfaces;
using System;

namespace Plugin.Runtime.Services.AI.Inputs
{
    /// <summary>
    /// Базовий клас для всіх інпутив AI
    /// За допомогою інпутів та їхньої ваги AI буде приймати output рішення  
    /// </summary>
    public abstract class BaseInput : IInput
    {
        /// <summary>
        /// Сила інпута
        /// </summary>
        public abstract float Size { get; }

        /// <summary>
        /// Вирахувати силу розміру інпута,
        /// на скільки одне число відрізняється від другого
        /// Наприклад, сила одного юніта 100, а другого 50, то результат буде сида інпута 0.5
        /// Так як один юніт на 50% сильніший за другого
        /// </summary>
        protected float CompareSize(float u1Value, float u2Value)
        {
            float value = (u1Value / u2Value) - 1f;

            // Отримати кількість чисел перед крапкою
            var v1 = 1 + Math.Log10(value * value) / 2;
            var valueLength = Math.Max(v1 - v1 % 1, 1);

            if (valueLength > 1)
            {
                value /= CalculateOffset((int)valueLength);
            }

            return value < 0 ? 0 : value;
        }

        /// <summary>
        /// Функція, котра вирахує на скільки потрібно поділити число, що би значення було до 1
        /// Наприклад, число valueLength == 98, то функція поверне число, наприклад 100, що би отримати число 0.98
        /// Наприклад, число valueLength == 7.5, то функція поверне число, наприклад 10, що би отримати число 0.75
        /// </summary>
        private float CalculateOffset(int valueLength)
        {
            float offset = 1;

            for (int i = 0; i < valueLength; i++)
            {
                offset *= 10;
            }

            return offset;
        }
    }
}

using Plugin.Interfaces;

namespace Plugin.Runtime.Services.AI.Outputs
{
    public abstract class BaseOutput : IOutput
    {
        public abstract int OutputId { get; }

        /// <summary>
        /// Вказати силу активації поточного рішення
        /// Тобто, яка сумма інпутів потрібна для прийняття поточного рішення
        /// </summary>
        public float Activation { get; }

        /// <summary>
        /// Вага для кожного інпута
        /// </summary>
        public float[] Weight { get; }

        /// <summary>
        /// Загальна сумма інпутів
        /// </summary>
        public float Sum { get; protected set; }

        /// <summary>
        /// Поточне рішення було прийнято?
        /// Тобто, потрібна сумма інпутів для прийняття поточного рішення була набрана
        /// </summary>
        public bool IsActive { get; protected set; }

        public BaseOutput()
        {

        }

        public BaseOutput(float[] inputs, float[] weight, float activation, bool isLog)
        {
            Weight = weight;
            Activation = activation;

            // Перемножаємо всі inputs на вагу прийнятта рішення
            string log = $"Decision id = {OutputId};";
            for (int i = 0; i < inputs.Length; i++)
            {
                float sum = inputs[i] * weight[i];
                Sum += sum;
                log += $" [{i}] = {sum};";
            }

            IsActive = Sum >= Activation;
            if (Sum <= 0)
                IsActive = false;

            // log += $" Sum = {Sum}; IsActive = {IsActive}";
            // if (isLog)
            //     Debug.Log(log);
        }
    }
}

namespace Plugin.Interfaces
{
    /// <summary>
    /// Інтерфейс для класів, котрі зберігають в собі прийняті рішення AI
    /// </summary>
    public interface IOutput
    {
        int OutputId { get; }

        /// <summary>
        /// Вказати силу активації поточного рішення
        /// Тобто, яка сумма інпутів потрібна для прийняття поточного рішення
        /// </summary>
        float Activation { get; }

        /// <summary>
        /// Вага для кожного інпута
        /// </summary>
        float[] Weight { get; }

        /// <summary>
        /// Загальна сумма інпутів
        /// </summary>
        float Sum { get; }

        /// <summary>
        /// Поточне рішення було прийнято?
        /// Тобто, потрібна сумма інпутів для прийняття поточного рішення була набрана
        /// </summary>
        bool IsActive { get; }
    }
}

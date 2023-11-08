namespace Plugin.Interfaces
{
    /// <summary>
    /// Інтерфейс для інпута AI
    /// За допомогою інпутів та їхньої ваги AI буде приймати output рішення
    /// </summary>
    public interface IInput
    {
        /// <summary>
        /// Сила інпута
        /// </summary>
        float Size { get; }
    }
}

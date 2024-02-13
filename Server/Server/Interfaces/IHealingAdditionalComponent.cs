using Plugin.Tools;

namespace Plugin.Interfaces
{
    /// <summary>
    /// Дополнительный (пассивный) навык медика. Имеет несколько хилок, 
    /// которыми может лечить своих членов команды
    /// </summary>
    public interface IHealingAdditionalComponent
    {
        /// <summary>
        /// Может ли медик лечить? 
        /// Проверяю, есть ли у него аптечки
        /// </summary>
        bool CanExecuteAdditional();

        /// <summary>
        /// Получить силу лечения
        /// </summary>
        int GetHealthPower();

        /// <summary>
        /// Вылечить
        /// </summary>
        void SpendAdditional();

        /// <summary>
        /// Получить рисунок экшена
        /// </summary>
        Int2[] GetAdditionalArea();

        int AdditionalCapacity { get; set; }
    }
}

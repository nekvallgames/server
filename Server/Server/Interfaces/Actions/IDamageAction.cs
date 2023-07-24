using Plugin.Interfaces.UnitComponents;
using Plugin.Tools;

namespace Plugin.Interfaces.Actions
{
    /// <summary>
    /// Выполнить действие - нанести урон
    /// </summary>
    public interface IDamageAction : IActionComponent
    {
        /// <summary>
        /// Инитиализировать оружие
        /// </summary>
        void Initialize();

        /// <summary>
        /// Перезарядить оружие
        /// </summary>
        void Reload();

        /// <summary>
        /// Может ли юнит выстрелить?
        /// Есть ли патроны для выстрела?
        /// </summary>
        bool CanExecuteAction();

        /// <summary>
        /// Юнит выстрелил. Использовать аммуницию
        /// </summary>
        void SpendAction();

        /// <summary>
        /// Сила урона от текущего оружия. Но, текущее значение может изменятся,
        /// так как сущность может получить как баф, так и дебаф
        /// </summary>
        int Power { get; set; }

        /// <summary>
        /// Базовая сила урона от текущего оружия
        /// Текущий параметр изменять нельзя!!!
        /// </summary>
        int OriginalPower { get; }

        /// <summary>
        /// Получить рисунок экшена
        /// </summary>
        Int2[] DamageActionArea { get; }

        /// <summary>
        /// Поточна кількість амуніції 
        /// </summary>
        int Capacity { get; }

        /// <summary>
        /// Орігінальна кількість амуніції при створенні юніта
        /// </summary>
        int OriginalCapacity { get; }
    }
}

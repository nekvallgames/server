namespace Plugin.Interfaces.UnitComponents
{
    public interface IHealthComponent
    {
        /// <summary>
        /// Получить количество жизней юнита
        /// </summary>
        int HealthCapacity { get; set; }

        /// <summary>
        /// Получить максимальное количество жизней юнита
        /// </summary>
        int HealthCapacityMax { get; }
    }
}

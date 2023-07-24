namespace Plugin.Interfaces.UnitComponents
{
    public interface IArmorComponent
    {
        /// <summary>
        /// Получить количество брони юнита
        /// </summary>
        int Capacity { get; set; }
        
        /// <summary>
        /// Получить максимальное количество брони юнита
        /// </summary>
        int CapacityMax { get; }
    }
}

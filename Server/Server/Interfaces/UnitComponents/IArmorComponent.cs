namespace Plugin.Interfaces.UnitComponents
{
    public interface IArmorComponent
    {
        /// <summary>
        /// Получить количество брони юнита
        /// </summary>
        int ArmorCapacity { get; set; }
        
        /// <summary>
        /// Получить максимальное количество брони юнита
        /// </summary>
        int ArmorCapacityMax { get; }
    }
}

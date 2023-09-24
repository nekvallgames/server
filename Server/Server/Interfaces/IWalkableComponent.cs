using Plugin.Tools;

namespace Plugin.Interfaces
{
    /// <summary>
    /// Повісити цей інтерфейс на юніта, що би юніт мав можливість переміщатися по ігровій сітці
    /// </summary>
    public interface IWalkableComponent
    {
        int WayMask { get; }
        Enums.WalkNavigation NavigationWay { get; set; }
    }
}

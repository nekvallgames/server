using Plugin.Tools;

namespace Plugin.Interfaces
{
    public interface IUnit
    {
        /// <summary>
        /// Id ігрової кімнати, до котрої поточний юніт відноситься
        /// </summary>
        string GameId { get; }

        /// <summary>
        /// Владелец юнита
        /// </summary>
        int OwnerActorNr { get; }

        /// <summary>
        /// ID юнита
        /// </summary>
        int UnitId { get; }

        /// <summary>
        /// Получить номер инстанса юнита
        /// </summary>
        int InstanceId { get; }

        /// <summary>
        /// Позиція на ігровій сітці
        /// </summary>
        Int2 Position { get; set; }

        /// <summary>
        /// Ширина та висота юніта
        /// </summary>
        Int2 BodySize { get; }

        /// <summary>
        /// Поточний юніт мертвий?
        /// </summary>
        bool IsDead { get; set; }
    }
}

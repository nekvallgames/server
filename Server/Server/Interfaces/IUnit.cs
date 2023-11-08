using Plugin.Schemes;
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
        /// Бінарна маска, котра буде зберігати 
        /// в собі дані про кожну частину тіла поточного юніта
        /// </summary>
        PartBodyScheme[] AreaGrid { get; }

        /// <summary>
        /// Поточний юніт мертвий?
        /// </summary>
        bool IsDead { get; set; }

        /// <summary>
        /// Рiвень юніта
        /// </summary>
        int Level { get; }

        bool IsHumanoid { get; }

        bool Compare(IUnit unit);
        bool Compare(int ownerActorNr, int unitId, int instanceId);
        bool Compare(int unitId, int instanceId);
    }
}

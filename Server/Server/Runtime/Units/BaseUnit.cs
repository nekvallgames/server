using Plugin.Interfaces;
using Plugin.Tools;

namespace Plugin.Runtime.Units
{
    public abstract class BaseUnit : IUnit
    {
        /// <summary>
        /// Id ігрової кімнати, до котрої поточний юніт відноситься
        /// </summary>
        public string GameId { get; protected set; }

        /// <summary>
        /// Владелец игрового юнита
        /// </summary>
        public int OwnerActorId { get; protected set; }

        /// <summary>
        /// ID юнита
        /// </summary>
        public int UnitId { get; protected set; }

        /// <summary>
        /// Получить номер инстанса юнита
        /// </summary>
        public int InstanceId { get; protected set; }

        /// <summary>
        /// Позиція на ігровій сітці
        /// </summary>
        public Int2 Position { get; set; }

        /// <summary>
        /// Ширина та висота юніта
        /// </summary>
        public abstract Int2 BodySize { get; }

        /// <summary>
        /// Поточний юніт мертвий?
        /// </summary>
        public bool IsDead { get; set; }

        

        public BaseUnit( string gameId, int ownerActorId, int unitId, int instanceUnitId )
        {
            GameId = gameId;
            OwnerActorId = ownerActorId;
            UnitId = unitId;
            InstanceId = instanceUnitId;

            LogChannel.Log($"Created unit ownerId = {OwnerActorId}, uId = {UnitId}, instance = {InstanceId}", LogChannel.Type.Default);
        }
    }
}

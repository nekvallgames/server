using Plugin.Interfaces;
using Plugin.Parameters;
using Plugin.Runtime.Services;
using Plugin.Schemes;
using Plugin.Schemes.Public;
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
        public int OwnerActorNr { get; protected set; }

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
        /// Бінарна маска, котра буде зберігати 
        /// в собі дані про кожну частину тіла поточного юніта
        /// </summary>
        public abstract PartBodyScheme[] AreaGrid { get; }
        
        /// <summary>
        /// Поточний юніт мертвий?
        /// </summary>
        public bool IsDead { get; set; }

        /// <summary>
        /// Рiвень юніта
        /// </summary>
        public int Level { get; private set; }
        

        protected UnitPublicScheme unitPublicScheme;
        protected IncreaseUnitDamageService increaseUnitDamageService;
        protected IncreaseUnitHealthService increaseUnitHealthService;

        public BaseUnit(UnitFactoryParameters parameters)
        {
            GameId = parameters.GameId;
            OwnerActorNr = parameters.OwnerActorNr;
            UnitId = parameters.UnitId;
            InstanceId = parameters.InstanceId;
            Level = parameters.Level;
 
            unitPublicScheme = parameters.UnitsPublicModelService.Get(UnitId);
            increaseUnitDamageService = parameters.IncreaseUnitDamageService;
            increaseUnitHealthService = parameters.IncreaseUnitHealthService;

            // LogChannel.Log($"Created unit ownerId = {OwnerActorNr}, uId = {UnitId}, instance = {InstanceId}, level = {Level}", LogChannel.Type.Default);
        }

        protected BaseUnit()
        {

        }
    }
}

using System.Collections.Generic;

namespace Plugin.Schemes
{
    /// <summary>
    /// В поточній схемі зберігаємо сели, по котрим може рухатись поточний юніт
    /// </summary>
    public class UnitPathPrivateScheme
    {
        public bool IsNull { get; private set; }

        public int OwnerActorId { get; }
        public int UnitId { get; }
        public int InstanceId { get; }

        /// <summary>
        /// Данні шляху, по котрому поточний юніт може переміщатися
        /// </summary>
        public List<(int, int)> Path { get; }

        public UnitPathPrivateScheme()
        { 

        }

        public UnitPathPrivateScheme(int ownerActorId,
                                     int unitId,
                                     int instanceId,
                                     List<(int, int)> path)
        {
            IsNull = false;
            OwnerActorId = ownerActorId;
            UnitId = unitId;
            InstanceId = instanceId;
            Path = path;
        }
    }
}

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
        public List<PathCellPrivateScheme> Path { get; }

        /// <summary>
        /// Список селлів, по котрим поточний юніт може переміщатися
        /// </summary>
        public List<Cell> Cells { get; }

        public static UnitPathPrivateScheme Null
        {
            get
            {
                UnitPathPrivateScheme pathSchemeUnit = new UnitPathPrivateScheme();
                pathSchemeUnit.IsNull = true;

                return pathSchemeUnit;
            }
        }

        public UnitPathPrivateScheme()
        { 

        }

        public UnitPathPrivateScheme(int ownerActorId,
                                     int unitId,
                                     int instanceId,
                                     List<PathCellPrivateScheme> path,
                                     List<Cell> cells)
        {
            IsNull = false;
            OwnerActorId = ownerActorId;
            UnitId = unitId;
            InstanceId = instanceId;
            Path = path;
            Cells = cells;
        }
    }
}

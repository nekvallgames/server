using Plugin.Schemes;
using System.Linq;

namespace Plugin.Runtime.Services
{
    /// <summary>
    /// Сервіс, за допомогою котрого будемо реалізовувати манімапуляції із level юнітом
    /// cost - це ціна прокачки в софт валюті для прокачки юніта на вказаний level
    /// capacity - це кількість карт, котрі гравець повинен потратити для прокачки юніта на вказаний level
    /// </summary>
    public class UnitLevelService
    {
        private const string jsonName = "upgrade_unit_level_cost";

        private UpgradeUnitLevelCostPublicScheme _data;

        public UnitLevelService(JsonReaderService jsonReaderService)
        {
            _data = jsonReaderService.Read<UpgradeUnitLevelCostPublicScheme>(jsonName);
        }

        /// <summary>
        /// Отримати level юніта, вказавши id та capacity
        /// </summary>
        public int GetLevel(int unitId, int capacity)
        {
            foreach (CapacitiesScheme capacities in _data.Capacities)
            {
                if (!capacities.ids.Any(x => x == unitId))
                    continue;

                int level = 0;
                foreach (int levelCapacity in capacities.capacity)
                {
                    if (capacity < levelCapacity)
                        break;

                    level++;
                }

                return level - 1;
            }

            return 0;
        }
    }
}

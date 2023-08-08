using Plugin.Schemes.Public;
using System.Linq;

namespace Plugin.Runtime.Services
{
    public class IncreaseUnitHealthService
    {
        private const string jsonName = "increase_unit_health";

        private IncreaseUnitHealthPublicScheme[] _data;

        public IncreaseUnitHealthService(JsonReaderService jsonReaderService)
        {
            _data = jsonReaderService.Read<IncreaseUnitHealthPublicScheme[]>(jsonName);
        }

        /// <summary>
        /// Отримати для вказаного юніта по рівню,
        /// на скільки потрібно збільшити кількість рівня життів
        /// Тобто із кожним level-ом ігрового юніта в нього буде збільшуватись кількість жіттів
        /// </summary>
        public int GetAdditionalHealthByLevel(int unitId, int level)
        {
            int capacity = 0;

            if (!_data.Any(x => x.ids.Any(j => j == unitId)))
                return capacity;

            IncreaseUnitHealthPublicScheme scheme =
                _data.First(x => x.ids.Any(j => j == unitId));

            for (int i = 0; i <= level; i++)
            {
                capacity += scheme.capacity[i];
            }

            return capacity;
        }
    }

}

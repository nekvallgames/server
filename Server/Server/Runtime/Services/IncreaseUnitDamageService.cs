using Plugin.Schemes.Public;
using System.Linq;

namespace Plugin.Runtime.Services
{
    /// <summary>
    /// Сервіс, котрий збільшить урон від damage при створенні юніта, в залежності від level юніта
    /// </summary>
    public class IncreaseUnitDamageService
    {
        private const string jsonName = "increase_unit_damage";

        private IncreaseUnitDamagePublicScheme[] _data;

        public IncreaseUnitDamageService(JsonReaderService jsonReaderService)
        {
            _data = jsonReaderService.Read<IncreaseUnitDamagePublicScheme[]>(jsonName);
        }

        /// <summary>
        /// Отримати для вказаного юніта по рівню,
        /// на скільки потрібно збільшити кількість урона від одного пострілу
        /// Тобто із кожним level-ом ігрового юніта в нього буде збільшуватись кількість урона
        /// </summary>
        public int GetAdditionalDamageByLevel(int unitId, int level)
        {
            int capacity = 0;

            if (!_data.Any(x => x.ids.Any(j => j == unitId)))
                return capacity;

            IncreaseUnitDamagePublicScheme scheme =
                _data.First(x => x.ids.Any(j => j == unitId));

            for (int i = 0; i <= level; i++)
            {
                capacity += scheme.capacity[i];
            }

            return capacity;
        }
    }
}

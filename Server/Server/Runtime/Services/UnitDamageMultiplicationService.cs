using Plugin.Schemes.Public;
using Plugin.Tools;

namespace Plugin.Runtime.Services
{
    /// <summary>
    /// Сервіс, за допомогою котрого буде збільшуватися або зменшуватися урон по тушкі юніта
    /// </summary>
    public class UnitDamageMultiplicationService
    {
        private const string jsonName = "unit_damage_multiplication";

        private UnitMultiplicationPublicScheme _data;

        public UnitDamageMultiplicationService(JsonReaderService jsonReaderService)
        {
            _data = jsonReaderService.Read<UnitMultiplicationPublicScheme>(jsonName);
        }

        public float Get(Enums.PartBody partBody)
        {
            switch (partBody)
            {
                case Enums.PartBody.head:
                    {
                        return _data.head;
                    }
                    break;

                case Enums.PartBody.body:
                    {
                        return _data.body;
                    }
                    break;

                case Enums.PartBody.bottom:
                    {
                        return _data.bottom;
                    }
                    break;
            }

            return 0;
        }
    }
}

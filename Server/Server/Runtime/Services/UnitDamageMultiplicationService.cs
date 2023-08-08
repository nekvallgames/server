using Plugin.Schemes.Public;

namespace Plugin.Runtime.Services
{
    public class UnitDamageMultiplicationService
    {
        private const string jsonName = "unit_damage_multiplication";

        private UnitMultiplicationPublicScheme _data;

        public UnitDamageMultiplicationService(JsonReaderService jsonReaderService)
        {
            _data = jsonReaderService.Read<UnitMultiplicationPublicScheme>(jsonName);
        }
    }
}

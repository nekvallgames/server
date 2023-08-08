using Plugin.Schemes.Public;

namespace Plugin.Runtime.Services
{
    public class UnitsPublicModelService
    {
        private const string jsonName = "increase_unit_damage";

        private UnitsPublicScheme _data;

        public UnitsPublicModelService(JsonReaderService jsonReaderService)
        {
            _data = jsonReaderService.Read<UnitsPublicScheme>(jsonName);
        }

        public UnitPublicScheme Get(int unitId)
        {
            return _data.Find(x => x.id == unitId);
        }
    }
}

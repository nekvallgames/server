﻿using Plugin.Schemes.Public;

namespace Plugin.Runtime.Services
{
    /// <summary>
    /// Сервіс, за допомогою котрого можно отримати публічні данні юніта
    /// </summary>
    public class UnitsPublicModelService
    {
        private const string jsonName = "units";

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

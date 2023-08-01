using Newtonsoft.Json;
using System;
using System.IO;

namespace Plugin.Runtime.Services
{
    /// <summary>
    /// Сервіс, за допомогою котрого будемо читати Jsons, котрі знаходяться в корні проєкта
    /// </summary>
    public class JsonReaderService
    {
        private const string pathToJson = "\\deploy\\Plugins\\Plugin\\bin\\Resources\\Jsons\\";

        public T Read<T>(string jsonName)
        {
            string workingDirectory = Environment.CurrentDirectory;
            string path = Directory.GetParent(workingDirectory).Parent.FullName + pathToJson + jsonName + ".json";

            if (!File.Exists(path))
            {
                new NullReferenceException($"JsonReaderService :: Read() I can't read json: {jsonName}");
            }

            return JsonConvert.DeserializeObject<T>(File.ReadAllText(path));
        }
    }
}

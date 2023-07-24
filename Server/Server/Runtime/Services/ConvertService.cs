using Newtonsoft.Json;

namespace Plugin.Runtime.Services
{
    /// <summary>
    /// Сервіс, котрий буде допомогати серіалізувати/десереалізувати дані
    /// </summary>
    public class ConvertService
    {
        public string SerializeObject(object data)
        {
            return JsonConvert.SerializeObject(data);
        }

        public T DeserializeObject<T>(string value)
        {
            return JsonConvert.DeserializeObject<T>(value);
        }   
    }
}

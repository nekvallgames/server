using Plugin.Interfaces;
using System.Collections.Generic;

namespace Plugin.Runtime.Providers
{
    public class PrivateModelProvider
    {
        private List<IPrivateModel> _models;

        public PrivateModelProvider(List<IPrivateModel> models)
        {
            _models = models;
        }

        public T Get<T>() where T : class, IPrivateModel
        {
            return _models.Find(x => x is T) as T;
        }
    }
}

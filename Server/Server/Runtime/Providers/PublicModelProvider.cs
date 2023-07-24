using Plugin.Interfaces;
using System.Collections.Generic;

namespace Plugin.Runtime.Providers
{
    public class PublicModelProvider
    {
        private List<IPublicModel> _models;

        public PublicModelProvider(List<IPublicModel> models)
        {
            _models = models;

            foreach (IPublicModel model in _models){
                model.Parse();
            }
        }

        public T Get<T>() where T : class, IPublicModel
        {
            return _models.Find(x => x is T) as T;
        }
    }
}

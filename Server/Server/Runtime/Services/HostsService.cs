using Photon.Hive.Plugin;
using Plugin.Models.Private;
using System.Collections.Generic;

namespace Plugin.Runtime.Services
{
    /// <summary>
    /// Сервіс, котрий буде маніпулювати хостами ігрових кімнат
    /// </summary>
    public class HostsService
    {
        private HostsPrivateModel _model;

        public HostsService(HostsPrivateModel model)
        {
            _model = model;
        }

        public void Add(IPluginHost host)
        {
            _model.Add(host);
        }

        public IPluginHost Get(string gameId)
        {
            return _model.Items.Find(x => x.GameId == gameId);
        }

        /// <summary>
        /// Отримати акторів, котрі знаходяться в ігровій кімнаті
        /// </summary>
        public IList<IActor> Actors(string gameId)
        {
            return Get(gameId).GameActorsActive;
        }

        /// <summary>
        /// Вказаний актор є участником даної кімнати?
        /// </summary>
        public bool IsMemberHost(IPluginHost host, string gameId)
        {
            return host.GameId == gameId;
        }
    }
}

using Photon.Hive.Plugin;
using Plugin.Interfaces;
using Plugin.Plugins.PVP;
using Plugin.Runtime.Providers;

namespace Plugin
{
    /// <summary>
    /// Фабрика по созданию плагинов
    ///
    /// При создании комнаты, нужно указать имя плагина,
    /// который будет отвечать за логику поведения комнаты
    /// </summary>
    public class PluginFactory : PluginFactoryBase
    {
        private IBackendBroadcastProvider _backendBroadcastProvider;

        public PluginFactory()
        {
            _backendBroadcastProvider = new BackendBroadcastProvider();
            _backendBroadcastProvider.Connect();
        }
        
        public override IGamePlugin CreatePlugin(string pluginName)
        {
            switch (pluginName){
                case PVPPlugin.NAME:
                    {
                        return new PVPPlugin();
                    }
                    break;
            }

            return null;
        }
    }
}

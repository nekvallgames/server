using Photon.Hive.Plugin;
using Plugin.Installers;
using Plugin.Plugins.PVP;
using Plugin.Runtime.Providers;
using System;
using System.Diagnostics;

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
        private BackendBroadcastProvider _backendBroadcastProvider;

        public PluginFactory()
        {
            
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

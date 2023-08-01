using Photon.Hive.Plugin;
using Plugin.Installers;
using Plugin.Plugins.PVP;

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
        public PluginFactory()
        {
            GameInstaller.GetInstance();
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

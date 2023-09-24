using System.Diagnostics;

namespace Plugin.Tools
{
    /// <summary>
    /// Класс, который реализовыват логирование в редакторе
    /// </summary>
    public static class LogChannel
    {
        public enum Type
        {
            Default,                // логирование по умолчанию
            Plot,                   // логирование игрового сценария
            ExecuteAction,          // логирование выполнить действие 
            Error
        }

        /// <summary>
        /// Логировать сообщение в дебаг режиме
        /// </summary>
        public static void Log(string message, Type typeLog = Type.Default)
        {
            if (typeLog == Type.ExecuteAction)
            {
                // return;
            }

            if (typeLog == Type.Plot)
            {
                // return;
            }

            Debug.WriteLine(message);

            //  _pluginHost.LogDebug(message);
        }
    }
}

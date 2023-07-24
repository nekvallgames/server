using Photon.Hive.Plugin;

namespace Plugin.Signals
{
    /// <summary>
    /// Подія, коли модель із даними хостів ігрових кімнат була змінена
    /// </summary>
    public class HostsPrivateModelSignal : ModelChangeSignal
    {
        public string GameId;

        public HostsPrivateModelSignal(string gameId, StatusType status) : base(status)
        {
            GameId = gameId;
        }
    }
}

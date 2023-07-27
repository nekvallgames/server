using System.Collections.Generic;
using System.Threading.Tasks;

namespace Plugin.Interfaces
{
    public interface IBackendBroadcastProvider
    {
        /// <summary>
        /// Підключитися до DataBace
        /// </summary>
        void Connect();

        /// <summary>
        /// Отримати юнітів, котрі вибрані гравцем
        /// Тобто отримати юнітів, котрі в гравця вибрані в колоді
        /// </summary>
        Task<List<int>> GetUnitsInDeck(string profileId);
    }
}

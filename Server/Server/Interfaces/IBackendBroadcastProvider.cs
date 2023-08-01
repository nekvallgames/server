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
        /// Отримати дані вказаного актора із database
        /// item1 - рейтинг гравця
        /// item2 - id юнітів, котрі в гравця знаходяться в колоді
        /// </summary>
        Task<(int, List<int>)> GetActorData(string profileId);
        /// <summary>
        /// Отримати ownCapacity юнітів вказаного гравця
        /// </summary>
        Task<List<(int, int)>> GetOwnCapacityUnits(string profileId);
        /// <summary>
        /// Перезаписати рейтинг вказаного гравця
        /// </summary>
        Task SetRating(string profileId, int capacity);
    }
}

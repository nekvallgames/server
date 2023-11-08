using System.Collections.Generic;

namespace Plugin.Interfaces
{
    public interface IActorScheme
    {
        /// <summary>
        /// Id ігрової кімнати, в котрій знаходяться гравці
        /// </summary>
        string GameId { get; }
        /// <summary>
        /// Id гравця в ігрової кімнаті
        /// </summary>
        int ActorNr { get; }
        /// <summary>
        /// Id гравця для синхронізації із DB
        /// </summary>
        string ProfileId { get; }
        /// <summary>
        /// Рейтинг актора
        /// </summary>
        int Rating { get; set; }
        /// <summary>
        /// Is actor left from room?
        /// </summary>
        bool IsLeft { get; set; }
        /// <summary>
        /// Поточний актор являється ботом?
        /// </summary>
        bool IsAI { get; }
        /// <summary>
        /// Id юнітів, котрими буде грати гравець
        /// </summary>
        List<int> Deck { get; set; }
        /// <summary>
        /// Levels юнітів, котрі знаходяться в колоді Deck
        /// </summary>
        List<int> Levels { get; set; }
    }
}

using System.Collections.Generic;

namespace Plugin.Interfaces
{
    public interface IPlotModelScheme
    {
        /// <summary>
        /// Вказати id ігрової кімнати
        /// </summary>
        string GameId { get; }

        /// <summary>
        /// Поточний крок синхронізації ігрового сценарія
        /// </summary>
        int SyncStep { get; set; }

        /// <summary>
        /// Поточний ігровий мод
        /// </summary>
        int GameMode { get; set; }

        /// <summary>
        /// Гравці дограли до кінця і є переможець?
        /// </summary>
        bool IsGameFinished { get; set; }

        /// <summary>
        /// Чи була гра призупинена? Наприклад один із гравців покинув ігрову кімнату
        /// </summary>
        bool IsAbort { get; set; }

        /// <summary>
        /// Номер гравців в ігровій кімнаті, хто отримав перемогу
        /// </summary>
        List<int> WinnerActorsNr { get; set; }

        /// <summary>
        /// Чи потрібно перевіряти юніта, корректна в нього позиція чи ні? 
        /// </summary>
        bool IsNeedToCheckOnCorrectPosition { get; set; }
    }
}

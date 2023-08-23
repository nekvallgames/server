using System;

namespace Plugin.Interfaces
{
    public interface IMode
    {
        int ModeId { get; }

        /// <summary>
        /// Виконати логіку мода
        /// success - виконається, при успішному виконанні всієї логіки, котра була закладена в моді
        /// </summary>
        void ExecuteMode(Action success);
    }
}

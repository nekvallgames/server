using Plugin.Tools;
using System.Collections.Generic;

namespace Plugin.Interfaces
{
    /// <summary>
    /// Інтерфейс для групи із компонентів, котра потрібна для подальшої синхронізації із Game Server
    /// </summary>
    public interface IGroupSyncComponents
    {
        /// <summary>
        /// Тип групи. Що саме синхронізуємо?
        /// </summary>
        Enums.SyncGroup SyncGroup { get; }
        /// <summary>
        /// Компоненти, з котрих формується синхронізація поточної дії юніта
        /// </summary>
        List<ISyncComponent> SyncComponents { get; }
    }
}

using Plugin.Interfaces.UnitComponents;
using Plugin.Tools;

namespace Plugin.Interfaces.Actions
{
    /// <summary>
    /// Выполнить действие - бросить гранату
    /// Интерфейс с гранатой разширяет интерфейс с огнестрельным оружием
    /// </summary>
    public interface IWaveDamageAction : IDamageAction
    {
        /// <summary>
        /// Получить урон от гранаты на 1-й волне
        /// </summary>
        int[] GetWaveDamage();
    }
}

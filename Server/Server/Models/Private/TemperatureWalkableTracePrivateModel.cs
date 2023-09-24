using Plugin.Interfaces;
using Plugin.Templates;

namespace Plugin.Models.Private
{
    /// <summary>
    /// Модель із даними, котра буде зберігати в собі дані із температурним слідом переміщення юнітів гравця
    /// Це потрібно для роботи AI
    /// </summary>
    public class TemperatureWalkableTracePrivateModel : BaseModel<ITemperatureWalkableTraceScheme>, IPrivateModel
    {

    }
}

using Plugin.Interfaces;
using Plugin.Models.Private;

namespace Plugin.Runtime.Services
{
    /// <summary>
    /// Сервіс, за допомогою котрого будуть виконуватись маніпуляції із PlotsPrivateModel
    /// </summary>
    public class PlotsModelService
    {
        private PlotsPrivateModel _model;

        public PlotsModelService(PlotsPrivateModel model)
        {
            _model = model;
        }

        public void Add(IPlotModelScheme model)
        {
            _model.Add(model);
        }

        public IPlotModelScheme Get(string gameId, int actorId)
        {
            return _model.Items.Find(x => x.GameId == gameId && x.OwnerActorId == actorId);
        }
    }
}

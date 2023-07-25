using Plugin.Interfaces;
using Plugin.Models.Private;
using System.Linq;

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

        public IPlotModelScheme Get(string gameId)
        {
            return _model.Items.Find(x => x.GameId == gameId);
        }

        public bool Has(string gameId)
        {
            return _model.Items.Any(x => x.GameId == gameId);
        }

        public bool Remove(string gameId)
        {
            return _model.Items.Remove(Get(gameId));
        }
    }
}

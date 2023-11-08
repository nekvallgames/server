using Plugin.Schemes;
using Plugin.Tools;

namespace Plugin.Runtime.Services
{
    public class DeserializeStepService
    {
        private OpStockService _opStockService;
        private ActorStepsService _actorStepsService;
        private ConvertService _convertService;

        public DeserializeStepService(OpStockService opStockService, 
                                      ActorStepsService actorStepsService, 
                                      ConvertService convertService)
        {
            _opStockService = opStockService;
            _actorStepsService = actorStepsService;
            _convertService = convertService;
        }

        /// <summary>
        /// Нужно десериализировать операции, которые прислал игрок на Game Server
        /// </summary>
        public void DeserializeOp(string gameId, int actorNr)
        {
            if (!_opStockService.HasOp(gameId, actorNr, OperationCode.syncStep))
                return;

            var stepData = _opStockService.TakeOp(gameId, actorNr, OperationCode.syncStep);

            var stepScheme = _convertService.DeserializeObject<StepScheme>(stepData.Data.ToString());

            _actorStepsService.AddStepScheme(gameId, actorNr, stepScheme);
        }
    }
}

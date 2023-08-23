using Photon.Hive.Plugin;
using Plugin.Builders;
using Plugin.Interfaces;
using Plugin.Schemes;
using Plugin.Tools;
using System.Collections.Generic;
using System.Linq;

namespace Plugin.Runtime.Services
{
    /// <summary>
    /// Сервіс, котрий буде синхронізувати виконані дії гравців між собою
    /// Беремо дані синхронізації із моделі SyncPrivateModel, запихуємо всі дані в StepScheme
    /// і відправляємо ці дані акторам
    /// </summary>
    public class SyncStepService
    {
        private StepSchemeBuilder _stepSchemeBuilder;
        private ConvertService _convertService;
        private HostsService _hostsService;
        private PlotsModelService _plotsModelService;
        private ActorService _actorService;

        public SyncStepService(StepSchemeBuilder stepSchemeBuilder,
                               ConvertService convertService,
                               HostsService hostsService,
                               PlotsModelService plotsModelService,
                               ActorService actorService)
        {
            _stepSchemeBuilder = stepSchemeBuilder;
            _convertService = convertService;
            _hostsService = hostsService;
            _actorService = actorService;

            _plotsModelService = plotsModelService;
        }

        public void Sync(IPluginHost host, int[] syncSteps)
        {
            IPlotModelScheme plotModel = _plotsModelService.Get(host.GameId);

            // Создать коллекцию, которая будет хранить в себе данные, которые нужно синхронизировать 
            // между клиентами
            // key   - это ActorID
            // value - это StepScheme, которая имеет кучу из компонентов
            var pushData = new Dictionary<byte, object> { };

            // Зібрати синхронізацію дій акторів і відправити результат їхній дій всім акторам в кімнаті
            foreach (IActorScheme actor in _actorService.GetActorsInRoom(host.GameId))
            {
                var scheme = new StepResultScheme { 
                    gameMode = plotModel.GameMode,
                    isWin = plotModel.IsGameFinished ? plotModel.WinnerActorsNr.Any(x => x == actor.ActorNr) : false,
                    rating = actor.Rating,
                    stepScheme = _stepSchemeBuilder.Create(host.GameId, actor.ActorNr, syncSteps)
                };

                string jsonString = _convertService.SerializeObject(scheme);
                pushData.Add((byte)actor.ActorNr, jsonString);
            }

            host.BroadcastEvent(ReciverGroup.All,                   // отправить сообщение всем
                                0,                                  // номер актера, если нужно отправить уникальное сообщение
                                0,
                                OperationCode.stepResult,
                                pushData,
                                CacheOperations.DoNotCache);        // не кэшировать сообщение
        }
    }
}

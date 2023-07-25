using Photon.Hive.Plugin;
using Plugin.Builders;
using Plugin.Schemes;
using Plugin.Tools;
using System.Collections.Generic;

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

        public SyncStepService(StepSchemeBuilder stepSchemeBuilder,
                               ConvertService convertService,
                               HostsService hostsService)
        {
            _stepSchemeBuilder = stepSchemeBuilder;
            _convertService = convertService;
            _hostsService = hostsService;
        }

        public void Sync(IPluginHost host, int[] syncSteps)
        {
            // Создать коллекцию, которая будет хранить в себе данные, которые нужно синхронизировать 
            // между клиентами
            // key   - это ActorID
            // value - это StepScheme, которая имеет кучу из компонентов
            var pushData = new Dictionary<byte, object> { };

            // Зібрати синхронізацію дій акторів і відправити результат їхній дій всім акторам в кімнаті
            foreach (IActor actor in _hostsService.GetActors(host.GameId))
            {
                StepScheme scheme = _stepSchemeBuilder.Create(host.GameId, actor.ActorNr, syncSteps);
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

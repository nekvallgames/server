using Plugin.Interfaces;
using Plugin.Runtime.Services.Sync;
using Plugin.Schemes;
using System.Collections.Generic;

namespace Plugin.Builders
{
    /// <summary>
    /// Білдер, котрий створить StepScheme дій актора, котрі ним були виконані в вказані syncSteps
    /// </summary>
    public class StepSchemeBuilder
    {
        private SyncService _syncService;

        public StepSchemeBuilder( SyncService syncService )
        {
            _syncService = syncService;
        }

        /// <summary>
        /// Створити StepScheme для вказаних кроків синхронізації
        /// syncSteps - номери ігрових кроків, наприклад [0, 1], [2, 3] і т.д.
        /// </summary>
        public StepScheme Create(string gameId, int actorId, int[] syncSteps)
        {
            var scheme = new StepScheme();

            for (int i = 0; i < syncSteps.Length; i++)
            {
                List<ISyncGroupComponent> syncGroups = _syncService.Get(gameId, actorId, syncSteps[i]).SyncGroups;

                foreach (ISyncGroupComponent syncGroup in syncGroups)
                {
                    foreach (ISyncComponent component in syncGroup.SyncElements){
                        scheme.Add(component);
                    }
                }
            }

            return scheme;
        }
    }
}

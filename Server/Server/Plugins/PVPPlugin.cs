using Photon.Hive.Plugin;
using Plugin.Interfaces;
using Plugin.Runtime.Services.PlotStates;
using Plugin.Runtime.Services.PlotStates.States;
using Plugin.Schemes;

namespace Plugin.Plugins.PVP
{
    /// <summary>
    /// Плагин для режиму PVP
    /// </summary>
    public class PVPPlugin : PluginHook
    {
        public const string NAME = "PVPPlugin";

        /// <summary>
        /// Имя плагина. Что бы текущий плагин выполнял логику созданой комнаты, 
        /// нужно имя плагина передать в параметрах, при создании комнаты
        /// </summary>
        public override string Name => NAME; // anything other than "Default" or "ErrorPlugin"

        /// <summary>
        /// Реалізувати логіку плагіна
        /// </summary>
        protected override void PluginImplementation()
        {
            // Для поточної ігрової кімнати створити стейт машину із стейтами,
            // котрі потрібні для ігрового режиму 
            plotService = new PlotStatesService();
            plotService.Add(new IState[] { new AccumulateState(plotService, host, 2, SyncStartState.NAME),
                                           new SyncStartState(plotService, host, 2, WaitStepResult.NAME),
                                           new WaitStepResult(plotService, host, 2, SyncState.NAME),
                                           new SyncState(plotService, host, WaitStepResult.NAME)
                                          });

            // запустити ігровий сценарій
            plotService.ChangeState(AccumulateState.NAME);
        }

        /// <summary>
        /// Игрок на стороне клиента отправил ивент "Создать комнату" и при успешном создании комнаты на 
        /// стороне GameServer выполнится текущий метод
        /// </summary>
        public override void OnCreateGame(ICreateGameCallInfo info)
        {
            base.OnCreateGame(info);

            // CreateActorScheme(host.GameId, !info.IsJoin ? 1 : info.Request.ActorNr);

            if (!plotsModelService.Has(host.GameId))
            {
                plotsModelService.Add(new PVPPlotModelScheme(host.GameId));
            }
        }

        /// <summary>
        /// Игрок на стороне клиента отправил ивент "Присоединится к текущей комнате" и при успешном 
        /// присоеденении к комнате на стороне GameServer выполнится текущий метод
        /// </summary>
        public override void OnJoin(IJoinGameCallInfo info)
        {
            base.OnJoin(info);

            // CreateActorScheme(host.GameId, info.ActorNr);
        }

        /// <summary>
        /// Створити модель даних для зберігання даних ігрового режиму,
        /// для нового гравця поточної ігрової кімнати
        /// </summary>
        // private void CreateActorScheme(string gameId, int actorId)
        // {
        //     // IActor actor = GameInstaller.GetInstance().hostsService.GetActor(gameId, actorId);
        //     // var profileId = actor.Properties.GetProperty("id");
        // 
        //     я отут закінчив
        // 
        //     //IList<IActor> actors = GameInstaller.GetInstance().hostsService.GetActors(gameId);
        //     //actors[0].Properties
        // 
        // 
        // 
         //    GameInstaller.GetInstance().plotsModelService.Add(new PVPPlotModelScheme(gameId, actorId));
        // }

    }
}

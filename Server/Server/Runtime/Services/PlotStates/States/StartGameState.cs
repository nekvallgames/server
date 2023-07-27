using Photon.Hive.Plugin;
using Plugin.Installers;
using Plugin.Interfaces;
using Plugin.Schemes;
using Plugin.Tools;
using System.Collections.Generic;

namespace Plugin.Runtime.Services.PlotStates.States
{
    public class StartGameState : BasePlotState, IState
    {
        public const string NAME = "StartGameState";
        public string Name => NAME;

        private ActorService _actorService;
        private ConvertService _convertService;

        public StartGameState(PlotStatesService plotStatesService,
                              IPluginHost host,
                              string nextState) : base(plotStatesService, host, nextState)
        {
            var gameInstaller = GameInstaller.GetInstance();

            _actorService = gameInstaller.actorService;
            _convertService = gameInstaller.convertService;
        }

        public void EnterState()
        {
            LogChannel.Log("PlotStatesService :: StartGameState :: EnterState()", LogChannel.Type.Plot);

            SendStartGame();

            plotStatesService.ChangeState(nextState);
        }

        /// <summary>
        /// Відправити клієнтам сигнал, що гра стартувала
        /// </summary>
        private void SendStartGame()
        {
            // Создать коллекцию, которая будет хранить в себе данные
            // между клиентами
            // key   - это ActorID
            // value - это ChoosedUnitsScheme, которая имеет ID юнитов, которыми будут играть игроки
            Dictionary<byte, object> pushData = new Dictionary<byte, object> { };

            List<IActorScheme> actors = _actorService.GetActorsInRoom(host.GameId);

            foreach (IActorScheme actor in actors)
            {
                var choosedUnitsScheme = new ChoosedUnitsScheme(){
                    unitsId = new List<int>()
                };

                foreach (int unitId in actor.Deck)
                {
                    if (unitId == -1)
                        continue;

                    choosedUnitsScheme.unitsId.Add(unitId);
                }

                string jsonString = _convertService.SerializeObject(choosedUnitsScheme);

                pushData.Add((byte)actor.ActorNr, jsonString);
            }

            host.BroadcastEvent(ReciverGroup.All,                   // отправить сообщение всем
                                0,                                  // номер актера, если нужно отправить уникальное сообщение
                                0,
                                OperationCode.startGame,
                                pushData,
                                CacheOperations.DoNotCache);        // не кэшировать сообщение
        }

        public void ExitState()
        {
            
        }
    }
}

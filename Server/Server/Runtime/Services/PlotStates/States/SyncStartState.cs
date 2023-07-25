using Google.Cloud.Firestore;
using Photon.Hive.Plugin;
using Plugin.Installers;
using Plugin.Interfaces;
using Plugin.Interfaces.UnitComponents;
using Plugin.Schemes;
using Plugin.Signals;
using Plugin.Tools;
using System;
using System.Collections.Generic;

namespace Plugin.Runtime.Services.PlotStates.States
{
    /// <summary>
    /// Стейт, в котрому потрібно між клієнтами синхронізувати вибраних юнітів
    /// </summary>
    public class SyncStartState : BasePlotState, IState
    {
        public const string NAME = "SyncStartState";
        public string Name => NAME;

        private SignalBus _signalBus;
        private UnitsService _unitsService;
        private ConvertService _convertService;
        private HostsService _hostsService;

        /// <summary>
        /// Кількість гравців, котрі потрібні для старту ігрової кімнати
        /// </summary>
        private int _countActors;
        /// <summary>
        /// Наступний стейт, в котрий перейдемо після поточного стейту
        /// </summary>
        private string _nextState;

        private int _expectedCount = 0;
        private bool _isIgnoreSignal;

        public SyncStartState(PlotStatesService plotStatesService,
                              IPluginHost host, 
                              int countActors, 
                              string nextState):base(plotStatesService, host)
        {
            _countActors = countActors;
            _nextState = nextState;

            var gameInstaller = GameInstaller.GetInstance();

            _signalBus = gameInstaller.signalBus;
            _unitsService = gameInstaller.unitsService;
            _convertService = gameInstaller.convertService;
            _hostsService = gameInstaller.hostsService;
        }

        public void EnterState()
        {
            LogChannel.Log("PlotStatesService :: SyncStartState :: EnterState()", LogChannel.Type.Plot);
            _isIgnoreSignal = false;

            // Слухаємо оновлення моделі із операціями акторів
            _signalBus.Subscrible<OpStockPrivateModelSignal>( OnOpStockChange );

            // Отправляем всем актерам операцию, что бы они выбрали юнитов
            host.BroadcastEvent(ReciverGroup.All,                   // отправить сообщение всем
                                0,                                  // номер актера, если нужно отправить уникальное сообщение
                                0,
                                OperationCode.selectUnitsForGame,
                                null,
                                CacheOperations.DoNotCache);        // не кэшировать сообщение
        }

        /// <summary>
        /// Слухаємо оновлення моделі із операціями акторів
        /// </summary>
        private void OnOpStockChange(OpStockPrivateModelSignal signalData)
        {
            if (signalData.OpCode == OperationCode.choosedUnitsForGame 
                && signalData.Status == OpStockPrivateModelSignal.StatusType.remove
                && _hostsService.IsMemberHost(host, signalData.GameId))
            {
                _expectedCount++;
            }

            if (_expectedCount == _countActors && !_isIgnoreSignal)
            {
                _isIgnoreSignal = true;
                // Відправити акторам сигнал, що гра стартонула
                SendStartGame();

                plotStatesService.ChangeState(_nextState);
            }
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

            foreach (IActor actor in _hostsService.GetActors(host.GameId))
            {
                var choosedUnitsScheme = new ChoosedUnitsScheme(){
                    unitsId = new List<int>()
                };

                List<IUnit> unitsList = _unitsService.GetUnits(host.GameId, actor.ActorNr);

                foreach (IUnit unit in unitsList)
                {
                    if (typeof(IIgnoreSyncComponent).IsAssignableFrom(unit.GetType()))
                        continue;

                    choosedUnitsScheme.unitsId.Add(unit.UnitId);
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
            _isIgnoreSignal = true;
            _signalBus.Unsubscrible<OpStockPrivateModelSignal>(OnOpStockChange);
        }
    }
}

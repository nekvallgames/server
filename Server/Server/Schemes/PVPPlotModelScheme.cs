using Plugin.Interfaces;
using Plugin.Runtime.Services.PlotMode.States.PVP;
using Plugin.Tools;
using System.Collections.Generic;

namespace Plugin.Schemes
{
    /// <summary>
    /// Схема, котра буде зберігати в данні ігрового сценарія PVP
    /// Поточна схема буде лежати в моделі PlotsPrivateModel
    /// </summary>
    public class PVPPlotModelScheme : IPlotModelScheme
    {
        /// <summary>
        /// Вказати id ігрової кімнати, в котрій знаходиться актер
        /// </summary>
        public string GameId { get; }

        /// <summary>
        /// Поточний крок синхронізації ігрового сценарія
        /// </summary>
        public int SyncStep { get; set; }

        /// <summary>
        /// Текущее игровое состояние
        /// </summary>
        public string GameMode { get; set; }

        /// <summary>
        /// Время, на выполнение шага First Move
        /// </summary>
        public int FirstMoveTimeDuration { get; private set; }

        /// <summary>
        /// Время, на выполнение шага First Attack
        /// </summary>
        public int FirstAttackTimeDuration { get; private set; }

        /// <summary>
        /// Время, которое локальный игрок секономил,
        /// выделеное ему на выполнение шага перемещения
        /// </summary>
        public int FirstMoveSaveTime { get; set; }

        /// <summary>
        /// Сохранить время, когда стартовал стейт FirstMove
        /// </summary>
        public int StartFirstMoveTime { get; set; }

        /// <summary>
        /// Нужно спрятать VIP атрибуты для вражеского юнита?
        /// </summary>
        public bool IsNeedToHideVipElements { get; set; }

        /// <summary>
        /// Нужно отобразить VIP атрибуты для вражеского юнита?
        /// </summary>
        public bool IsNeedToShowVipElements { get; set; }

        /// <summary>
        /// Гравець має тільки одного юніта в команді
        /// Этот последний юнит получил последнюю помощь?
        /// (Помощь заключается в супер уроне и перемещении по всей карте)
        /// List<ActorNr>
        /// </summary>
        public List<int> HasLastUnitsHelp { get; set; }

        /// <summary>
        /// Чи був створений vip? Vip потрібно створити тільки 1 раз для
        /// обох команд при ігровому режиму FightWithVipMode
        /// </summary>
        public bool WasPreparedVipMode { get; set; }

        /// <summary>
        /// Чи були виконані підготовчі роботи для ігрового режиму дуєль?
        /// </summary>
        public bool WasPreparedDuel { get; set; }

        private int _fightToFirstDeadStage_countGame;    // Количество игр, сыграных на этапе FightToFirstDeadStage
        private int _fightWithVipStage_countGame;        // Количество игр, сыграных на этапе FightWithVipStage
        private int _duelStage_countGame;                // Количество игр, сыграных на этапе DuelStage

        public PVPPlotModelScheme(string gameId)
        {
            GameId = gameId;
            GameMode = PVPFightToFirstDeadMode.NAME;
        }

        /// <summary>
        /// Увеличить каунтер сыграных игр на текущем игровом этапе
        /// </summary>
        public void IncreasePlayedStage(string gameMode)
        {
            switch (gameMode)
            {
                case PVPFightToFirstDeadMode.NAME:
                    {
                        _fightToFirstDeadStage_countGame++;
                    }
                    break;

                case PVPFightWithVipMode.NAME:
                    {
                        _fightWithVipStage_countGame++;
                    }
                    break;

                case PVPDuelMode.NAME:
                    {
                        _duelStage_countGame++;
                    }
                    break;

                default:
                    {
                        LogChannel.Log("PlotPrivateModel :: IncreasePlayedStage() I can't increase count game stage, because I don't know this stage = " + gameMode);
                    }
                    break;
            }
        }

        /// <summary>
        /// Получить количество сыграных игр на указаном игровом этапе
        /// </summary>
        public int GetCountPlayedStage(string gameMode)
        {
            switch (gameMode)
            {
                case PVPFightToFirstDeadMode.NAME:
                    {
                        return _fightToFirstDeadStage_countGame;
                    }
                    break;

                case PVPFightWithVipMode.NAME:
                    {
                        return _fightWithVipStage_countGame;
                    }
                    break;

                case PVPDuelMode.NAME:
                    {
                        return _duelStage_countGame;
                    }
                    break;

                default:
                    {
                        LogChannel.Log("PlotPrivateModel :: GetCountPlayedStage() I can't give count played stage, because I don't know this stage = " + gameMode);
                    }
                    break;
            }

            return 0;
        }

    }
}

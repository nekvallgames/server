using Plugin.Interfaces;
using Plugin.Models.Private;
using Plugin.Schemes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Plugin.Runtime.Services
{
    /// <summary>
    /// Сервіс, котрий буде відслідковувати температурний слід юнітів,
    /// котрі переміщаються по карті
    /// Чим довше юніт стоїть на одному місці, тим гарячіше буде його слід
    /// </summary>
    public class CellTemperatureTraceService
    {
        private UnitsService _unitService;
        private HitAreaService _hitAreaService;

        /// <summary>
        /// При кожному оновленні температурного сліду на скільки слід буде нагріватись
        /// </summary>
        private const float INCREASE_TEMPERATURE = 1f;
        /// <summary>
        /// При кожному оновленні температурного сліду на скільки слід буде охолоджуватись
        /// </summary>
        private const float DECREASE_TEMPERATURE = 0.4f;

        private CellTemperaturePrivateModel _cellTemperaturePrivateModel;

        public CellTemperatureTraceService(UnitsService unitsService,
            HitAreaService hitAreaService,
            CellTemperaturePrivateModel cellTemperaturePrivateModel)
        {
            _unitService = unitsService;
            _hitAreaService = hitAreaService;
            _cellTemperaturePrivateModel = cellTemperaturePrivateModel;
        }

        /// <summary>
        /// Оновити температурний слід для юнітів вказаного гравця
        /// </summary>
        public void UpdateTemperatureTrace(string gameId, int actorNr)
        {
            List<CellTemperatureScheme> temperatureCells = _cellTemperaturePrivateModel.Get(gameId, actorNr).Temperatures;

            // Зменшити температурний слід всім юнітам поточного гравця
            foreach (CellTemperatureScheme t in temperatureCells)
            {
                t.Temperature -= DECREASE_TEMPERATURE;
            }

            // Видалити сели, котрі стали холодними
            List<CellTemperatureScheme> temperatures = temperatureCells.FindAll(x => x.Temperature > 0f);
            temperatureCells.Clear();
            temperatureCells.AddRange(temperatures);

            var units = new List<IUnit>();
            _unitService.GetAliveUnits(gameId, actorNr, ref units);

            foreach (IUnit unit in units)
            {
                var cellsBehind = new List<(int, int)>();
                _hitAreaService.GetBodyCellsArea(unit, ref cellsBehind);

                IncreaseTemperature(temperatureCells, cellsBehind);
            }
        }

        public void CreateTemperatureTrace(string gameId, int actorNr)
        {
            _cellTemperaturePrivateModel.Add(new CellTemperatureTracesScheme(gameId, actorNr));
        }

        /// <summary>
        /// Збільшити температурний слід
        /// </summary>
        private void IncreaseTemperature(List<CellTemperatureScheme> existTemperatures, List<(int, int)> newTemperatures)
        {
            foreach ((int, int) temperature in newTemperatures)
            {
                if (existTemperatures.Any(x => x.PositionW == temperature.Item1
                                            && x.PositionH == temperature.Item2))
                {
                    // Оновити існуючий температурний слід
                    CellTemperatureScheme tCell = existTemperatures.First(x => x.PositionW == temperature.Item1
                                                                            && x.PositionH == temperature.Item2);

                    tCell.Temperature += INCREASE_TEMPERATURE;
                }
                else
                {
                    // Створити новий температурний слід
                    existTemperatures.Add(new CellTemperatureScheme(temperature.Item1,
                                                                    temperature.Item2,
                                                                    INCREASE_TEMPERATURE));
                }
            }
        }

        /// <summary>
        /// Отримати температурний слід
        /// </summary>
        /// <param name="actorId">вказати гравця власника сліду</param>
        /// <param name="compareList">вказати список селів, в координатах котрих нас цікавить температурний слід</param>
        /// <param name="resultList">результат покласти в поточний ліст</param>
        /// <param name="minTemperature">вказати мінімальну температуру яка потрібна. Якщо температура буде меншою, то слід буде проігнорований</param>
        public void GetTrace(string gameId, int actorNr, ref List<(int, int)> compareList, ref List<(int, int)> resultList, float minTemperature = 1f)
        {
            if (!_cellTemperaturePrivateModel.Has(gameId, actorNr)){
                return;
            }

            List<CellTemperatureScheme> content = _cellTemperaturePrivateModel.Get(gameId, actorNr).Temperatures;

            foreach ((int, int) require in compareList)
            {
                if (content.Any(x => x.PositionW == require.Item1 && x.PositionH == require.Item2 && x.Temperature >= minTemperature))
                {
                    var cell = content.First(x => x.PositionW == require.Item1 && x.PositionH == require.Item2);
                    resultList.Add((cell.PositionW, cell.PositionH));
                }
            }
        }
    }
}

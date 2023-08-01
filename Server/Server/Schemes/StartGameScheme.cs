using Plugin.Interfaces;
using System;
using System.Collections.Generic;

namespace Plugin.Schemes
{
    /// <summary>
    /// Схема із даними гравця, котрі потрібні для старта гри 
    /// </summary>
    [Serializable]
    public struct StartGameScheme : IOpScheme
    {
        /// <summary>
        /// Список із обраними юнітами
        /// item1 - id
        /// item2 - level
        /// </summary>
        public List<ChoosedUnit> units;
    }

    [Serializable]
    public struct ChoosedUnit
    {
        public int unitId;
        public int level;

        public ChoosedUnit(int unitId, int level)
        {
            this.unitId = unitId;
            this.level = level;
        }
    }
}

using Plugin.Interfaces;
using System;
using System.Collections.Generic;

namespace Plugin.Schemes
{
    /// <summary>
    /// Схема c выбраными ID юнитами, которыми игрок будет играть
    /// </summary>
    [Serializable]
    public class ChoosedUnitsScheme : IOpScheme
    {
        public List<int> unitsId;
    }
}

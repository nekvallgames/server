using Plugin.Tools;

namespace Plugin.Schemes.Public
{
    /// <summary>
    /// Схема, которая реализует карту навигации перемещение юнита
    /// </summary>
    public struct NavigationMapPublicScheme
    {
        public Enums.WalkNavigation Type { get; }

        /// <summary>
        /// Ширина карты пути
        /// </summary>
        public uint MapWidth { get; private set; }

        /// <summary>
        /// Высота карты пути
        /// </summary>
        public uint MapHeight { get; private set; }

        /// <summary>
        /// Что бы определить 0 индекс на глобальной карте, нужно от текущих координат юнита отнять startIndexOffsetW
        /// </summary>
        public int StartIndexOffsetW { get; private set; }

        /// <summary>
        /// Что бы определить 0 индекс на глобальной карте, нужно от текущих координат юнита отнять startIndexOffsetH
        /// </summary>
        public int StartIndexOffsetH { get; private set; }

        /// <summary>
        /// Массив с рисунком пути
        /// </summary>
        public uint[] Map { get; private set; }

        public bool IsNull { get; set; }

        public static NavigationMapPublicScheme Null
        {
            get {
                var scheme = new NavigationMapPublicScheme();
                scheme.IsNull = true;
                return scheme;
            }            
        }

        public NavigationMapPublicScheme(Enums.WalkNavigation type, uint mapWidth, uint mapHeight, int startIndexOffsetW, int startIndexOffsetH, uint[] map)
        {
            Type = type;
            MapWidth = mapWidth;
            MapHeight = mapHeight;
            StartIndexOffsetW = startIndexOffsetW;
            StartIndexOffsetH = startIndexOffsetH;
            Map = map;

            IsNull = false;
        }
    }
}

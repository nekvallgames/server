using Plugin.Interfaces;
using Plugin.Schemes.Public;
using Plugin.Tools;
using System.Collections.Generic;
using System.Linq;

namespace Plugin.Models.Public
{
    public class NavigationMapPublicModel : IPublicModel
    {
        private List<NavigationMapPublicScheme> _mapList = new List<NavigationMapPublicScheme>();

        public void Parse()
        {
            CreateBodyWidth_1();
            CreateBodyWidth_1_for_duel();

            CreateBodyWidth_2();
            CreateBodyWidth_2_for_duel();

            CreateBodyWidth_3();
            CreateBodyWidth_3_for_duel();

            CreateStaticBodyWidth(1);
            CreateStaticBodyWidth(2);
            CreateStaticBodyWidth(3);
            CreateStaticBodyWidth(4);

            CreateBodyWidth_1_horizontalWidth_13();
        }

        /// <summary>
        /// Получить карту путей. Это путь на сетке, как может перемещатся юнит
        /// </summary>
        public NavigationMapPublicScheme GetMap(Enums.WalkNavigation walkNavigation)
        {
            if (!_mapList.Any(x => x.Type == walkNavigation)){
                return NavigationMapPublicScheme.Null;
            }

            return _mapList.Find(x => x.Type == walkNavigation);
        }

        private void CreateBodyWidth_1()
        {
            // ширина/высота карты пути
            uint mapWidth = 5;
            uint mapHeight = 23;

            // Что бы определить 0 индекс на глобальной карте, нужно от текущих координат юнита отнять startIndexOffsetH и startIndexOffsetW
            // От 2-ки стремимся к нулевому индексу в map
            int startIndexOffsetW = -2;
            int startIndexOffsetH = 11;    // потому что отчет идет от левого нижнего угла

            uint[] map = new uint[]
            {
                0, 0, 1, 0, 0,
                0, 0, 1, 0, 0,
                0, 0, 1, 0, 0,
                0, 0, 1, 0, 0,
                0, 0, 1, 0, 0,
                0, 0, 1, 0, 0,
                0, 0, 1, 0, 0,
                0, 0, 1, 0, 0,
                0, 0, 1, 0, 0,
                0, 0, 1, 0, 0,
                0, 0, 1, 0, 0,
                1, 1, 2, 1, 1,
                0, 0, 1, 0, 0,
                0, 0, 1, 0, 0,
                0, 0, 1, 0, 0,
                0, 0, 1, 0, 0,
                0, 0, 1, 0, 0,
                0, 0, 1, 0, 0,
                0, 0, 1, 0, 0,
                0, 0, 1, 0, 0,
                0, 0, 1, 0, 0,
                0, 0, 1, 0, 0,
                0, 0, 1, 0, 0
            };

            _mapList.Add(new NavigationMapPublicScheme(Enums.WalkNavigation.body_width_1, 
                                                       mapWidth, 
                                                       mapHeight, 
                                                       startIndexOffsetW, 
                                                       startIndexOffsetH, 
                                                       map));
        }

        private void CreateBodyWidth_1_horizontalWidth_13()
        {
            // ширина/высота карты пути
            uint mapWidth = 13;
            uint mapHeight = 23;

            // Что бы определить 0 индекс на глобальной карте, нужно от текущих координат юнита отнять startIndexOffsetH и startIndexOffsetW
            // От 2-ки стремимся к нулевому индексу в map
            int startIndexOffsetW = -6;
            int startIndexOffsetH = 11;    // потому что отчет идет от левого нижнего угла

            uint[] map = new uint[]
            {
                0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0,
                1, 1, 1, 1, 1, 1, 2, 1, 1, 1, 1, 1, 1,
                0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0
            };

            _mapList.Add(new NavigationMapPublicScheme(Enums.WalkNavigation.body_width_1_horizontal_width_13, 
                                                       mapWidth, 
                                                       mapHeight, 
                                                       startIndexOffsetW, 
                                                       startIndexOffsetH, 
                                                       map));
        }

        private void CreateBodyWidth_1_for_duel()
        {
            // ширина/высота карты пути
            uint mapWidth = 5;
            uint mapHeight = 1;

            // Что бы определить 0 индекс на глобальной карте, нужно от текущих координат юнита отнять startIndexOffsetH и startIndexOffsetW
            // От 2-ки стремимся к нулевому индексу в map
            int startIndexOffsetW = -2;
            int startIndexOffsetH = 0;    // потому что отчет идет от левого нижнего угла

            uint[] map = new uint[]
            {
                1, 1, 2, 1, 1
            };

            _mapList.Add(new NavigationMapPublicScheme(Enums.WalkNavigation.body_width_1_for_duel, 
                                                       mapWidth, 
                                                       mapHeight, 
                                                       startIndexOffsetW,
                                                       startIndexOffsetH,
                                                       map));
        }

        private void CreateBodyWidth_2()
        {
            // ширина/высота карты пути
            uint mapWidth = 6;
            uint mapHeight = 23;

            // Что бы определить 0 индекс на глобальной карте, нужно от текущих координат юнита отнять startIndexOffsetH и startIndexOffsetW
            // От 2-ки стремимся к нулевому индексу в map
            int startIndexOffsetW = -2;
            int startIndexOffsetH = 11;    // потому что отчет идет от левого нижнего угла

            uint[] map = new uint[]
            {
                0, 0, 1, 1, 0, 0,
                0, 0, 1, 1, 0, 0,
                0, 0, 1, 1, 0, 0,
                0, 0, 1, 1, 0, 0,
                0, 0, 1, 1, 0, 0,
                0, 0, 1, 1, 0, 0,
                0, 0, 1, 1, 0, 0,
                0, 0, 1, 1, 0, 0,
                0, 0, 1, 1, 0, 0,
                0, 0, 1, 1, 0, 0,
                1, 1, 1, 1, 1, 1,
                1, 1, 2, 1, 1, 1,
                1, 1, 1, 1, 1, 1,
                0, 0, 1, 1, 0, 0,
                0, 0, 1, 1, 0, 0,
                0, 0, 1, 1, 0, 0,
                0, 0, 1, 1, 0, 0,
                0, 0, 1, 1, 0, 0,
                0, 0, 1, 1, 0, 0,
                0, 0, 1, 1, 0, 0,
                0, 0, 1, 1, 0, 0,
                0, 0, 1, 1, 0, 0,
                0, 0, 1, 1, 0, 0
            };

            _mapList.Add(new NavigationMapPublicScheme(Enums.WalkNavigation.body_width_2, 
                                                       mapWidth, 
                                                       mapHeight, 
                                                       startIndexOffsetW, 
                                                       startIndexOffsetH, 
                                                       map));
        }

        private void CreateBodyWidth_2_for_duel()
        {
            // ширина/высота карты пути
            uint mapWidth = 6;
            uint mapHeight = 1;

            // Что бы определить 0 индекс на глобальной карте, нужно от текущих координат юнита отнять startIndexOffsetH и startIndexOffsetW
            // От 2-ки стремимся к нулевому индексу в map
            int startIndexOffsetW = -2;
            int startIndexOffsetH = 0;    // потому что отчет идет от левого нижнего угла

            uint[] map = new uint[]
            {
                1, 1, 2, 1, 1, 1,
            };

            _mapList.Add(new NavigationMapPublicScheme(Enums.WalkNavigation.body_width_2_for_duel, 
                                                       mapWidth, 
                                                       mapHeight, 
                                                       startIndexOffsetW, 
                                                       startIndexOffsetH, 
                                                       map));
        }

        private void CreateBodyWidth_3()
        {
            // ширина/высота карты пути
            uint mapWidth = 9;
            uint mapHeight = 19;

            // Что бы определить 0 индекс на глобальной карте, нужно от текущих координат юнита отнять startIndexOffsetH и startIndexOffsetW
            // От 2-ки стремимся к нулевому индексу в map
            int startIndexOffsetW = -3;
            int startIndexOffsetH = 9;    // потому что отчет идет от левого нижнего угла

            uint[] map = new uint[]
            {
                0, 0, 0, 1, 1, 1, 0, 0, 0,
                0, 0, 0, 1, 1, 1, 0, 0, 0,
                0, 0, 0, 1, 1, 1, 0, 0, 0,
                0, 0, 0, 1, 1, 1, 0, 0, 0,
                0, 0, 0, 1, 1, 1, 0, 0, 0,
                0, 0, 0, 1, 1, 1, 0, 0, 0,
                0, 0, 0, 1, 1, 1, 0, 0, 0,
                0, 0, 0, 1, 1, 1, 0, 0, 0,
                1, 1, 1, 1, 1, 1, 1, 1, 1,
                1, 1, 1, 2, 1, 1, 1, 1, 1,
                1, 1, 1, 1, 1, 1, 1, 1, 1,
                0, 0, 0, 1, 1, 1, 0, 0, 0,
                0, 0, 0, 1, 1, 1, 0, 0, 0,
                0, 0, 0, 1, 1, 1, 0, 0, 0,
                0, 0, 0, 1, 1, 1, 0, 0, 0,
                0, 0, 0, 1, 1, 1, 0, 0, 0,
                0, 0, 0, 1, 1, 1, 0, 0, 0,
                0, 0, 0, 1, 1, 1, 0, 0, 0,
                0, 0, 0, 1, 1, 1, 0, 0, 0,
            };

            _mapList.Add(new NavigationMapPublicScheme(Enums.WalkNavigation.body_width_3, 
                                                       mapWidth, 
                                                       mapHeight, 
                                                       startIndexOffsetW, 
                                                       startIndexOffsetH, 
                                                       map));
        }

        private void CreateBodyWidth_3_for_duel()
        {
            // ширина/высота карты пути
            uint mapWidth = 9;
            uint mapHeight = 1;

            // Что бы определить 0 индекс на глобальной карте, нужно от текущих координат юнита отнять startIndexOffsetH и startIndexOffsetW
            // От 2-ки стремимся к нулевому индексу в map
            int startIndexOffsetW = -3;
            int startIndexOffsetH = 0;    // потому что отчет идет от левого нижнего угла

            uint[] map = new uint[]
            {
                1, 1, 1, 2, 1, 1, 1, 1, 1
            };

            _mapList.Add(new NavigationMapPublicScheme(Enums.WalkNavigation.body_width_3_for_duel, 
                                                       mapWidth, 
                                                       mapHeight, 
                                                       startIndexOffsetW, 
                                                       startIndexOffsetH,
                                                       map));
        }


        /// <summary>
        /// Создать путь перемещения, который имеет только ширину
        /// </summary>
        private void CreateStaticBodyWidth(uint width)
        {
            // ширина/высота карты пути
            uint mapWidth = width;
            uint mapHeight = 1;

            // Что бы определить 0 индекс на глобальной карте, нужно от текущих координат юнита отнять startIndexOffsetH и startIndexOffsetW
            // От 2-ки стремимся к нулевому индексу в map
            int startIndexOffsetW = 0;
            int startIndexOffsetH = 0;    // потому что отчет идет от левого нижнего угла

            uint[] map;
            Enums.WalkNavigation walkNavigation;

            switch (width)
            {
                default:
                case 1:
                    {
                        map = new uint[] { 2 };
                        walkNavigation = Enums.WalkNavigation.static_body_width_1;
                    }
                    break;

                case 2:
                    {
                        map = new uint[] { 2, 1 };
                        walkNavigation = Enums.WalkNavigation.static_body_width_2;
                    }
                    break;

                case 3:
                    {
                        map = new uint[] { 2, 1, 1 };
                        walkNavigation = Enums.WalkNavigation.static_body_width_3;
                    }
                    break;

                case 4:
                    {
                        map = new uint[] { 2, 1, 1, 1 };
                        walkNavigation = Enums.WalkNavigation.static_body_width_4;
                    }
                    break;
            }

            _mapList.Add(new NavigationMapPublicScheme(walkNavigation, 
                                                       mapWidth, 
                                                       mapHeight, 
                                                       startIndexOffsetW,
                                                       startIndexOffsetH, 
                                                       map));
        }
    }
}

using Plugin.Interfaces;
using System;
using System.Collections.Generic;

namespace Plugin.Runtime.Services
{
    /// <summary>
    /// Сервис, который будет сортировать цели на игровой сетке 
    /// </summary>
    public class SortHitOnGridService
    {
        /// <summary>
        /// Инициализация цели, в которую будет выстрел
        ///
        /// В списке occupiedList мы имеем всех оккупантов, которые заняли текущею ячейку
        /// Нужно перебрать всех оккупантов, и высчитать, кто находится ближе к выстрелу.
        /// То есть, юнит, который имеет позицию по высоте меньше, тот стоит ближе всех к попаданию атаки
        /// 
        /// Но! Также нужно проверить, что преграда тоже может стоять в тех же координатах
        /// по высоте, что и юнит, так что в преграды приоритет выше! Это значит то, что
        /// преграда возьмет на себя урон первая.
        /// </summary>
        public List<IUnit> SortTargets(List<IUnit> occupiedList)
        {
            if (occupiedList.Count <= 0)
            {
                return new List<IUnit>();
            }

            List<SortVO> barrierList = new List<SortVO>();
            List<SortVO> otherList = new List<SortVO>();

            // 1. Сортируем список из оккупантов на преграды и остальных участников
            foreach (IUnit occupied in occupiedList)
            {
                //PositionOnGridComponent positionOnGrid = entityManager.GetComponentData<PositionOnGridComponent>(occupied.PositionOnGridH);
                var sortVo = new SortVO
                {
                    unit = occupied,
                    positionOnGridH = occupied.Position.y
                };

                if (occupied is IBarrier)
                {
                    barrierList.Add(sortVo);      // unit is barrier
                }
                else
                {
                    otherList.Add(sortVo);        // unit is other
                }
            }

            // 2. Сортируем список с юнитами по высоте H
            barrierList.Sort((x, y) => x.positionOnGridH.CompareTo(y.positionOnGridH));
            otherList.Sort((x, y) => x.positionOnGridH.CompareTo(y.positionOnGridH));

            // 3. Если есть только юниты, а список с преградами пуст,
            // значит проверку на укрытие юнитов за преградами делать не нужно
            if (otherList.Count > 0 && barrierList.Count <= 0)
            {
                return ConvertToEntityList(otherList);
            }

            // 4. Если есть только преграды, а список с юнитами пуст,
            // значит проверку на укрытие юнитов за преградами делать не нужно
            if (barrierList.Count > 0 && otherList.Count <= 0)
            {
                return ConvertToEntityList(barrierList);
            }

            // 5. В списках есть как преграды так и юниты
            // Найти минимальный/максимальный индекс по высоте
            int minH = Math.Min(barrierList[0].positionOnGridH, otherList[0].positionOnGridH);
            int maxH = Math.Min(barrierList[barrierList.Count - 1].positionOnGridH, otherList[otherList.Count - 1].positionOnGridH);

            // 3. Теперь нужно сортировать юнитов и преграды по позиции H
            // Только исключение, если юнит и преграда находятся в одних и тех же позиции по H,
            // то преграда должна находится выше юнита. Типа юнит должен стоять за преградой
            List<SortVO> list = new List<SortVO>();
            for (int i = maxH; i >= minH; i--)
            {
                // В текущею высоту ячейки положить юнита
                SortVO other = GetItemByPositionH(otherList, i);
                if (!other.IsNull)
                {
                    list.Add(other);
                }

                // В текущею высоту ячейки положить преграду
                SortVO barrier = GetItemByPositionH(barrierList, i);
                if (!barrier.IsNull)
                {
                    list.Add(barrier);
                }
            }

            list.Reverse();

            return ConvertToEntityList(list);
        }

        /// <summary>
        /// Получить итем из массива указав по позицию по высоте H
        /// </summary>
        private SortVO GetItemByPositionH(List<SortVO> list, int positionH)
        {
            foreach (SortVO sort in list)
            {
                if (sort.positionOnGridH == positionH)
                    return sort;
            }

            return SortVO.Null;
        }

        private List<IUnit> ConvertToEntityList(List<SortVO> otherList)
        {
            List<IUnit> list = new List<IUnit>();

            foreach (SortVO data in otherList)
            {
                list.Add(data.unit);
            }
            return list;
        }
    }

    /// <summary>
    /// Вспомагательный класс для сортировки сущностей
    /// </summary>
    internal struct SortVO
    {
        public IUnit unit;
        public int positionOnGridH;
        public bool IsNull;

        public static SortVO Null
        {
            get
            {
                SortVO cell = new SortVO();
                cell.IsNull = true;

                return cell;
            }
        }

        public SortVO(IUnit unit, int positionOnGridH)
        {
            IsNull = false;

            this.unit = unit;
            this.positionOnGridH = positionOnGridH;
        }
    }
}


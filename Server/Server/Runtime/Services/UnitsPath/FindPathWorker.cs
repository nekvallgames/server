using Plugin.Interfaces;
using Plugin.Models.Public;
using Plugin.Schemes;
using Plugin.Schemes.Public;
using Plugin.Tools;
using System.Collections.Generic;
using System.Numerics;

namespace Plugin.Runtime.Services.UnitsPath
{
    /// <summary>
    /// Класс помощник UnitsPathService, который будет искать путь,
    /// (зона перемещения) по которому может перемещатся юнит
    /// </summary>
    public class FindPathWorker
    {
        private NavigationMapPublicModel _navigationModel;
        private GridService _gridService;

        private int _currPathIndex;    // текущая проверяемая ячейка
        private int _nextPathIndex;    // следующая ячейка, которую будем проверять

        public FindPathWorker(NavigationMapPublicModel navigationModel, GridService gridService)
        {
            _navigationModel = navigationModel;
            _gridService = gridService;
        }

        /// <summary>
        /// Найти на игровой сетке все свободные селлы,
        /// по которым юнит может перемещатся 
        /// </summary>
        public List<(int, int)> GetAllMovementCells(IUnit unit, IGrid grid)
        {
            var result = new List<(int, int)>();

            IWalkableComponent moveTypeData = (IWalkableComponent)unit;

            foreach (Cell cell in grid.CellsList)
            {
                int cellMask = (int)cell.binaryMask;
                int IsMaskCorrect = (cellMask & moveTypeData.WayMask);

                if (IsMaskCorrect > 0      // Если на текущею ячейку юнит может стать
                    /*&& !IsCellBlockWay*/)   // Если ячейка не заблокирована
                {
                    var wayPoint = new PathCellPrivateScheme
                    {
                        positionW = (int)cell.wIndex,
                        positionH = (int)cell.hIndex
                    };
                    result.Add((wayPoint.positionW, wayPoint.positionH));
                }
            }

            return result;
        }

        /// <summary>
        /// Найти зону перемещения для текущего юнита
        /// То есть, вернуть список селлов, на которые юнит может встать
        /// unit - указать юнит, для которого нужно посчитать путь перемещения
        /// positionOnGridW - позиция на игровой сетке
        /// positionOnGridH - позиция на игровой сетке
        ///
        /// Передать проверяемую позицию, что бы можно было высчитать не только текущею его позицию,
        /// где позиционируется юнит, а и любую рандомнуюю позицию. 
        /// </summary>
        public List<(int, int)> CalculatePath(IUnit unit, int positionOnGridW, int positionOnGridH, IGrid grid)
        {
            // Получить просто список ячеек, которые свободны, и юнит может стать на них
            // Пока без каких либо проверок, может юнит дойти или не может
            // 
            // Берем матрицу перемещения юнита, получаем селлы, куда он может встать
            // Максимум выбрасываем селлы, в которые игрок выстрелил
            // unitPath = GetStepCells(unit, positionOnGridW, positionOnGridH, grid);

            // Теперь нужно от стартовой ячейки позиции юнита, перебрать все ячейки path, и 
            // найти путь, куда может дойти юнит
            //
            // Существующие ячейки, от стартовой позиции юнита, прогоняем алгоритмом поиска пути
            // Что бы в итоге получить только те селлы, к которым юнит может дойти
            return FindWay(unit, 
                           positionOnGridW, 
                           positionOnGridH, 
                           GetStepCells(unit, positionOnGridW, positionOnGridH, grid));
        }

        /// <summary>
        /// Получить селлы на гриде, куда может встать юнит
        /// Это без проверки, обрезаный путь или нет. Пока просто,
        /// если ячейка свододна, значит она будет добавлена в список
        /// positionOnGridW - позиция юнита на игровой сетке по ширине
        /// positionOnGridH - позиция юнита на игровой сетке по высоте
        /// </summary>
        private List<PathCellPrivateScheme> GetStepCells(IUnit unit, int positionOnGridW, int positionOnGridH, IGrid grid)
        {
            var path = new List<PathCellPrivateScheme>();

            IWalkableComponent moveTypeData = (IWalkableComponent)unit;

            // Размер игровой сетки
            Vector2 gridSize = new Vector2(grid.SizeGridW, grid.SizeGridH);

            // 1. Получить карту (матрицу) путей. Это путь на сетке, как может перемещатся юнит, если ему ничего не мешает
            // То есть, это матрица, как может перемещатся юнит. И эту матрицу мы накладываем на игровую сетку, и 
            // после будем откидывать все лишнее. Лишнее подразумывается, куда не может пойти юнит. 
            NavigationMapPublicScheme navigationMapScheme = _navigationModel.GetMap(moveTypeData.NavigationWay);
            if (navigationMapScheme.IsNull)
            {
                LogChannel.Log($"FindPathWorker :: GetStepCells() navigationController don't has this {moveTypeData.NavigationWay}. Fix me!", LogChannel.Type.Plot);
                return path;
            }

            // 2. Отталкиваясь от позиции юнита, найти левый верхний угол (тоесть нулевой индекс) карты путей юнита
            // То есть, на игровую сетку мы накладываем матрицу перемещения юнита. И начиная с 
            // левого верхнего угла перебираем всю матрицу, и находим под ней селлы на игровой сетке
            // Если селл существует и подходит для перемещения, то добавляем его в массив path
            int startIndexW = (int)(positionOnGridW + navigationMapScheme.StartIndexOffsetW);
            int startIndexH = (int)(positionOnGridH + navigationMapScheme.StartIndexOffsetH);

            // 3. Перебираем ввесь массив карты путей, с 0 индекса. 
            // Нулевой индекс находится на глобальной карте в startIndexW и startIndexH
            int index = 0;                    // индекс по ширине
            int localW = startIndexW;
            int localH = startIndexH;

            for (int i = 0; i < navigationMapScheme.Map.Length; i++)
            {
                // Проверка, если индекс по высоте вышел за верхний край игровой сетки
                // Нужно сместить каретку на начало массива, но опустится на 1 ряд вниз
                if (localH > gridSize.Y)
                {
                    localW = startIndexW;
                    localH--;
                    index = 0;
                    i += (int)(navigationMapScheme.MapWidth - 1);
                    continue;
                }

                // Проверка, если индекс по ширине вышел за левый край игровой сетки
                if (localW < 0)
                {
                    localW++;
                    index++;
                    continue;
                }

                // Проверка, если индекс по ширине вышел за правый край игровой сетки
                // Нужно сместить каретку на начало массива, но опустится на 1 ряд вниз
                if (localW >= gridSize.X)
                {
                    i += (int)(navigationMapScheme.MapWidth - 2) - (index - 1);
                    localW = startIndexW;
                    localH--;
                    index = 0;
                    continue;
                }



                // проверяем маску в карте путей. Если номер 1 или 2, значит согласно маске путей
                // юнит может стать в текущею ячейку
                bool canStep = navigationMapScheme.Map[i] == 1 || navigationMapScheme.Map[i] == 2 ? true : false;
                if (canStep)
                {
                    Cell cell = _gridService.GetCell(grid, localW, localH);

                    if (cell != null)
                    {
                        // CellComponent cellComponent = _entityManager.GetComponentData<CellComponent>(cell);

                        int cellMask = (int)cell.binaryMask;
                        int IsMaskCorrect = (cellMask & moveTypeData.WayMask);
                        //bool IsCellBlockWay = cell.IsAlive;
                        // IsCellBlockWay = false;

                        if (IsMaskCorrect > 0      // Если на текущею ячейку юнит может стать
                            /*&& !IsCellBlockWay*/ )   // Если ячейка не заблокирована
                        {
                            var wayPoint = new PathCellPrivateScheme
                            {
                                positionW = localW,
                                positionH = localH
                            };
                            path.Add(wayPoint);
                        }
                    }
                }

                // Мы перебрали все индексы по ширине
                // Возвращаемся по ширине в 0 индекс, а по высоте вниз
                if (index >= navigationMapScheme.MapWidth - 1)
                {
                    localW = startIndexW;
                    localH--;
                    index = 0;
                    continue;
                }

                // Продолжаем двигатся вправо по ширине
                localW++;
                index++;

                if (localH < 0)
                    break;        // проверяемый ряд ячеек уже находится ниже нижнего края игровой сетки
            }

            return path;
        }

        /// <summary>
        /// Агоритм поиска пути
        /// От стартовой ячейки позиции юнита, перебрать все ячейки path, и 
        /// найти путь, куда может дойти юнит
        /// positionOnGridW - позиция юнита на игровой сетке по ширине
        /// positionOnGridH - позиция юнита на игровой сетке по высоте
        /// </summary>
        private List<(int, int)> FindWay(IUnit unit, int positionOnGridW, int positionOnGridH, List<PathCellPrivateScheme> pathSteps)
        {
            var result = new List<(int, int)>();

            _currPathIndex = 0; // текущая проверяемая ячейка
            _nextPathIndex = 0; // следующая ячейка, которую будем проверять

            // 1. В списке точек, куда может стать юнит, найти селл, в котором сейчас находится юнит
            // С этого селла и запустим алгоритм поиска пути
            PathCellPrivateScheme startCell = GetCellByPosition(positionOnGridW,
                                                                positionOnGridH,
                                                                pathSteps);
            startCell.pathIndex = _currPathIndex; // указать индекс пути для стартового селла. С которого начнем поиск пути
            _nextPathIndex = 1; // указать индекс, который пулучит следующий селл в прокладываемом пути

            // 2. Перебираем все селлы, и прокладываем путь от позиции, где стоит юнит,
            // до всего, до чего может дойти юнит
            do
            {
                // 2.1 Найти всех соседей текущего проверяемого селла.
                // Каждому найденному соседу, мы даем индекс пути и делаем его IsDirty, 
                // что бы больше не проверять этого соседа
                FindNeighbours(pathSteps);
                // 2.2 Увеличиваем индекс текущего проверяемого селла
                _currPathIndex++;
            } while (_currPathIndex < _nextPathIndex);


            // 3. Ложим в список все селлы, которые IsDirty
            // Все селлы, которые IsDirty == true, это селлы, куда
            // может дойти юнит
            foreach (PathCellPrivateScheme pathCell in pathSteps)
            {
                if (pathCell.IsDirty)
                {
                    result.Add((pathCell.positionW, pathCell.positionH));
                }
            }

            // 4. Если wayPath пустой, значит путь не был найден.
            // Вернуть ходя бы ячейки, которые юнит занимает по ширине
            if (result.Count <= 0)
            {
                foreach (PathCellPrivateScheme pathCell in pathSteps)
                {
                    if (pathCell.positionW == positionOnGridW && pathCell.positionH == positionOnGridH)
                    {
                        result.Add((pathCell.positionW, pathCell.positionH));
                        break;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Получить селл по его координатам на игровой сетке
        /// </summary>
        private PathCellPrivateScheme GetCellByPosition(int posW, int posH, List<PathCellPrivateScheme> pathSteps)
        {
            foreach (PathCellPrivateScheme pathCell in pathSteps)
            {
                if (pathCell.positionW == posW && pathCell.positionH == posH)
                {
                    return pathCell;
                }
            }

            return PathCellPrivateScheme.Null;
        }

        /// <summary>
        /// Получить селл по его индексу пути
        /// </summary>
        private PathCellPrivateScheme GetCellByPathIndex(int pathIndex, List<PathCellPrivateScheme> pathSteps)
        {
            foreach (PathCellPrivateScheme pathCell in pathSteps)
            {
                if (pathCell.pathIndex == pathIndex)
                {
                    return pathCell;
                }
            }

            return PathCellPrivateScheme.Null;
        }

        /// <summary>
        /// Найти все рядом стоящие ячейки вокруг указаной ячейки cell
        /// </summary>
        private void FindNeighbours(List<PathCellPrivateScheme> pathSteps)
        {
            // 1. В общем списке точек, куда может стать юнит, найти селл по _currPathIndex
            // Это и будет селл, вокруг которого будем искать соседей
            PathCellPrivateScheme cell = GetCellByPathIndex(_currPathIndex, pathSteps);

            // 2. Ищем соседей крестиком, против часовой стрели, то есть: up, left, down, right
            FindNeighbour(cell.positionW, (int)(cell.positionH + 1));  // Ищем UP
            FindNeighbour((int)(cell.positionW - 1), cell.positionH);    // Ищем LEFT
            FindNeighbour(cell.positionW, (int)(cell.positionH - 1));  // Ищем DOWN
            FindNeighbour((int)(cell.positionW + 1), cell.positionH);    // Ищем RIGHT


            void FindNeighbour(int positionOnGridW, int positionOnGridH)
            {
                PathCellPrivateScheme neighbor = GetCellByPosition(positionOnGridW, positionOnGridH, pathSteps);

                // Проверяем, соседний селл существует и он должен быть чистым
                // То есть, IsDirty == false, значит алгоритм поиска пути еще не дошел до этого селла
                // и алгоритм поиска пути ничего не знает о текущем селле
                if (!neighbor.IsNull && !neighbor.IsDirty)
                {
                    neighbor.pathIndex = _nextPathIndex;
                    neighbor.IsDirty = true;
                    _nextPathIndex++;
                }
            }
        }
    }
}

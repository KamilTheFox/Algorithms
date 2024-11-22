using MathNet.Numerics.LinearAlgebra;

public class HungarianAlgorithm
{
    // Матрица стоимости
    private Matrix<double> costMatrix;
    // Размеры матрицы
    private int rows;
    private int column;

    // для отмеченных нулей
    private bool[,] starMatrix;
    // для временных отметок
    private bool[,] primeMatrix;
    // Массив для отметки покрытых строк
    private bool[] rowsCovered;
    // Массив для отметки покрытых столбцов
    private bool[] colsCovered;

    public HungarianAlgorithm(double[,] costs)
    {
        // Создаем матрицу из входных данных
        costMatrix = Matrix<double>.Build.DenseOfArray(costs);
        rows = costMatrix.RowCount;
        column = costMatrix.ColumnCount;

        // Приводим матрицу к квадратному виду, если она не квадратная
        // Дополняем нулями при необходимости
        int maxDim = Math.Max(rows, column);
        if (rows != column)
        {
            var newMatrix = Matrix<double>.Build.Dense(maxDim, maxDim);
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < column; j++)
                    newMatrix[i, j] = costMatrix[i, j];

            costMatrix = newMatrix;
            rows = column = maxDim;
        }

        // Инициализируем вспомогательные массивы
        starMatrix = new bool[rows, column];
        primeMatrix = new bool[rows, column];
        rowsCovered = new bool[rows];
        colsCovered = new bool[column];
    }

    public int[] FindAssignments()
    {
        // Шаг 1: Редукция по строкам
        ReduceRows();

        // Шаг 2: Поиск начальных независимых нулей
        InitialStarZeros();

        // Основной цикл алгоритма
        int step = 3;
        while (step != -1)
        {
            switch (step)
            {
                case 3:
                    step = CoverColumns();
                    break;
                case 4:
                    step = PrimeUncoveredZero();
                    break;
                case 5:
                    step = MakeNewStarZeros();
                    break;
            }
        }

        // Получаем результат
        return GetResult();
    }
    // Уменьшаем значения в строках
    // Находим минимум в каждой строке и вычитаем его из всех элементов строки
    private void ReduceRows()
    {
        // Находим минимальный элемент в каждой строке и вычитаем его
        for (int i = 0; i < rows; i++)
        {
            double minInRow = double.MaxValue;
            for (int j = 0; j < column; j++)
                minInRow = Math.Min(minInRow, costMatrix[i, j]);

            for (int j = 0; j < column; j++)
                costMatrix[i, j] -= minInRow;
        }
    }

    // Находим начальное множество независимых нулей
    // Нули независимы, если в их строке и столбце нет других отмеченных нулей
    private void InitialStarZeros()
    {
        // Находим независимые нули
        bool[] rowUsed = new bool[rows];
        bool[] colUsed = new bool[column];

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < column; j++)
            {
                if (costMatrix[i, j] == 0 && !rowUsed[i] && !colUsed[j])
                {
                    starMatrix[i, j] = true;
                    rowUsed[i] = true;
                    colUsed[j] = true;
                }
            }
        }
    }
    // Покрываем столбцы, содержащие отмеченные нули
    // Если все столбцы покрыты - решение найдено
    private int CoverColumns()
    {
        int coveredCount = 0;
        Array.Clear(rowsCovered, 0, rows);
        Array.Clear(colsCovered, 0, column);

        for (int j = 0; j < column; j++)
        {
            for (int i = 0; i < rows; i++)
            {
                if (starMatrix[i, j])
                {
                    colsCovered[j] = true;
                    coveredCount++;
                    break;
                }
            }
        }

        return (coveredCount >= column) ? -1 : 4;
    }


    // Ищем непокрытые нули и помечаем их штрихами
    // Если находим ноль без звездочки в строке - переходим к шагу 5
    // Если нет непокрытых нулей - модифицируем матрицу

    // Модификация матрицы:
    // - Находим минимальный непокрытый элемент
    // - Вычитаем его из непокрытых элементов
    // - Прибавляем к элементам на пересечении покрытых строк и столбцов
    private int PrimeUncoveredZero()
    {
        // Ищем непокрытый ноль
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < column; j++)
            {
                if (costMatrix[i, j] == 0 && !rowsCovered[i] && !colsCovered[j])
                {
                    primeMatrix[i, j] = true;

                    // Ищем звёздочку в строке
                    int starCol = -1;
                    for (int j2 = 0; j2 < column; j2++)
                    {
                        if (starMatrix[i, j2])
                        {
                            starCol = j2;
                            break;
                        }
                    }

                    if (starCol == -1) // Нет звёздочки в строке
                    {
                        return 5; // Переходим к шагу 5
                    }
                    else
                    {
                        rowsCovered[i] = true;
                        colsCovered[starCol] = false;
                    }
                }
            }
        }

        // Если непокрытый ноль не найден, модифицируем матрицу
        double minUncovered = FindMinUncovered();
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < column; j++)
            {
                if (rowsCovered[i])
                {
                    if (colsCovered[j])
                        costMatrix[i, j] += minUncovered;
                }
                else if (!colsCovered[j])
                {
                    costMatrix[i, j] -= minUncovered;
                }
            }
        }

        return 4;
    }
    // Находим минимальный элемент среди непокрытых элементов матрицы
    private double FindMinUncovered()
    {
        double min = double.MaxValue;
        for (int i = 0; i < rows; i++)
        {
            if (!rowsCovered[i])
            {
                for (int j = 0; j < column; j++)
                {
                    if (!colsCovered[j])
                        min = Math.Min(min, costMatrix[i, j]);
                }
            }
        }
        return min;
    }
    // Строим чередующийся путь, начиная с помеченного штрихом нуля
    // Путь чередует звездочки и штрихи
    // Инвертируем звездочки вдоль пути
    // Очищаем все штрихи и покрытия
    private int MakeNewStarZeros()
    {
        List<(int, int)> path = new List<(int, int)>();

        // Находим начальную точку
        int row = -1, col = -1;
        for (int i = 0; i < rows && row == -1; i++)
        {
            for (int j = 0; j < column; j++)
            {
                if (primeMatrix[i, j])
                {
                    row = i;
                    col = j;
                    break;
                }
            }
        }

        path.Add((row, col));
        bool done = false;

        while (!done)
        {
            // Ищем звёздочку в столбце
            int starRow = -1;
            for (int i = 0; i < rows; i++)
            {
                if (starMatrix[i, path[path.Count - 1].Item2])
                {
                    starRow = i;
                    break;
                }
            }

            if (starRow == -1)
                done = true;
            else
            {
                path.Add((starRow, path[path.Count - 1].Item2));

                // Ищем штрих в строке
                int primeCol = -1;
                for (int j = 0; j < column; j++)
                {
                    if (primeMatrix[starRow, j])
                    {
                        primeCol = j;
                        break;
                    }
                }
                path.Add((starRow, primeCol));
            }
        }

        // Обновляем матрицы
        foreach (var point in path)
        {
            int i = point.Item1;
            int j = point.Item2;
            starMatrix[i, j] = !starMatrix[i, j];
        }

        // Очищаем временные данные
        Array.Clear(primeMatrix, 0, primeMatrix.Length);
        Array.Clear(rowsCovered, 0, rowsCovered.Length);
        Array.Clear(colsCovered, 0, colsCovered.Length);

        return 3;
    }

    private int[] GetResult()
    {
        int[] result = new int[rows];
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < column; j++)
            {
                if (starMatrix[i, j])
                {
                    result[i] = j;
                    break;
                }
            }
        }
        return result;
    }
}

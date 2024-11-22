namespace DataStruct
{
    public static class Relevant
    {
        private class RelevantObject
        {
            public int Index { get; set; }
            public long Relevance { get; set; }
            public int[] Features { get; set; }
        }

        static List<RelevantObject> sortedObjects = new List<RelevantObject>();

        static RelevantObject[] objects;

        static int[] parameters;

        static int n;

        public static void ReadInput()
        {
            Console.WriteLine("Введите количество параметров");
            n = int.Parse(Console.ReadLine());
            Console.WriteLine($"введите {n} параметров");
            parameters = Array.ConvertAll(Console.ReadLine().Split(), int.Parse);

            Console.WriteLine("Введите количество параметров");
            int d = int.Parse(Console.ReadLine());
            Console.WriteLine($"Введите для {d} объектов {n} параметров");

            objects = new RelevantObject[d];
            for (int i = 0; i < d; i++)
            {
                var features = Array.ConvertAll(Console.ReadLine().Split(), int.Parse);
                objects[i] = new RelevantObject
                {
                    Index = i + 1,
                    Features = features,
                    Relevance = CalculateRelevance(features)
                };
            }

            sortedObjects = objects.OrderByDescending(x => x.Relevance).ToList();
        }

        public static void ProcessQueries()
        {
            Console.WriteLine("Введите номер запроса");
            int q = int.Parse(Console.ReadLine());
            Console.WriteLine("Введите запрос");
            for (int i = 0; i < q; i++)
            {
                string[] query = Console.ReadLine().Split();
                if (query[0] == "1")//Запрос на top-К объектов
                {
                    int k = int.Parse(query[1]);
                    PrintTopK(k);
                }
                else if (query[0] == "2")//Запрос на изменение
                {
                    int objIndex = int.Parse(query[1]) - 1;
                    int featureIndex = int.Parse(query[2]) - 1;
                    int newValue = int.Parse(query[3]);
                    UpdateObject(objIndex, featureIndex, newValue);
                }
            }
        }

        static void UpdateObject(int objIndex, int featureIndex, int newValue)
        {
            // Обновляем данные
            objects[objIndex].Features[featureIndex] = newValue;
            // Считаем новую релевантность
            objects[objIndex].Relevance = CalculateRelevance(objects[objIndex].Features);

            // Ищем позицию для вставки по бинарному поиску
            int left = 0;
            int right = sortedObjects.Count - 1;
            int insertPosition = 0;

            while (left <= right)
            {
                int mid = left + (right - left) / 2;
                var newRelevance = objects[objIndex].Relevance;
                if (sortedObjects[mid].Relevance == newRelevance)
                {
                    insertPosition = mid;
                    break;
                }

                if (sortedObjects[mid].Relevance < newRelevance)
                {
                    right = mid - 1;
                    insertPosition = mid;
                }
                else
                {
                    left = mid + 1;
                    insertPosition = left;
                }
            }

            // Удаляем старую позицию
            int oldPosition = sortedObjects.FindIndex(x => x.Index == objIndex + 1);
            if (oldPosition != -1)
                sortedObjects.RemoveAt(oldPosition);

            // Вставляем в новую позицию сам объект из objectsById
            sortedObjects.Insert(insertPosition, objects[objIndex]);
        }

        static void PrintTopK(int k)
        {
            Console.WriteLine(string.Join(" ", sortedObjects.Take(k).Select(x => x.Index)));
        }

        static long CalculateRelevance(int[] obj)
        {
            long relevance = 0;
            for (int i = 0; i < n; i++)
            {
                relevance += (long)parameters[i] * obj[i];
            }
            return relevance;
        }
    }
}

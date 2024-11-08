using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStruct
{
    public static class Relevant
    {
        static List<int[]> objects = new List<int[]>();
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
            for (int i = 0; i < d; i++)
            {
                objects.Add(Array.ConvertAll(Console.ReadLine().Split(), int.Parse));
            }
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
                    objects[objIndex][featureIndex] = newValue;
                }
            }
        }

        static void PrintTopK(int k)
        {
            var relevances = objects.Select((obj, index) => new { Index = index + 1, Relevance = CalculateRelevance(obj) }) //преобразоание объектов в релевантрость
                                    .OrderByDescending(x => x.Relevance)// сортировка по убыванию
                                    .Take(k);//берем первые 3 элемента

            Console.WriteLine(string.Join(" ", relevances.Select(x => x.Index)));
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

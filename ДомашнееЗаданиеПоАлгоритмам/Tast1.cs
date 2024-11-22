using DataStruct;
using Edge = BellmanFord.Edge;

public class UnitTests
{
    public static void StartTask1()
    {

        Console.WriteLine("Создаю связный список");
        IMyData<int> singleLinked = new SingleLinkedList<int>();


        //Инициализирую

        int[] vs = new int[14].Select(x => new Random().Next(0, 16)).ToArray();
        vs[4] = 4;
        singleLinked.AddRange(vs);
        //Проверяю вывод
        Console.WriteLine(singleLinked.ToString());

        Console.WriteLine("Удаляю элемент 4");
        //Удаляю элемент содержащий 4
        singleLinked.Remove(4);
        Console.WriteLine("Проверяю на удаление");
        Console.WriteLine(singleLinked.ToString());

        //Делаю Копию для сравнения
        IMyData<int> singleLinkedClone = (IMyData<int>)singleLinked.Clone();

        Console.WriteLine("Делаю инверсию");
        singleLinked.Inverse();

        Console.WriteLine("Сравнение инверсии с LINQ");
        Console.WriteLine("My Reverse: " +  singleLinked.ToString());
        Console.WriteLine("LINQ Reverse: " + String.Join(", ", singleLinkedClone.ToArray().Reverse()));

        Console.WriteLine("");
    }

    public static void StartTask2()
    {
        //Ввод
        Relevant.ReadInput();
        //Запросы
        Relevant.ProcessQueries();
    }

    public static void StartTask3_AVL_TREE()
    {
        Console.WriteLine("Создаю АВЛ дерево");
        IMyData<int> test = new BinaryAVLTree<int>();

        test.AddRange(new int[14].Select(x => new Random().Next(0, 20)).Append(15).ToArray());

        Console.WriteLine("Вывожу результат с учетом балансировки");
        Console.WriteLine(test.ToString());

        Console.WriteLine();

        Console.WriteLine("Инвертирую дерево");
        test.Inverse();

        Console.WriteLine(test.ToString());

        Console.WriteLine("Удаляю элемент 15");
        test.Remove(15);

        Console.WriteLine("Вывод с учетом удаления");
        Console.WriteLine(test.ToString());

        Console.WriteLine();

        Console.WriteLine("Вывожу инвертированный отсортированный список список");

        Console.WriteLine(String.Join(", " , test.ToList()));

        Console.WriteLine();
    }
    //Красночерное дерево
    public static void StartTask4_RBTree()
    {
        Console.WriteLine("Создаю рандомное красно-черное дерево");

        IMyData<int> test = new RedBlackTree<int>();

        test.AddRange(new int[10].Select(x => new Random().Next(0,20)).ToArray());

        Console.WriteLine(test.ToString());
    }

    //Словарик на основе красно-черного дерева
    public static void StartTask5()
    {

        Console.WriteLine("Создаю словарик");

        IDictionary<string, string> dictionaryTree = new DictionaryTree<string, string>(1);

        dictionaryTree.Add("0", "Ноль");
        dictionaryTree.Add("1", "Один");
        dictionaryTree.Add("2", "Два");
        dictionaryTree.Add("3", "Три");
        dictionaryTree.Add("4", "Четыре");
        dictionaryTree.Add("5", "Пять");
        dictionaryTree.Add("6", "Шесть");
        dictionaryTree.Add("7", "Семь");
        dictionaryTree.Add("8", "Восемь");

        Console.WriteLine("Вывожу рандомно элементы");

        Random random = new Random();

        for(int i = 0; i < 4; i++)
        {
            int r = random.Next(0,9);
            Console.WriteLine($"{r}: {dictionaryTree[r.ToString()]}");
        }

        Console.WriteLine("Вывожу содержимое до удаения");

        Console.WriteLine(String.Join("\t,", dictionaryTree.Keys));
        Console.WriteLine(String.Join("\t,", dictionaryTree.Values));

        Console.WriteLine("Удаляю элемент 3. Где true - успешно, иначе удаления небыло");

        Console.WriteLine($"Delete 3 {dictionaryTree.Remove("3")}");

        Console.WriteLine($"Delete 3 {dictionaryTree.Remove("3")}");

        Console.WriteLine("Вывожу содержимое после удаления");

        Console.WriteLine(String.Join("\t,", dictionaryTree.Keys));
        Console.WriteLine(String.Join("\t,", dictionaryTree.Values));
    }

    public static void StartTask_6()
    {

        Console.WriteLine($"Создаю отель");
        Hotel hotel = new Hotel();

        Console.WriteLine($"Добавляю даты: \"2024 - 09 - 15\", \"2024 - 09 - 15\"");
        Console.WriteLine($"Добавляю даты: \"2024 - 09 - 14\", \"2024 - 09 - 21\"");
        hotel.Add(TicketHotel.Create("2024-09-15", "2024-09-15"));
        hotel.Add(TicketHotel.Create("2024-09-14", "2024-09-21"));

        Console.WriteLine($"\nДень с максимальным количеством посетителей");
        Console.WriteLine(hotel.FindMostCrowdedDay().ToString());
    }

    public static void StartTask_7()
    {
        var graph = new BellmanFord();

        // Добавляем рёбра

        graph.Add(new Edge { Source = 0, Destination = 1, Weight = 4 });
        graph.Add(new Edge { Source = 0, Destination = 2, Weight = 2 });
        graph.Add(new Edge { Source = 1, Destination = 2, Weight = -3 });
        graph.Add(new Edge { Source = 2, Destination = 3, Weight = 2 });
        graph.Add(new Edge { Source = 3, Destination = 1, Weight = 1 });

        // Выводим структуру графа
        graph.PrintGraph();

        // Ищем и выводим кратчайшие пути
        graph.PrintShortestPaths(0);
    }

    public static void StartTask_8()
    {
        double[,] costMatrix = new double[,]
        {
            { 5, 7, 1 },
            { 2, 3, 3 },
            { 3, 1, 2 }
        };
        Console.WriteLine("Исходная матрица");
        int rows = costMatrix.GetLength(0);
        int cols = costMatrix.GetLength(1);

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                Console.Write(costMatrix[i, j] + "\t");
            }
            Console.WriteLine();
        }

        Console.WriteLine("\nвывод\n");

        var hungarian = new HungarianAlgorithm(costMatrix);
        int[] assignments = hungarian.FindAssignments();

        Console.WriteLine(String.Join(", ",assignments));
    }

}

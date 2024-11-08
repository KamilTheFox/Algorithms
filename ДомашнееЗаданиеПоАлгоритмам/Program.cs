
Console.WriteLine("\n-------------ДОМАШНЕЕ ЗАДАНИЕ -------------\n");

Console.WriteLine("\n-------------     Задание 1   -------------\n");

Console.WriteLine("\nВремя поиска О(n)");
Console.WriteLine("Использование памяти О(n)");

DZ.StartTask1(); // Это обработка реверса односвязного списка

Console.WriteLine("\n-------------     Задание 2   -------------\n");

Console.WriteLine("\n----Что бы его активировать нужно ввести \"2\" в конце-----\n");

Console.WriteLine("\n-------------     Задание 3   -------------\n");

Console.WriteLine("\nВремя поиска О(Log n)");
Console.WriteLine("Использование памяти О(n)");

DZ.StartTask3_AVL_TREE(); // обработка AVL и inverse

Console.WriteLine("\n-------------     Задание 4   -------------\n");

Console.WriteLine("\nВремя поиска О(Log n)");
Console.WriteLine("Использование памяти О(n)");

DZ.StartTask4_RBTree(); // Балансировка Красно-Черного дерева

Console.WriteLine("\n-------------     Задание 5   -------------\n");


Console.WriteLine("\nВремя поиска О(Log n)");
Console.WriteLine("Использование памяти О(n)");

DZ.StartTask5(); //Тест словарика на основе красночерного дерева

Console.WriteLine("\n-------------     Задание 6   -------------\n");


Console.WriteLine("\nВремя поиска О(n + k) где k - пересечения и n посетители");
Console.WriteLine("Использование памяти О(n)");

DZ.StartTask_6(); //Вывод дня с максимальным количеством посетителей

Console.WriteLine("\n-------------     Задание 7   -------------\n");

Console.WriteLine("\nВремя поиска О(V * E) где E - Ребра и V вершины");
Console.WriteLine("Использование памяти О(V)");

DZ.StartTask_7(); //Вывод дня с максимальным количеством посетителей

Console.WriteLine("\n-------------                 -------------\n");

string text = Console.ReadLine();

if(text == "2")
{
    DZ.StartTask2();
}















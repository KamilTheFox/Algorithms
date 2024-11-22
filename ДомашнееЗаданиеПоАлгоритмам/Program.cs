
Console.WriteLine("\n-------------ДОМАШНЕЕ ЗАДАНИЕ -------------\n");

List<Action> works = new List<Action>();

works.AddRange(new Action[]
{
    UnitTests.StartTask1,
    UnitTests.StartTask2,
    UnitTests.StartTask3_AVL_TREE,
    UnitTests.StartTask4_RBTree,
    UnitTests.StartTask5,
    UnitTests.StartTask_6,
    UnitTests.StartTask_7,
    UnitTests.StartTask_8,
});

for (int i = 0; i < works.Count; i++)
{
    Console.WriteLine($"\n-------------     Задание {i + 1}   -------------\n");
    if (i == 1)
        Console.WriteLine("\n----Что бы его активировать нужно ввести \"2\" в конце-----\n");
    else
        works[i].Invoke();

}
Console.WriteLine("\n-------------                 -------------\n");

string text = Console.ReadLine();

if (text == "2")
{
    works[1].Invoke();
}

















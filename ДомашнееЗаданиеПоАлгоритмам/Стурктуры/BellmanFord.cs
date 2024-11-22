public class BellmanFord
{
    public struct Edge
    {
        public int Source { get; set; }
        public int Destination { get; set; }
        public int Weight { get; set; }
    }

    private List<Edge> edges = new List<Edge>();

    private List<int> vertexes = new List<int>();

    public void Add(Edge edge)
    {
        AddVertex(edge.Source);
        AddVertex(edge.Destination);
        edges.Add(edge);
    }
    private void AddVertex(int id)
    {
        if (!vertexes.Contains(id))
        {
            vertexes.Add(id);
        }
    }
    public int[] FindShortestPaths(int source)
    {
        if (!vertexes.Contains(source))
        {
            Console.WriteLine("\u001b[31m Нет такой вершины в графе\u001b[0m");
            return new int[0];
        }
        int verticesCount = vertexes.Max() + 1;
        // Инициализация расстояний
        int[] distances = new int[verticesCount];
        for (int i = 0; i < verticesCount; i++)
            distances[i] = int.MaxValue;
        distances[source] = 0;

        // Основной цикл алгоритма
        for (int i = 1; i < verticesCount; i++)
        {
            foreach (var edge in edges)
            {
                int u = edge.Source;
                int v = edge.Destination;
                int weight = edge.Weight;

                if (distances[u] != int.MaxValue &&
                    distances[u] + weight < distances[v])
                {
                    distances[v] = distances[u] + weight;
                }
            }
        }

        // Проверка на отрицательные циклы
        foreach (var edge in edges)
        {
            int u = edge.Source;
            int v = edge.Destination;
            int weight = edge.Weight;

            if (distances[u] != int.MaxValue &&
                distances[u] + weight < distances[v])
            {
                Console.WriteLine("\u001b[31m Граф содержит отрицательный цикл \u001b[0m");
            }
        }

        return distances;
    }
    public void PrintGraph()
    {
        Console.WriteLine("=== Структура графа ===");

        // Сортируем вершины для красивого вывода
        var sortedVertices = vertexes.OrderBy(v => v).ToList();

        foreach (var vertex in sortedVertices)
        {
            Console.WriteLine($"\u001b[32mВершина {vertex}:\u001b[0m");
            var outgoingEdges = edges.Where(e => e.Source == vertex).ToList();

            if (outgoingEdges.Any())
            {
                foreach (var edge in outgoingEdges)
                {
                    string weightColor = edge.Weight < 0 ? "\u001b[31m" : "\u001b[36m";
                    Console.WriteLine($"  ╰─→ {edge.Destination} " +
                        $"(вес: {weightColor}{edge.Weight}\u001b[0m)");
                }
            }
            else
            {
                Console.WriteLine("  ╰─→ Нет исходящих рёбер");
            }
        }
    }

    public void PrintShortestPaths(int source)
    {
        var distances = FindShortestPaths(source);
        if (distances.Length == 0) return;

        Console.WriteLine($"\n== Кратчайшие пути от вершины {source} ===");

        for (int i = 0; i < distances.Length; i++)
        {
            string distance = distances[i] == int.MaxValue
                ? "недостижима"
                : distances[i].ToString();

            Console.WriteLine($"До вершины {vertexes[i]}: " +
            $"\u001b[33m{distance}\u001b[0m");
        }
    }
}

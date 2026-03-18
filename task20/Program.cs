namespace GraphTasks
{
    using System;
    using System.Collections.Generic;

    // ==========================================================================
    // Задача 1: Компоненты сильной связности — алгоритм Мальгранжа
    // ==========================================================================

    class StrongComponents
    {
        private int n;
        private List<int>[] graph;
        private List<int>[] reverseGraph;

        public StrongComponents(int n)
        {
            this.n = n;
            graph = new List<int>[n];
            reverseGraph = new List<int>[n];
            for (int i = 0; i < n; i++)
            {
                graph[i] = new List<int>();
                reverseGraph[i] = new List<int>();
            }
        }

        public void AddEdge(int from, int to)
        {
            graph[from].Add(to);
            reverseGraph[to].Add(from);
        }
        
        private void DFS(int v, List<int>[] g, bool[] visited, bool[] removed)
        {
            visited[v] = true;
            for (int i = 0; i < g[v].Count; i++)
            {
                int u = g[v][i];
                if (!visited[u] && !removed[u])
                    DFS(u, g, visited, removed);
            }
        }

        public List<List<int>> FindSCC()
        {
            bool[] removed = new bool[n];
            List<List<int>> result = new List<List<int>>();

            for (int start = 0; start < n; start++)
            {
                if (removed[start])
                    continue;
                
                bool[] reachForward = new bool[n];
                DFS(start, graph, reachForward, removed);
                
                bool[] reachBack = new bool[n];
                DFS(start, reverseGraph, reachBack, removed);
                
                List<int> component = new List<int>();
                for (int v = 0; v < n; v++)
                {
                    if (reachForward[v] && reachBack[v])
                        component.Add(v);
                }
                
                for (int i = 0; i < component.Count; i++)
                    removed[component[i]] = true;

                result.Add(component);
            }

            return result;
        }
    }

    // ==========================================================================
    // Задача 2: Максимальный поток — алгоритм проталкивания предпотока
    // ==========================================================================

    class PushRelabel
    {
        private int n;
        private int[,] capacity; 
        private int[,] flow;     
        private int[] height;    
        private int[] excess;    

        public PushRelabel(int n)
        {
            this.n = n;
            capacity = new int[n, n];
            flow = new int[n, n];
            height = new int[n];
            excess = new int[n];
        }

        public void AddEdge(int from, int to, int cap)
        {
            capacity[from, to] += cap;
        }
        
        private void Push(int u, int v)
        {
            int delta = Math.Min(excess[u], capacity[u, v] - flow[u, v]);
            flow[u, v] += delta;
            flow[v, u] -= delta; // обратное ребро
            excess[u] -= delta;
            excess[v] += delta;
        }


        private void Relabel(int u)
        {
            int minH = int.MaxValue;
            for (int v = 0; v < n; v++)
            {
                if (capacity[u, v] - flow[u, v] > 0)
                    if (height[v] < minH)
                        minH = height[v];
            }
            if (minH != int.MaxValue)
                height[u] = minH + 1;
        }

        public int MaxFlow(int source, int sink)
        {
            height[source] = n;
            for (int v = 0; v < n; v++)
            {
                if (capacity[source, v] > 0)
                {
                    flow[source, v] = capacity[source, v];
                    flow[v, source] = -capacity[source, v];
                    excess[v] = capacity[source, v];
                    excess[source] -= capacity[source, v];
                }
            }
            bool active = true;
            while (active)
            {
                active = false;
                for (int u = 0; u < n; u++)
                {
                    if (u == source || u == sink || excess[u] <= 0)
                        continue;

                    active = true;
                    
                    bool pushed = false;
                    for (int v = 0; v < n; v++)
                    {
                        if (capacity[u, v] - flow[u, v] > 0 && height[u] == height[v] + 1)
                        {
                            Push(u, v);
                            pushed = true;
                            break;
                        }
                    }
                    
                    if (!pushed)
                        Relabel(u);
                    break;
                }
            }
            int result = 0;
            for (int v = 0; v < n; v++)
                if (flow[source, v] > 0)
                    result += flow[source, v];
            return result;
        }
    }

    // ==========================================================================
    // Задача 3: Проверка изоморфности двух графов (метод перебора с отсечениями)
    // ==========================================================================

    class GraphIsomorphism
    {
        private int n;
        private bool[,] adj1;  
        private bool[,] adj2; 
        private int[] match;   
        private bool[] used;  

        public GraphIsomorphism(int n)
        {
            this.n = n;
            adj1 = new bool[n, n];
            adj2 = new bool[n, n];
            match = new int[n];
            used = new bool[n];
        }

        public void AddEdge1(int u, int v)
        {
            adj1[u, v] = true;
            adj1[v, u] = true;
        }

        public void AddEdge2(int u, int v)
        {
            adj2[u, v] = true;
            adj2[v, u] = true;
        }
        
        private int Degree1(int v)
        {
            int d = 0;
            for (int i = 0; i < n; i++)
                if (adj1[v, i]) d++;
            return d;
        }
        
        private int Degree2(int v)
        {
            int d = 0;
            for (int i = 0; i < n; i++)
                if (adj2[v, i]) d++;
            return d;
        }
        
        private bool Backtrack(int depth)
        {
            if (depth == n)
                return true; 

            for (int v2 = 0; v2 < n; v2++)
            {
                if (used[v2])
                    continue;
                
                if (Degree1(depth) != Degree2(v2))
                    continue;
                
                bool ok = true;
                for (int i = 0; i < depth; i++)
                {
                    if (adj1[depth, i] != adj2[v2, match[i]])
                    {
                        ok = false;
                        break;
                    }
                }

                if (!ok)
                    continue;

                // Назначаем идём глубже
                match[depth] = v2;
                used[v2] = true;

                if (Backtrack(depth + 1))
                    return true;
                
                used[v2] = false;
            }

            return false;
        }

        public bool IsIsomorphic()
        {
            int edges1 = 0, edges2 = 0;
            for (int i = 0; i < n; i++)
                for (int j = i + 1; j < n; j++)
                {
                    if (adj1[i, j]) edges1++;
                    if (adj2[i, j]) edges2++;
                }

            if (edges1 != edges2)
                return false;

            for (int i = 0; i < n; i++)
                used[i] = false;

            return Backtrack(0);
        }
        
        public int[] GetMapping()
        {
            return match;
        }
    }

    class Program
    {
        static void Main()
        {
            // ------------------------------------------------------------------
            // Задача 1: Компоненты сильной связности
            // Ожидаемые компоненты: {0,1,2}, {3,4}, {5}
            // ------------------------------------------------------------------
            Console.WriteLine("=== Задача 1: Компоненты сильной связности (алгоритм Мальгранжа) ===");

            StrongComponents scc = new StrongComponents(6);
            scc.AddEdge(0, 1);
            scc.AddEdge(1, 2);
            scc.AddEdge(2, 0);
            scc.AddEdge(1, 3);
            scc.AddEdge(3, 4);
            scc.AddEdge(4, 3);
            scc.AddEdge(4, 5);

            List<List<int>> components = scc.FindSCC();
            Console.WriteLine("Количество компонент: " + components.Count);
            for (int i = 0; i < components.Count; i++)
            {
                Console.Write("Компонента " + (i + 1) + ": { ");
                for (int j = 0; j < components[i].Count; j++)
                    Console.Write(components[i][j] + " ");
                Console.WriteLine("}");
            }

            // ------------------------------------------------------------------
            // Задача 2: Максимальный поток
            // Ожидаемый максимальный поток: 20
            // ------------------------------------------------------------------
            Console.WriteLine("\n=== Задача 2: Максимальный поток (алгоритм проталкивания предпотока) ===");

            PushRelabel pr = new PushRelabel(6);
            pr.AddEdge(0, 1, 10);
            pr.AddEdge(0, 2, 10);
            pr.AddEdge(1, 3, 10);
            pr.AddEdge(2, 4, 10);
            pr.AddEdge(3, 5, 10);
            pr.AddEdge(4, 5, 10);
            pr.AddEdge(1, 2, 1);

            int maxFlow = pr.MaxFlow(0, 5);
            Console.WriteLine("Максимальный поток из 0 в 5: " + maxFlow);

            // ------------------------------------------------------------------
            // Задача 3: Изоморфность графов
            // Ожидаемый результат: изоморфны
            // ------------------------------------------------------------------
            Console.WriteLine("\n=== Задача 3: Проверка изоморфности двух графов ===");

            GraphIsomorphism iso = new GraphIsomorphism(4);
            // Граф 1: квадрат 0-1-2-3-0
            iso.AddEdge1(0, 1);
            iso.AddEdge1(1, 2);
            iso.AddEdge1(2, 3);
            iso.AddEdge1(3, 0);
            // Граф 2: квадрат 0-2-1-3-0 (изоморфен, переставлены вершины)
            iso.AddEdge2(0, 2);
            iso.AddEdge2(2, 1);
            iso.AddEdge2(1, 3);
            iso.AddEdge2(3, 0);

            bool isIso = iso.IsIsomorphic();
            Console.WriteLine("Граф 1 и Граф 2 изоморфны: " + isIso);

            if (isIso)
            {
                int[] mapping = iso.GetMapping();
                Console.Write("Соответствие вершин (граф1 → граф2): ");
                for (int i = 0; i < mapping.Length; i++)
                    Console.Write(i + "→" + mapping[i] + " ");
                Console.WriteLine();
            }

            // Дополнительный пример: неизоморфные графы
            Console.WriteLine();
            GraphIsomorphism iso2 = new GraphIsomorphism(4);
            // Граф 1: путь 0-1-2-3
            iso2.AddEdge1(0, 1);
            iso2.AddEdge1(1, 2);
            iso2.AddEdge1(2, 3);
            // Граф 2: звезда, центр 0
            iso2.AddEdge2(0, 1);
            iso2.AddEdge2(0, 2);
            iso2.AddEdge2(0, 3);

            bool isIso2 = iso2.IsIsomorphic();
            Console.WriteLine("Путь 0-1-2-3 и звезда с центром 0 изоморфны: " + isIso2);
        }
    }
}
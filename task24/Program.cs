using System.Diagnostics;
using ScottPlot;
using task19;          
using Task21;          

const int DefaultRunCount = 20;
const int MediumRunCount = 7;
const int LargeRunCount = 3;
const int ExtremeRunCount = 1;

bool fullSizes = false;
bool strictRuns = false;
int? forcedRuns = null;

foreach (string x in args)
{
    if (x.Equals("--all", StringComparison.OrdinalIgnoreCase) ||
        x.Equals("-a", StringComparison.OrdinalIgnoreCase))
    {
        fullSizes = true;
        continue;
    }

    if (x.Equals("--strict-runs", StringComparison.OrdinalIgnoreCase))
    {
        strictRuns = true;
        continue;
    }

    if (x.StartsWith("--runs=", StringComparison.OrdinalIgnoreCase))
    {
        string value = x.Substring("--runs=".Length);
        if (int.TryParse(value, out int parsedRuns) && parsedRuns > 0)
            forcedRuns = parsedRuns;
    }
}

int[] sizes = fullSizes
    ? new[] { 100_000, 1_000_000, 10_000_000, 100_000_000 }
    : new[] { 100_000, 1_000_000, 10_000_000 };

string chartsDir = Path.Combine(Directory.GetCurrentDirectory(), "charts");
Directory.CreateDirectory(chartsDir);

int m = sizes.Length;
double[] putHash = new double[m];
double[] putTree = new double[m];
double[] getHash = new double[m];
double[] getTree = new double[m];
double[] remHash = new double[m];
double[] remTree = new double[m];
int[] runCounts = new int[m];

Console.WriteLine("Задача 24: сравнение MyHashMap (task21) и MyTreeMap (task19 - – красно-чёрное дерево).");
if (!fullSizes)
    Console.WriteLine("Режим: 10^5, 10^6, 10^7. Для добавления 10^8 запустите с --all.");
else
    Console.WriteLine("Режим: все размеры 10^5, 10^6, 10^7, 10^8 (--all).");

if (forcedRuns.HasValue)
    Console.WriteLine($"Прогонов на каждый размер (put/get/remove): {forcedRuns.Value} (через --runs).");
else if (strictRuns)
    Console.WriteLine($"Прогонов на каждый размер (put/get/remove): {DefaultRunCount} (фиксировано через --strict-runs).");
else
    Console.WriteLine(
        $"Прогоны адаптивные: до 10^6 -> {DefaultRunCount}, до 10^7 -> {MediumRunCount}, до 10^8 -> {ExtremeRunCount}.");

Console.WriteLine(
    "Ключи всегда уникальные (0..N-1), порядок задаётся детерминированной псевдослучайной перестановкой.\n");

for (int si = 0; si < m; si++)
{
    int n = sizes[si];
    int runCount = forcedRuns ?? (strictRuns ? DefaultRunCount : GetAdaptiveRunCount(n));
    runCounts[si] = runCount;

    Console.WriteLine($"Размер N = {n:N0} (прогонов: {runCount}) ...");

    Permutation insertOrder = new(n, 10_000 + si);
    Permutation lookupOrder = new(n, 20_000 + si);
    Permutation removeOrder = new(n, 30_000 + si);

    if (runCount == 1)
    {
        (putHash[si], getHash[si], remHash[si]) = BenchAllHashSingleRun(n, insertOrder, lookupOrder, removeOrder);
        (putTree[si], getTree[si], remTree[si]) = BenchAllTreeSingleRun(n, insertOrder, lookupOrder, removeOrder);
    }
    else
    {
        putHash[si] = BenchPutHash(n, runCount, insertOrder);
        putTree[si] = BenchPutTree(n, runCount, insertOrder);
        getHash[si] = BenchGetHash(n, runCount, insertOrder, lookupOrder);
        getTree[si] = BenchGetTree(n, runCount, insertOrder, lookupOrder);
        remHash[si] = BenchRemoveHash(n, runCount, insertOrder, removeOrder);
        remTree[si] = BenchRemoveTree(n, runCount, insertOrder, removeOrder);
    }

    Console.WriteLine(
        $"  put:    Hash {putHash[si]:F3} с, Tree {putTree[si]:F3} с (среднее за {runCount} прогонов)");
    Console.WriteLine(
        $"  get:    Hash {getHash[si]:F3} с, Tree {getTree[si]:F3} с (среднее за {runCount} прогонов)");
    Console.WriteLine(
        $"  remove: Hash {remHash[si]:F3} с, Tree {remTree[si]:F3} с (среднее за {runCount} прогонов)\n");

    GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true);
    GC.WaitForPendingFinalizers();
}

string pPut = Path.Combine(chartsDir, "put.png");
string pGet = Path.Combine(chartsDir, "get.png");
string pRem = Path.Combine(chartsDir, "remove.png");

SaveChartPng("Put (пустая карта, N вставок)", pPut, sizes, putHash, putTree);
SaveChartPng("Get (карта заполнена, N поисков)", pGet, sizes, getHash, getTree);
SaveChartPng("Remove (карта заполнена, N удалений)", pRem, sizes, remHash, remTree);

Console.WriteLine("Графики (PNG) сохранены:");
Console.WriteLine($"  {Path.GetFullPath(pPut)}");
Console.WriteLine($"  {Path.GetFullPath(pGet)}");
Console.WriteLine($"  {Path.GetFullPath(pRem)}");

Console.WriteLine("\nФактическое число прогонов:");
for (int i = 0; i < sizes.Length; i++)
    Console.WriteLine($"  N = {sizes[i]:N0}: {runCounts[i]}");

Console.WriteLine();
Console.WriteLine("--- Анализ -----");
Console.WriteLine(
    "Хеш-таблица обычно даёт амортизированное O(1) для put/get/remove при умеренной загрузке бакетов.");
Console.WriteLine(
    "Красно-чёрное дерево (task19) гарантирует O(log N) для всех операций, что на больших N медленнее хеш-таблицы, но стабильнее несбалансированного дерева.");
Console.WriteLine(
    "Итог: для одиночных операций по ключу на больших объёмах чаще выгоднее хеш-таблица; дерево полезно для упорядоченных/диапазонных операций.");

static int GetAdaptiveRunCount(int n)
{
    if (n <= 1_000_000)
        return DefaultRunCount;
    if (n <= 10_000_000)
        return MediumRunCount;
    if (n <= 50_000_000)
        return LargeRunCount;
    return ExtremeRunCount;
}

static MyHashMap<int, int> CreateHashMapForBenchmark(int n)
{
    int capacity = Math.Max(16, n);
    return new MyHashMap<int, int>(capacity, 1.0f);
}

static double BenchPutHash(int n, int runCount, Permutation insertOrder)
{
    double sum = 0;
    for (int run = 0; run < runCount; run++)
    {
        var map = CreateHashMapForBenchmark(n);
        var sw = Stopwatch.StartNew();
        for (int i = 0; i < n; i++)
        {
            int key = insertOrder.At(i);
            map.Put(key, key);
        }
        sw.Stop();
        sum += sw.Elapsed.TotalSeconds;
    }
    return sum / runCount;
}

static double BenchPutTree(int n, int runCount, Permutation insertOrder)
{
    double sum = 0;
    for (int run = 0; run < runCount; run++)
    {
        var map = new MyTreeMap<int, int>();
        var sw = Stopwatch.StartNew();
        for (int i = 0; i < n; i++)
        {
            int key = insertOrder.At(i);
            map.Put(key, key);
        }
        sw.Stop();
        sum += sw.Elapsed.TotalSeconds;
    }
    return sum / runCount;
}

static double BenchGetHash(int n, int runCount, Permutation insertOrder, Permutation lookupOrder)
{
    var map = CreateHashMapForBenchmark(n);
    for (int i = 0; i < n; i++)
    {
        int key = insertOrder.At(i);
        map.Put(key, key);
    }

    double sum = 0;
    for (int run = 0; run < runCount; run++)
    {
        var sw = Stopwatch.StartNew();
        for (int i = 0; i < n; i++)
            _ = map.Get(lookupOrder.At(i));
        sw.Stop();
        sum += sw.Elapsed.TotalSeconds;
    }
    return sum / runCount;
}

static double BenchGetTree(int n, int runCount, Permutation insertOrder, Permutation lookupOrder)
{
    var map = new MyTreeMap<int, int>();
    for (int i = 0; i < n; i++)
    {
        int key = insertOrder.At(i);
        map.Put(key, key);
    }

    double sum = 0;
    for (int run = 0; run < runCount; run++)
    {
        var sw = Stopwatch.StartNew();
        for (int i = 0; i < n; i++)
            _ = map.Get(lookupOrder.At(i));   // теперь метод Get существует
        sw.Stop();
        sum += sw.Elapsed.TotalSeconds;
    }
    return sum / runCount;
}

static double BenchRemoveHash(int n, int runCount, Permutation insertOrder, Permutation removeOrder)
{
    double sum = 0;
    for (int run = 0; run < runCount; run++)
    {
        var map = CreateHashMapForBenchmark(n);
        for (int i = 0; i < n; i++)
        {
            int key = insertOrder.At(i);
            map.Put(key, key);
        }

        var sw = Stopwatch.StartNew();
        for (int i = 0; i < n; i++)
            map.Remove(removeOrder.At(i));
        sw.Stop();
        sum += sw.Elapsed.TotalSeconds;
    }
    return sum / runCount;
}

static double BenchRemoveTree(int n, int runCount, Permutation insertOrder, Permutation removeOrder)
{
    double sum = 0;
    for (int run = 0; run < runCount; run++)
    {
        var map = new MyTreeMap<int, int>();
        for (int i = 0; i < n; i++)
        {
            int key = insertOrder.At(i);
            map.Put(key, key);
        }

        var sw = Stopwatch.StartNew();
        for (int i = 0; i < n; i++)
            map.Remove(removeOrder.At(i));
        sw.Stop();
        sum += sw.Elapsed.TotalSeconds;
    }
    return sum / runCount;
}

static (double put, double get, double remove) BenchAllHashSingleRun(
    int n,
    Permutation insertOrder,
    Permutation lookupOrder,
    Permutation removeOrder)
{
    var map = CreateHashMapForBenchmark(n);
    var sw = Stopwatch.StartNew();

    for (int i = 0; i < n; i++)
    {
        int key = insertOrder.At(i);
        map.Put(key, key);
    }
    sw.Stop();
    double put = sw.Elapsed.TotalSeconds;

    sw.Restart();
    for (int i = 0; i < n; i++)
        _ = map.Get(lookupOrder.At(i));
    sw.Stop();
    double get = sw.Elapsed.TotalSeconds;

    sw.Restart();
    for (int i = 0; i < n; i++)
        map.Remove(removeOrder.At(i));
    sw.Stop();
    double remove = sw.Elapsed.TotalSeconds;

    return (put, get, remove);
}

static (double put, double get, double remove) BenchAllTreeSingleRun(
    int n,
    Permutation insertOrder,
    Permutation lookupOrder,
    Permutation removeOrder)
{
    var map = new MyTreeMap<int, int>();
    var sw = Stopwatch.StartNew();

    for (int i = 0; i < n; i++)
    {
        int key = insertOrder.At(i);
        map.Put(key, key);
    }
    sw.Stop();
    double put = sw.Elapsed.TotalSeconds;

    sw.Restart();
    for (int i = 0; i < n; i++)
        _ = map.Get(lookupOrder.At(i));
    sw.Stop();
    double get = sw.Elapsed.TotalSeconds;

    sw.Restart();
    for (int i = 0; i < n; i++)
        map.Remove(removeOrder.At(i));
    sw.Stop();
    double remove = sw.Elapsed.TotalSeconds;

    return (put, get, remove);
}

static void SaveChartPng(string title, string path, int[] sizesArr, double[] hashY, double[] treeY)
{
    double[] xs = sizesArr.Select(s => (double)s).ToArray();
    Plot plt = new();
    plt.Title(title);
    plt.XLabel("Размер N");
    plt.YLabel("Среднее время, с");
    var h = plt.Add.Scatter(xs, hashY);
    h.LegendText = "MyHashMap";
    h.LineWidth = 2;
    h.MarkerSize = 12;
    var t = plt.Add.Scatter(xs, treeY);
    t.LegendText = "MyTreeMap (RB)";
    t.LineWidth = 2;
    t.MarkerSize = 12;
    plt.ShowLegend();
    plt.SavePng(path, 900, 600);
}

readonly struct Permutation
{
    private readonly int n;
    private readonly int multiplier;
    private readonly int offset;

    public Permutation(int n, int seed)
    {
        if (n <= 0)
            throw new ArgumentOutOfRangeException(nameof(n), "Размер перестановки должен быть положительным.");

        this.n = n;
        if (n == 1)
        {
            multiplier = 1;
            offset = 0;
            return;
        }

        Random rnd = new(seed);
        multiplier = NextCoprime(n, rnd);
        offset = rnd.Next(n);
    }

    public int At(int index)
    {
        return (int)(((long)index * multiplier + offset) % n);
    }

    private static int NextCoprime(int n, Random rnd)
    {
        while (true)
        {
            int candidate = rnd.Next(1, n);
            if (GreatestCommonDivisor(candidate, n) == 1)
                return candidate;
        }
    }

    private static int GreatestCommonDivisor(int a, int b)
    {
        while (b != 0)
        {
            int t = a % b;
            a = b;
            b = t;
        }
        return a < 0 ? -a : a;
    }
}
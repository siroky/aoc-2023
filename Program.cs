using System.Reflection;

namespace AOC;

class Program
{
    static void Main(string[] args)
    {
        var day = $"{DateTime.Now.Day:00}";
        var lines = File.ReadAllLines($"../../../Inputs/{day}.txt").ToList();
        var solver = Type.GetType($"AOC.Day{day}");
        var solve = solver.GetMethod("Solve", BindingFlags.Public | BindingFlags.Static);
        var results = solve.Invoke(null, new object[] { lines }) as IEnumerable<object>;

        foreach (var result in results)
        {
            Console.WriteLine(result);
        }
    }
}
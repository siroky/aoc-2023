namespace AOC;

public static class Extensions
{
    public static int ToInt(this string s)
    {
        return Int32.Parse(s);
    }

    public static long ToLong(this string s)
    {
        return Int64.Parse(s);
    }

    public static int ToInt(this char c)
    {
        return c.ToString().ToInt();
    }

    public static bool IsDigit(this char c)
    {
        return Char.IsDigit(c);
    }

    public static IEnumerable<string> Words(this string s)
    {
        return s.Split(" ", StringSplitOptions.RemoveEmptyEntries);
    }

    public static string Join(this IEnumerable<char> chars)
    {
        return new string(chars.ToArray());
    }

    public static string Join(this IEnumerable<string> strings, string separator = "")
    {
        return String.Join(separator, strings);
    }

    public static T Second<T>(this IEnumerable<T> items)
    {
        return items.ElementAt(1);
    }

    public static T Third<T>(this IEnumerable<T> items)
    {
        return items.ElementAt(2);
    }

    public static T Fourth<T>(this IEnumerable<T> items)
    {
        return items.ElementAt(3);
    }

    public static bool IsEmpty<T>(this IEnumerable<T> items)
    {
        return !items.Any();
    }

    public static bool NonEmpty<T>(this IEnumerable<T> items)
    {
        return !items.IsEmpty();
    }

    public static long Product<T>(this IEnumerable<T> items, Func<T, long> selector)
    {
        return items.Select(selector).Aggregate((a, b) => a * b);
    }

    public static long Product(this IEnumerable<long> items)
    {
        return items.Aggregate((a, b) => a * b);
    }

    public static IEnumerable<T> ToEnumerable<T>(this T item)
    {
        yield return item;
    }

    public static IEnumerable<T> Concat<T>(this T item, IEnumerable<T> items)
    {
        return new[] { item }.Concat(items);
    }

    public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> items)
    {
        return items.SelectMany(i => i);
    }

    public static IEnumerable<IEnumerable<T>> SplitBy<T>(this IEnumerable<T> items, Func<T, bool> predicate)
    {
        var subsequence = new List<T>();
        foreach (var item in items)
        {
            if (predicate(item))
            {
                if (subsequence.NonEmpty())
                {
                    yield return subsequence;
                    subsequence = new List<T>();
                }
            }
            else
            {
                subsequence.Add(item);
            }
        }
        if (subsequence.NonEmpty())
        {
            yield return subsequence;
        }
    }

    public static IEnumerable<int> To(this int start, int end)
    {
        var increment = start > end ? -1 : 1;
        var boundary = end + increment;
        for (var i = start; i != boundary; i += increment)
        {
            yield return i;
        }
    }

    public static IEnumerable<long> To(this long start, long end)
    {
        var increment = start > end ? -1 : 1;
        var boundary = end + increment;
        for (var i = start; i != boundary; i += increment)
        {
            yield return i;
        }
    }
}

public record Vector2(long X, long Y)
{
    public static readonly Vector2 Zero = new Vector2(0, 0);
    public static readonly Vector2 One = new Vector2(1, 1);
    public static readonly Vector2 Up = new Vector2(0, 1);
    public static readonly Vector2 Down = new Vector2(0, -1);
    public static readonly Vector2 Left = new Vector2(-1, 0);
    public static readonly Vector2 Right = new Vector2(1, 0);

    public static readonly IEnumerable<Vector2> StraightDirections = new[] { Up, Down, Left, Right };
    public static readonly IEnumerable<Vector2> DiagonalDirections = new[] { Up.Add(Left), Up.Add(Right), Down.Add(Left), Down.Add(Right) };
    public static readonly IEnumerable<Vector2> EightDirections = StraightDirections.Concat(DiagonalDirections).ToList();

    public override string ToString()
    {
        return $"({X}, {Y})";
    }
}

public static class Vector2Extensions
{
    public static bool LessOrEqual(this Vector2 a, Vector2 b)
    {
        return a.X <= b.X && a.Y <= b.Y;
    }

    public static bool GreaterOrEqual(this Vector2 a, Vector2 b)
    {
        return a.X >= b.X && a.Y >= b.Y;
    }

    public static bool In(this Vector2 v, Vector2 min, Vector2 max)
    {
        return v.GreaterOrEqual(min) && v.LessOrEqual(max);
    }

    public static bool In<T>(this Vector2 v, Grid2<T> grid)
    {
        return v.In(grid.Min, grid.Max);
    }

    public static long ManhattanLength(this Vector2 a)
    {
        return a.ManhattanDistance(Vector2.Zero);
    }

    public static long ManhattanDistance(this Vector2 a, Vector2 b)
    {
        return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
    }

    public static Vector2 Inverse(this Vector2 v)
    {
        return v.Multiply(-1);
    }

    public static Vector2 Sign(this Vector2 v)
    {
        return new Vector2(Math.Sign(v.X), Math.Sign(v.Y));
    }

    public static Vector2 Abs(this Vector2 v)
    {
        return new Vector2(Math.Abs(v.X), Math.Abs(v.Y));
    }

    public static Vector2 TurnClockwise(this Vector2 v)
    {
        return new Vector2(v.Y, -v.X);
    }

    public static Vector2 Multiply(this Vector2 v, long factor)
    {
        return new Vector2(v.X * factor, v.Y * factor);
    }

    public static Vector2 Add(this Vector2 a, Vector2 b)
    {
        return new Vector2(a.X + b.X, a.Y + b.Y);
    }

    public static Vector2 Subtract(this Vector2 a, Vector2 b)
    {
        return a.Add(b.Inverse());
    }

    public static Vector2 Divide(this Vector2 a, Vector2 b)
    {
        return new Vector2(a.X / b.X, a.Y / b.Y);
    }

    public static Vector2 Mod(this Vector2 a, Vector2 b)
    {
        return new Vector2(a.X % b.X, a.Y % b.Y);
    }

    public static Vector2 Min(this Vector2 a, Vector2 b)
    {
        return new Vector2(
            X: Math.Min(a.X, b.X),
            Y: Math.Min(a.Y, b.Y)
        );
    }

    public static Vector2 Shorter(this Vector2 a, Vector2 b)
    {
        return a.ManhattanLength() < b.ManhattanLength() ? a : b;
    }

    public static Vector2 Max(this Vector2 a, Vector2 b)
    {
        return new Vector2(
            X: Math.Max(a.X, b.X),
            Y: Math.Max(a.Y, b.Y)
        );
    }

    public static IEnumerable<Vector2> StraightAdjacent4(this Vector2 v)
    {
        return Vector2.StraightDirections.Select(d => v.Add(d));
    }

    public static IEnumerable<Vector2> EightAdjacent(this Vector2 v)
    {
        return Vector2.EightDirections.Select(d => v.Add(d));
    }

    public static Vector2 Min(this IEnumerable<Vector2> vectors)
    {
        return vectors.Aggregate((a, b) => a.Min(b));
    }

    public static Vector2 Max(this IEnumerable<Vector2> vectors)
    {
        return vectors.Aggregate((a, b) => a.Max(b));
    }
}

public record Vector3(long X, long Y, long Z);

public static class Vector3Extensions
{
    public static bool LessOrEqual(this Vector3 a, Vector3 b)
    {
        return a.X <= b.X && a.Y <= b.Y && a.Z <= b.Z;
    }

    public static long ManhattanDistance(this Vector3 a, Vector3 b)
    {
        return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y) + Math.Abs(a.Z - b.Z);
    }

    public static Vector2 ProjectXY(this Vector3 a)
    {
        return new Vector2(a.X, a.Y);
    }

    public static Vector3 Inverse(this Vector3 v)
    {
        return v.Multiply(-1);
    }

    public static Vector3 Multiply(this Vector3 v, long factor)
    {
        return new Vector3(v.X * factor, v.Y * factor, v.Z * factor);
    }

    public static Vector3 Add(this Vector3 a, Vector3 b)
    {
        return new Vector3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
    }

    public static Vector3 Subtract(this Vector3 a, Vector3 b)
    {
        return a.Add(b.Inverse());
    }

    public static Vector3 Sign(this Vector3 v)
    {
        return new Vector3(Math.Sign(v.X), Math.Sign(v.Y), Math.Sign(v.Z));
    }

    public static Vector3 Min(this Vector3 a, Vector3 b)
    {
        return new Vector3(
            X: Math.Min(a.X, b.X),
            Y: Math.Min(a.Y, b.Y),
            Z: Math.Min(a.Z, b.Z)
        );
    }

    public static Vector3 Max(this Vector3 a, Vector3 b)
    {
        return new Vector3(
            X: Math.Max(a.X, b.X),
            Y: Math.Max(a.Y, b.Y),
            Z: Math.Max(a.Z, b.Z)
        );
    }
}

public record Grid2<T>(Dictionary<Vector2, T> Items, Vector2 Min, Vector2 Max, Vector2 Size)
{
    public Grid2(Dictionary<Vector2, T> items, Vector2 min, Vector2 max)
        : this(items, min, max, max.Subtract(min).Add(Vector2.One)) { }

    public Grid2(Dictionary<Vector2, T> items)
        : this(items, items.Keys.Min(), items.Keys.Max()) { }
}

public record Grid2(Grid2<char> grid) : Grid2<char>(grid)
{
    public Grid2(Dictionary<Vector2, char> items, Vector2 min, Vector2 max)
        : this(new Grid2<char>(items, min, max)) { }
}

public static class GridExtensions
{
    public static Grid2<T> ToGrid<T>(this List<string> lines, Func<char, Vector2, T> value, Func<char, bool> predicate = null)
    {
        var items = new Dictionary<Vector2, T>();
        for (var l = 0; l < lines.Count; l++)
        {
            for (var x = 0; x < lines[l].Length; x++)
            {
                var c = lines[l][x];
                if (predicate == null || predicate(c))
                {
                    var position = new Vector2(x, lines.Count - 1 - l);
                    items.Add(position, value(c, position));
                }
            }
        }

        return new Grid2<T>(items, items.Keys.Min(), items.Keys.Max());
    }

    public static Grid2 ToGrid(this List<string> lines, Func<char, bool> predicate = null)
    {
        return new Grid2(lines.ToGrid((c, _) => c, predicate));
    }
}
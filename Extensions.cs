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

    public static string Join(this IEnumerable<string> strings)
    {
        return String.Join("", strings);
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

    public static T Fifth<T>(this IEnumerable<T> items)
    {
        return items.ElementAt(4);
    }

    public static T Sixth<T>(this IEnumerable<T> items)
    {
        return items.ElementAt(5);
    }

    public static T Seventh<T>(this IEnumerable<T> items)
    {
        return items.ElementAt(6);
    }

    public static T Eighth<T>(this IEnumerable<T> items)
    {
        return items.ElementAt(7);
    }

    public static bool NonEmpty<T>(this IEnumerable<T> items)
    {
        return items.Any();
    }

    public static int Product<T>(this IEnumerable<T> items, Func<T, int> selector)
    {
        return items.Select(selector).Aggregate((a, b) => a * b);
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

    public static IEnumerable<IEnumerable<T>> SubsequencesBy<T>(this IEnumerable<T> items, Func<T, bool> predicate)
    {
        var subsequence = new List<T>();
        foreach (var item in items)
        {
            if (predicate(item))
            {
                subsequence.Add(item);
            }
            else if (subsequence.NonEmpty())
            {
                yield return subsequence;
                subsequence = new List<T>();
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

public record Vector2(int X, int Y)
{
    public static readonly Vector2 Up = new Vector2(0, 1);
    public static readonly Vector2 Down = new Vector2(0, -1);
    public static readonly Vector2 Left = new Vector2(-1, 0);
    public static readonly Vector2 Right = new Vector2(1, 0);

    public static readonly IEnumerable<Vector2> Directions = new[] { Up, Down, Left, Right };
    public static readonly IEnumerable<Vector2> Diagonals = new[] { Up.Add(Left), Up.Add(Right), Down.Add(Left), Down.Add(Right) };
}

public record Vector3(int X, int Y, int Z);

public static class VectorExtensions
{
    public static Vector2 Inverse(this Vector2 v)
    {
        return new Vector2(-v.X, -v.Y);
    }

    public static bool LessOrEqual(this Vector3 a, Vector3 b)
    {
        return a.X <= b.X && a.Y <= b.Y && a.Z <= b.Z;
    }

    public static Vector2 Add(this Vector2 a, Vector2 b)
    {
        return new Vector2(a.X + b.X, a.Y + b.Y);
    }

    public static Vector3 Max(this Vector3 a, Vector3 b)
    {
        return new Vector3(
            X: Math.Max(a.X, b.X),
            Y: Math.Max(a.Y, b.Y),
            Z: Math.Max(a.Z, b.Z)
        );
    }

    public static IEnumerable<Vector2> Adjacent4(this Vector2 v)
    {
        return Vector2.Directions.Select(d => v.Add(d));
    }

    public static IEnumerable<Vector2> Adjacent8(this Vector2 v)
    {
        return Vector2.Directions.Concat(Vector2.Diagonals).Select(d => v.Add(d));
    }
}
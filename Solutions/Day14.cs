namespace AOC;

public static class Day14
{
    public static IEnumerable<object> Solve(List<string> lines)
    {
        var platform = ParsePlatform(lines);

        var tilted = Tilt(platform, Vector2.Up);
        yield return NorthLoad(tilted);

        var cycles = 1_000_000_000;
        var history = platform.ToEnumerable().ToList();
        for (var i = 1; i <= cycles; i++)
        {
            var cycled = TiltCycle(history.Last());
            var previous = history.FindIndex(p => Equals(cycled, p));
            if (previous >= 0)
            {
                var period = i - previous;
                var skipped = history[previous + (cycles - i) % period];
                yield return NorthLoad(skipped);
                break;
            }
            history.Add(cycled);
        }
    }

    private static long NorthLoad(Dictionary<Vector2, char> platform)
    {
        return platform.Where(kv => kv.Value == 'O').Select(kv => kv.Key.Y + 1).Sum();
    }

    private static bool Equals(Dictionary<Vector2, char> a, Dictionary<Vector2, char> b)
    {
        return a.All(kv => b.ContainsKey(kv.Key) && b[kv.Key] == kv.Value);
    }

    private static Dictionary<Vector2, char> TiltCycle(Dictionary<Vector2, char> platform)
    {
        var directions = new[] { Vector2.Up, Vector2.Left, Vector2.Down, Vector2.Right };
        return directions.Aggregate(platform, Tilt);
    }

    private static Dictionary<Vector2, char> Tilt(Dictionary<Vector2, char> platform, Vector2 direction)
    {
        var min = platform.Keys.Min();
        var max = platform.Keys.Max();
        var result = platform.Where(kv => kv.Value == '#').ToDictionary(kv => kv.Key, kv => kv.Value);

        // Determine target positions of rolling rocks, while ignoring other rolling rocks.
        var targets = new List<Vector2>();
        foreach (var (rock, c) in platform.Where(kv => kv.Value == 'O'))
        {
            var position = rock;
            while (!result.ContainsKey(position) && position.In(min, max))
            {
                position = position.Add(direction);
            }

            targets.Add(position.Subtract(direction));
        }

        // Stack the rolling rocks into the result.
        foreach (var targetGroup in targets.GroupBy(t => t))
        {
            for (var i = 0; i < targetGroup.Count(); i++)
            {
                result.Add(targetGroup.Key.Subtract(direction.Multiply(i)), 'O');
            }
        }

        return result;
    }

    private static Dictionary<Vector2, char> ParsePlatform(List<string> lines)
    {
        var result = new Dictionary<Vector2, char>();
        for (var y = 0; y < lines.Count; y++)
        {
            var line = lines[y];
            for (var x = 0; x < line.Length; x++)
            {
                var c = line[x];
                if (c != '.')
                {
                    result[new Vector2(x, lines.Count - y - 1)] = c;
                }
            }
        }

        return result;
    }
}
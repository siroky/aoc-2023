namespace AOC;

public static class Day14
{
    public static IEnumerable<object> Solve(List<string> lines)
    {
        var platform = lines.ToGrid();
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

    private static long NorthLoad(Grid2 platform)
    {
        return platform.Items.Where(kv => kv.Value == 'O').Select(kv => kv.Key.Y + 1).Sum();
    }

    private static bool Equals(Grid2 a, Grid2 b)
    {
        return a.Items.All(kv => b.Items.ContainsKey(kv.Key) && b.Items[kv.Key] == kv.Value);
    }

    private static Grid2 TiltCycle(Grid2 platform)
    {
        var directions = new[] { Vector2.Up, Vector2.Left, Vector2.Down, Vector2.Right };
        return directions.Aggregate(platform, Tilt);
    }

    private static Grid2 Tilt(Grid2 platform, Vector2 direction)
    {
        var result = platform.Items.Where(kv => kv.Value == '#').ToDictionary(kv => kv.Key, kv => kv.Value);

        // Determine target positions of rolling rocks, while ignoring other rolling rocks.
        var targets = new List<Vector2>();
        foreach (var (rock, c) in platform.Items.Where(kv => kv.Value == 'O'))
        {
            var position = rock;
            while (!result.ContainsKey(position) && position.In(platform))
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

        return new Grid2(result, platform.Min, platform.Max);
    }
}
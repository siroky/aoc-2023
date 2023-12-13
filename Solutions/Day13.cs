namespace AOC;

public static class Day13
{
    public static IEnumerable<object> Solve(List<string> lines)
    {
        var maps = lines.SplitBy(l => l.IsEmpty());
        var rocks = maps.Select(m => ParseRocks(m)).ToList();

        var notes = rocks.Select(r => ReflectionNote(r)).ToList();
        yield return notes.Sum();

        var smudgedNotes = rocks.Select(r => ReflectionNote(r, errorExpectancy: 1)).ToList();
        yield return smudgedNotes.Sum();
    }

    private static long ReflectionNote(HashSet<Vector2> rocks, int errorExpectancy = 0)
    {
        var x = ReflectionNote(rocks, v => v.X, (v, n) => new Vector2(n, v.Y), errorExpectancy);
        var y = ReflectionNote(rocks, v => v.Y, (v, n) => new Vector2(v.X, n), errorExpectancy);
        return x + 100 * y;
    }

    private static long ReflectionNote(HashSet<Vector2> rocks, Func<Vector2, long> project, Func<Vector2, long, Vector2> reflect, int errorExpectancy = 0)
    {
        var min = rocks.Aggregate((a, b) => a.Min(b));
        var max = rocks.Aggregate((a, b) => a.Max(b));

        for (var i = project(min); i < project(max); i++)
        {
            var allReflections = rocks.Select(r => reflect(r, i + (i - project(r) + 1)));
            var reflections = allReflections.Where(r => r.GreaterOrEqual(min) && r.LessOrEqual(max));
            if (reflections.Count(r => !rocks.Contains(r)) == errorExpectancy)
            {
                return i - project(min) + 1;
            }
        }

        return 0;
    }

    private static HashSet<Vector2> ParseRocks(IEnumerable<string> lines)
    {
        return lines.SelectMany((l, y) => l.Select((c, x) => (x, c == '#')).Where(t => t.Item2).Select(t => new Vector2(t.x, y))).ToHashSet();
    }
}
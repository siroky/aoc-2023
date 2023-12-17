namespace AOC;

public static class Day13
{
    public static IEnumerable<object> Solve(List<string> lines)
    {
        var maps = lines.SplitBy(l => l.IsEmpty());
        var rocks = maps.Select(m => m.ToList().ToGrid(c => c == '#')).ToList();

        var notes = rocks.Select(r => ReflectionNote(r)).ToList();
        yield return notes.Sum();

        var smudgedNotes = rocks.Select(r => ReflectionNote(r, errorExpectancy: 1)).ToList();
        yield return smudgedNotes.Sum();
    }

    private static long ReflectionNote(Grid2 rocks, int errorExpectancy = 0)
    {
        var x = ReflectionNote(rocks, v => v.X, (v, n) => new Vector2(n, v.Y), errorExpectancy);
        var y = ReflectionNote(rocks, v => v.Y, (v, n) => new Vector2(v.X, n), errorExpectancy);
        return x + 100 * (y == 0 ? 0 : rocks.Max.Y - rocks.Min.Y - y + 1);
    }

    private static long ReflectionNote(Grid2 rocks, Func<Vector2, long> project, Func<Vector2, long, Vector2> reflect, int errorExpectancy = 0)
    {
        for (var i = project(rocks.Min); i < project(rocks.Max); i++)
        {
            var allReflections = rocks.Items.Keys.Select(r => reflect(r, i + (i - project(r) + 1)));
            var reflections = allReflections.Where(r => r.In(rocks.Min, rocks.Max));
            if (reflections.Count(r => !rocks.Items.ContainsKey(r)) == errorExpectancy)
            {
                return i - project(rocks.Min) + 1;
            }
        }

        return 0;
    }
}
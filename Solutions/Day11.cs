namespace AOC;

public static class Day11
{
    public static IEnumerable<object> Solve(List<string> lines)
    {
        var galaxies = ParseGalaxies(lines.AsEnumerable().Reverse().ToList()).ToList();
        yield return Solve(galaxies, factor: 1);
        yield return Solve(galaxies, factor: 1_000_000);
    }

    private static long Solve(List<Vector2> galaxies, long factor)
    {
        var expanded = ExpandGalaxies(galaxies, factor).ToList();
        var distance = expanded.Sum(g1 => expanded.Sum(g2 => g1.ManhattanDistance(g2)));
        return distance / 2;
    }

    private static IEnumerable<Vector2> ExpandGalaxies(List<Vector2> galaxies, long factor)
    {
        var galaxyX = galaxies.Select(g => g.X).ToHashSet();
        var galaxyY = galaxies.Select(g => g.Y).ToHashSet();
        var emptyX = galaxyX.Min().To(galaxyX.Max()).Except(galaxyX).ToList();
        var emptyY = galaxyY.Min().To(galaxyY.Max()).Except(galaxyY).ToList();

        foreach (var galaxy in galaxies)
        {
            yield return new Vector2(
                galaxy.X + emptyX.Count(x => x < galaxy.X) * (factor - 1),
                galaxy.Y + emptyY.Count(y => y < galaxy.Y) * (factor - 1)
            );
        }
    }

    private static IEnumerable<Vector2> ParseGalaxies(List<string> lines)
    {
        for (var y = 0; y < lines.Count(); y++)
        {
            for (var x = 0; x < lines[y].Length; x++)
            {
                if (lines[y][x] == '#')
                {
                    yield return new Vector2(x, y);
                }
            }
        }
    }
}
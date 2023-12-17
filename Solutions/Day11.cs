namespace AOC;

public static class Day11
{
    public static IEnumerable<object> Solve(List<string> lines)
    {
        var galaxies = lines.ToGrid(c => c == '#');
        yield return Solve(galaxies, factor: 2);
        yield return Solve(galaxies, factor: 1_000_000);
    }

    private static long Solve(Grid2 galaxies, long factor)
    {
        var expanded = ExpandGalaxies(galaxies, factor).ToList();
        var distance = expanded.Sum(g1 => expanded.Sum(g2 => g1.ManhattanDistance(g2)));
        return distance / 2;
    }

    private static IEnumerable<Vector2> ExpandGalaxies(Grid2 galaxies, long factor)
    {
        var galaxyX = galaxies.Items.Keys.Select(g => g.X).ToHashSet();
        var galaxyY = galaxies.Items.Keys.Select(g => g.Y).ToHashSet();
        var emptyX = galaxies.Min.X.To(galaxies.Max.X).Except(galaxyX).ToList();
        var emptyY = galaxies.Min.Y.To(galaxies.Max.Y).Except(galaxyY).ToList();

        foreach (var galaxy in galaxies.Items.Keys)
        {
            yield return new Vector2(
                galaxy.X + emptyX.Count(x => x < galaxy.X) * (factor - 1),
                galaxy.Y + emptyY.Count(y => y < galaxy.Y) * (factor - 1)
            );
        }
    }
}
namespace AOC;

public static class Day21
{
    public static IEnumerable<object> Solve(List<string> lines)
    {
        var map = lines.ToGrid((c, p) => c == '#');
        var start = lines.ToGrid((c, p) => c == 'S').Items.Where(kv => kv.Value).First().Key;

        yield return Explore(map, start, steps: 64);

        var steps = 26501365;
        var size = map.Size.X;
        var half = size / 2;

        var tileSize = steps / size - 1;
        var oddTiles = (long)Math.Pow(tileSize / 2 * 2 + 1, 2);
        var evenTiles = (long)Math.Pow((tileSize + 1) / 2 * 2, 2);
        var oddCount = Explore(map, start, size * 2 + 1);
        var evenCount = Explore(map, start, size * 2);

        var cornerCounts = Vector2.StraightDirections.Select(d => Explore(map, start.Add(d.Multiply(half)), size - 1)).ToList();

        var smallerTiles = tileSize + 1;
        var smallerCounts = Vector2.DiagonalDirections.Select(d => Explore(map, start.Add(d.Multiply(half)), size / 2 - 1)).ToList();

        var largerTiles = tileSize;
        var largerCounts = Vector2.DiagonalDirections.Select(d => Explore(map, start.Add(d.Multiply(half)), size * 3 / 2 - 1)).ToList();

        yield return
            oddCount * oddTiles +
            evenCount * evenTiles +
            smallerTiles * smallerCounts.Sum() +
            largerTiles * largerCounts.Sum() +
            cornerCounts.Sum();
    }

    private static long Explore(Grid2<bool> map, Vector2 start, long steps)
    {
        var current = start.ToEnumerable().ToHashSet();
        for (var i = 0; i < steps; i++)
        {
            var neighbors = current.SelectMany(p => p.StraightAdjacent4());
            current = neighbors.Where(p => p.In(map) && !map.Items[p]).ToHashSet();
        }

        return current.Count();
    }
}
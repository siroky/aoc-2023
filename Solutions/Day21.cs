namespace AOC;

public static class Day21
{
    public static IEnumerable<object> Solve(List<string> lines)
    {
        var map = lines.ToGrid((c, p) => c == '#');
        var start = lines.ToGrid((c, p) => c == 'S').Items.Where(kv => kv.Value).First().Key;
        
        var destinations = Explore(map, start, steps: 64, infinite: false);
        yield return destinations;

        var infiniteDestinations = Explore(map, start, steps: 50, infinite: true);
        yield return infiniteDestinations;
    }

    private static long Explore(Grid2<bool> map, Vector2 start, long steps, bool infinite)
    {
        var current = start.ToEnumerable().ToHashSet();
        var tiles = new Dictionary<Vector2, (long Index, long Count)>();

        for (var i = 0; i < steps / 2; i++)
        {
            var neighbors = current.SelectMany(p => p.StraightAdjacent4()).Where(p => IsReachable(map, p, infinite)).ToHashSet();
            var next = neighbors.SelectMany(p => p.StraightAdjacent4()).Where(p => IsReachable(map, p, infinite) && !tiles.ContainsKey(Tile(map, p))).ToHashSet();

            var reachedTiles = next.GroupBy(p => Tile(map, p)).ToList();
            foreach (var tileGroup in reachedTiles)
            {
                var tile = tileGroup.Key;
                var nextPositions = tileGroup;
                var currentPositions = InTile(map, current, tile).ToList();

                if (nextPositions.SequenceEqual(currentPositions))
                {
                    tiles.Add(tile, (i, nextPositions.Count()));
                    next.ExceptWith(nextPositions);
                }
            }

            current = next;
        }

        return current.Count() + tiles.Values.Sum(t => t.Count);
    }

    private static bool IsReachable(Grid2<bool> map, Vector2 position, bool infinite)
    {
        return (infinite || position.In(map)) && !map.Items[position.Mod(map.Size).Add(map.Size).Mod(map.Size)];
    }

    private static IEnumerable<Vector2> InTile(Grid2<bool> map, IEnumerable<Vector2> positions, Vector2 tile)
    {
        return positions.Where(p => Tile(map, p) == tile);
    }

    private static Vector2 Tile(Grid2<bool> map, Vector2 position)
    {
        return new Vector2(
            (position.X - (position.X < 0 ? map.Size.X : 0)) / map.Size.X,
            (position.Y - (position.Y < 0 ? map.Size.Y : 0)) / map.Size.Y
        );
    }
}
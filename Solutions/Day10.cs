namespace AOC;

public static class Day10
{
    public static IEnumerable<object> Solve(List<string> lines)
    {
        var tiles = lines.ToGrid((c, p) => new Tile(p, c, new HashSet<Vector2>(c switch
        {
            '|' => new[] { Vector2.Up, Vector2.Down },
            '-' => new[] { Vector2.Left, Vector2.Right },
            'L' => new[] { Vector2.Up, Vector2.Right },
            'J' => new[] { Vector2.Up, Vector2.Left },
            '7' => new[] { Vector2.Down, Vector2.Left },
            'F' => new[] { Vector2.Down, Vector2.Right },
            'S' => Vector2.Directions,
            '.' => Enumerable.Empty<Vector2>(),
        })));
        
        var start = tiles.Items.Values.First(p => p.Type == 'S');
        var steps = Traverse(tiles, start).ToList();
        yield return steps.Count() - 1;

        var loop = steps.Flatten().ToHashSet();
        var inside = Inside(tiles, loop);
        yield return inside.Count();
    }

    private static IEnumerable<IEnumerable<Tile>> Traverse(Grid2<Tile> tiles, Tile start)
    {
        var visited = new HashSet<Tile>();
        var current = new List<Tile> { start };

        while (current.NonEmpty())
        {
            yield return current;
            visited.UnionWith(current);

            var next = new List<Tile>();
            foreach (var tile in current)
            {
                foreach (var connection in tile.Connections)
                {
                    var nextPosition = tile.Position.Add(connection);
                    var nextTile = tiles.Items.GetValueOrDefault(nextPosition);
                    if (nextTile != null && nextTile.Connections.Contains(connection.Inverse()) && !visited.Contains(nextTile))
                    {
                        next.Add(nextTile);
                    }
                }
            }

            current = next;
        }
    }

    private static IEnumerable<Tile> Inside(Grid2<Tile> tiles, HashSet<Tile> loop)
    {
        for (var y = tiles.Min.Y; y < tiles.Max.Y; y++)
        {
            var upInside = false;
            var downInside = false;

            for (var x = tiles.Min.X; x < tiles.Max.X; x++)
            {
                var tile = tiles.Items.GetValueOrDefault(new Vector2(x, y));

                if (loop.Contains(tile))
                {
                    if (tile.Connections.Contains(Vector2.Up))
                    {
                        upInside = !upInside;
                    }
                    if (tile.Connections.Contains(Vector2.Down))
                    {
                        downInside = !downInside;
                    }
                }
                else if (upInside && downInside)
                {
                    yield return tile;
                }
            }
        }
    }

    record Tile(Vector2 Position, char Type, HashSet<Vector2> Connections);
}
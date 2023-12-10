﻿namespace AOC;

public static class Day10
{
    public static IEnumerable<object> Solve(List<string> lines)
    {
        var tiles = lines.SelectMany((l, y) => l.Select((c, x) => ParseTile(x, -y, c)));
        var tileMap = tiles.ToDictionary(p => p.Position, p => p);
        
        var start = tileMap.Values.First(p => p.Type == 'S');
        var steps = Traverse(tileMap, start);
        yield return steps.Count() - 1;

        var loop = steps.Flatten().ToHashSet();
        var inside = GetInside(tileMap, loop);
        yield return inside.Count();
    }

    private static IEnumerable<Tile> GetInside(Dictionary<Vector2, Tile> tileMap, HashSet<Tile> loop)
    {
        var minY = tileMap.Keys.Min(p => p.Y);
        var maxX = tileMap.Keys.Max(p => p.X);
        for (var y = 0; y > minY; y--)
        {
            var upInside = false;
            var downInside = false;

            for (var x = 0; x < maxX; x++)
            {
                var position = new Vector2(x, y);
                var tile = tileMap.GetValueOrDefault(position);

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

    private static IEnumerable<IEnumerable<Tile>> Traverse(Dictionary<Vector2, Tile> tileMap, Tile start)
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
                    var nextTile = tileMap.GetValueOrDefault(nextPosition);
                    if (nextTile != null && nextTile.Connections.Contains(connection.Inverse()) && !visited.Contains(nextTile))
                    {
                        next.Add(nextTile);
                    }
                }
            }

            current = next;
        }
    }

    private static Tile ParseTile(int x, int y, char type)
    {
        var connections = type switch
        {
            '|' => new[] { Vector2.Up, Vector2.Down },
            '-' => new[] { Vector2.Left, Vector2.Right },
            'L' => new[] { Vector2.Up, Vector2.Right },
            'J' => new[] { Vector2.Up, Vector2.Left },
            '7' => new[] { Vector2.Down, Vector2.Left },
            'F' => new[] { Vector2.Down, Vector2.Right },
            'S' => Vector2.Directions,
            '.' => Enumerable.Empty<Vector2>(),
        };

        return new Tile(new Vector2(x, y), type, connections.ToHashSet());
    }

    record Tile(Vector2 Position, char Type, HashSet<Vector2> Connections);
}
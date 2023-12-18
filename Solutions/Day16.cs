namespace AOC;

public static class Day16
{
    public static IEnumerable<object> Solve(List<string> lines)
    {
        var grid = lines.ToGrid();
        var energized = Traverse(grid, new Beam(new Vector2(grid.Min.X, grid.Max.Y), Vector2.Right));
        yield return energized.Count;

        var verticalStarts = grid.Min.X.To(grid.Max.X).SelectMany(x => new[]
        {
            new Beam(new Vector2(x, grid.Min.Y), Vector2.Up),
            new Beam(new Vector2(x, grid.Max.Y), Vector2.Down),
        });
        var horizontalStarts = grid.Min.Y.To(grid.Max.Y).SelectMany(y => new[]
        {
            new Beam(new Vector2(grid.Min.X, y), Vector2.Right),
            new Beam(new Vector2(grid.Max.X, y), Vector2.Left),
        });
        var starts = verticalStarts.Concat(horizontalStarts).ToList();
        var energizedMap = starts.ToDictionary(s => s, s => Traverse(grid, s));
        yield return energizedMap.Values.Select(e => e.Count).Max();
    }

    private static HashSet<Vector2> Traverse(Grid2 grid, Beam start)
    {
        var visitedBeams = new HashSet<Beam>();
        var currentBeams = start.ToEnumerable().ToHashSet();

        while (currentBeams.NonEmpty())
        {
            visitedBeams.UnionWith(currentBeams);

            var newBeams = currentBeams.SelectMany(b => Step(b, grid.Items[b.Position])).ToHashSet();
            currentBeams = newBeams.Where(b => b.Position.In(grid) && !visitedBeams.Contains(b)).ToHashSet();
        }

        return visitedBeams.Select(b => b.Position).ToHashSet();
    }

    private static IEnumerable<Beam> Step(Beam b, char tile)
    {
        if (tile == '.' || b.Direction.Y == 0 && tile == '-' || b.Direction.X == 0 && tile == '|')
        {
            yield return Move(b, b.Direction);
        }
        else if (tile == '-')
        {
            yield return Move(b, Vector2.Left);
            yield return Move(b, Vector2.Right);
        }
        else if (tile == '|')
        {
            yield return Move(b, Vector2.Up);
            yield return Move(b, Vector2.Down);
        }
        else if (tile == '/')
        {
            yield return Move(b, new Vector2(b.Direction.Y, b.Direction.X));
        }
        else if (tile == '\\')
        {
            yield return Move(b, new Vector2(b.Direction.Y, b.Direction.X).Inverse());
        }
    }

    private static Beam Move(Beam beam, Vector2 direction)
    {
        return new Beam(beam.Position.Add(direction), direction);
    }

    record Beam(Vector2 Position, Vector2 Direction);
}
namespace AOC;

public static class Day17
{
    public static IEnumerable<object> Solve(List<string> lines)
    {
        var map = lines.ToGrid((c, _) => c.ToInt());
        var start = new Vector2(map.Min.X, map.Max.Y);
        var end = new Vector2(map.Max.X, map.Min.Y);

        var bestPathSmall = BestPath(map, start, end, minSteps: 1, maxSteps: 3);
        yield return bestPathSmall.Loss;

        var bestPathBig = BestPath(map, start, end, minSteps: 4, maxSteps: 10);
        yield return bestPathBig.Loss;
    }

    private static Path BestPath(Grid2<int> map, Vector2 start, Vector2 end, int minSteps, int maxSteps)
    {
        var initial = new Path(new Head(start, Vector2.Zero, 0), 0).ToEnumerable();
        var queue = new PriorityQueue<Path, int>(initial.Select(p => (p, p.Loss)));
        var seen = initial.Select(p => p.Head).ToHashSet();

        while (queue.Count > 0)
        {
            var path = queue.Dequeue();
            var head = path.Head;
            if (head.Position == end)
            {
                if (head.Steps < minSteps)
                {
                    continue;
                }
                return path;
            }
            
            var directions = Directions(head, minSteps, maxSteps);
            var allNewHeads = directions.Select(d => new Head(head.Position.Add(d), d, (d == head.Direction ? head.Steps : 0) + 1));
            var newHeads = allNewHeads.Where(h => h.Position.In(map) && !seen.Contains(h)).ToList();
            var newPaths = newHeads.Select(h => new Path(h, path.Loss + map.Items[h.Position])).ToList();
            foreach (var newPath in newPaths)
            {
                seen.Add(newPath.Head);
                queue.Enqueue(newPath, newPath.Loss);
            }
        }

        return null;
    }

    private static List<Vector2> Directions(Head head, int minSteps, int maxSteps)
    {
        var direction = head.Direction;
        var directions = new List<Vector2> { Vector2.Right, Vector2.Down, Vector2.Up, Vector2.Left };
        directions.Remove(direction.Inverse());
        if (head.Steps > 0 && head.Steps < minSteps)
        {
            directions.RemoveAll(d => d != direction);
        }
        if (head.Steps == maxSteps)
        {
            directions.Remove(direction);
        }
        return directions;
    }

    record Head(Vector2 Position, Vector2 Direction, int Steps);
    record Path(Head Head, int Loss);
}
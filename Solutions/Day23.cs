namespace AOC;

public static class Day23
{
    public static IEnumerable<object> Solve(List<string> lines)
    {
        var tracks = lines.ToGrid(c => c == '.');
        var slopes = lines.ToGrid((c, _) => ParseSlope(c), c => c != '#' && c != '.');
        var start = new Vector2(tracks.Min.X, tracks.Max.Y);
        var end = new Vector2(tracks.Max.X, tracks.Min.Y);

        var slipperyGraph = ParseGraph(tracks, slopes, start, end, slippery: true);
        yield return LongestPath(slipperyGraph, start, end);

        var normalGraph = ParseGraph(tracks, slopes, start, end, slippery: false);
        yield return LongestPath(normalGraph, start, end);
    }

    private static long LongestPath(Dictionary<Vector2, Dictionary<Vector2, long>> graph, Vector2 start, Vector2 end)
    {
        var done = new List<Dictionary<Vector2, long>>();
        var open = new List<Dictionary<Vector2, long>> { new Dictionary<Vector2, long> { { start, 0 } } };

        while (open.NonEmpty())
        {
            var newOpen = new List<Dictionary<Vector2, long>>();
            foreach (var path in open)
            {
                var last = path.Last();
                var lastPosition = last.Key;
                var lastDistance = last.Value;

                if (lastPosition == end)
                {
                    done.Add(path);
                }
                else
                {
                    var options = graph[lastPosition].Where(kv => !path.ContainsKey(kv.Key)).ToList();
                    foreach (var option in options)
                    {
                        var newPath = new Dictionary<Vector2, long>(path);
                        var newDistance = lastDistance + option.Value;
                        newPath.Add(option.Key, newDistance);
                        newOpen.Add(newPath);
                    }
                }
            }

            open = newOpen;
        }

        return done.Max(p => p.Last().Value);
    }

    private static Dictionary<Vector2, Dictionary<Vector2, long>> ParseGraph(Grid2 tracks, Grid2<Vector2> slopes, Vector2 start, Vector2 end, bool slippery)
    {
        var nodes = tracks.Items.Keys.Where(p => p == start || p == end || Neighbors(tracks, slopes, p, slippery: false).Count() > 2).ToHashSet();
        var graph = nodes.ToDictionary(n => n, n => new Dictionary<Vector2, long>());

        foreach (var node in nodes)
        {
            var distance = 1;
            var current = node.ToEnumerable().ToList();
            var visited = new HashSet<Vector2>();

            while (current.NonEmpty())
            {
                visited.UnionWith(current);
                var newCurrent = new List<Vector2>();
                var neighbors = current.SelectMany(p => Neighbors(tracks, slopes, p, slippery)).Where(n => !visited.Contains(n));
                foreach (var neighbor in neighbors)
                {
                    if (nodes.Contains(neighbor))
                    {
                        graph[node].Add(neighbor, distance);
                    }
                    else
                    {
                        newCurrent.Add(neighbor);
                    }
                }
                current = newCurrent;
                distance++;
            }
        }

        return graph;
    }

    private static IEnumerable<Vector2> Neighbors(Grid2 tracks, Grid2<Vector2> slopes, Vector2 position, bool slippery)
    {
        var options = Vector2.StraightDirections.Select(d => (P: position.Add(d), D: d));
        var viableOptions = options.Where(o =>
            tracks.Items.ContainsKey(o.P) || (
                slopes.Items.TryGetValue(o.P, out var slope) &&
                (!slippery || slope == o.D)
            )
        );
        return viableOptions.Select(o => o.P);
    }

    private static Vector2 ParseSlope(char c)
    {
        return c switch
        {
            '^' => Vector2.Up,
            'v' => Vector2.Down,
            '<' => Vector2.Left,
            '>' => Vector2.Right
        };
    }
}
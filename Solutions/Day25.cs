namespace AOC;

public static class Day25
{
    public static IEnumerable<object> Solve(List<string> lines)
    {
        var graph = ParseGraph(lines);
        var firstComponent = ConnectedComponent(graph, graph.First().Key, 4);
        var otherComponent = graph.Keys.Except(firstComponent).ToHashSet();

        yield return firstComponent.Count * otherComponent.Count;
    }

    private static HashSet<string> ConnectedComponent(Dictionary<string, HashSet<string>> graph, string start, int connectedness)
    {
        var component = start.ToEnumerable().ToHashSet();
        var nodes = graph.Keys.Except(component).ToList();
        foreach (var node in nodes)
        {
            var paths = new List<List<string>>();
            var used = new HashSet<(string, string)>();
            for (int i = 0; i < connectedness; i++)
            {
                var path = Path(graph, component, node, used);
                if (path == null)
                {
                    break;
                }
                else
                {
                    paths.Add(path);
                    used.UnionWith(path.Zip(path.Skip(1)));
                }
            }
            if (paths.Count == connectedness)
            {
                component.Add(node);
            }
        }

        return component;
    }

    private static List<string> Path(Dictionary<string, HashSet<string>> graph, HashSet<string> starts, string end, HashSet<(string, string)> used)
    {
        var visited = new HashSet<string>(starts);
        var paths = new Queue<List<string>>();
        foreach (var start in starts)
        {
            paths.Enqueue(start.ToEnumerable().ToList());
        }

        while (paths.TryDequeue(out var path))
        {
            var last = path.Last();
            if (last == end)
            {
                return path;
            }

            var neighbors = graph[last].Where(n => !visited.Contains(n) && !used.Contains((last, n)) && !used.Contains((n, last))).ToList();
            foreach (var neighbor in neighbors)
            {
                paths.Enqueue(path.Append(neighbor).ToList());
                visited.Add(neighbor);
            }
        }

        return null;
    }

    private static Dictionary<string, HashSet<string>> ParseGraph(List<string> lines)
    {
        var components = new Dictionary<string, HashSet<string>>();
        foreach (var line in lines)
        {
            var parts = line.Split(":");
            var source = parts[0];
            var targets = parts[1].Words();
            foreach (var target in targets)
            {
                components.TryAdd(source, new HashSet<string>());
                components.TryAdd(target, new HashSet<string>());

                components[source].Add(target);
                components[target].Add(source);
            }
        }
        return components;
    }
}
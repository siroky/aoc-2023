namespace AOC;

public static class Day08
{
    public static IEnumerable<object> Solve(List<string> lines)
    {
        var instructions = ParseInstructions(lines.First()).ToList();
        var nodes = lines.Skip(2).Select(l => ParseNode(l)).ToDictionary(n => n.Name, n => n);
        foreach (var node in nodes.Values)
        {
            node.Paths.Add(nodes[node.Left]);
            node.Paths.Add(nodes[node.Right]);
        }

        var camelStarts = nodes["AAA"].ToEnumerable().ToList();
        var camelEnds = nodes["ZZZ"].ToEnumerable().ToHashSet();
        var camelPath = Traverse(instructions, camelStarts, camelEnds);
        yield return camelPath;

        var ghostStarts = nodes.Values.Where(n => n.Name.EndsWith("A")).ToList();
        var ghostEnds = nodes.Values.Where(n => n.Name.EndsWith("Z")).ToHashSet();
        var ghostPath = Traverse(instructions, ghostStarts, ghostEnds);
        yield return ghostPath;
    }

    private static long Traverse(List<int> instructions, List<Node> starts, HashSet<Node> ends)
    {
        var cycles = new Dictionary<int, long>();
        var current = starts;
        var instructionIndex = 0L;

        while (current.Any(n => !ends.Contains(n)))
        {
            var instruction = instructions.ElementAt((int)(instructionIndex % instructions.Count));
            var next = current.Select(n => n.Paths[instruction]);

            current = next.ToList();
            instructionIndex++;

            // For each start, there appears to be periodic behavior after N repetitions of the instructions.
            // Determine the N for all starts, and the least common multiple of all Ns is the answer.
            if (instructionIndex % instructions.Count == 0)
            {
                for (var i = 0; i < current.Count; i++)
                {
                    if (!cycles.ContainsKey(i) && ends.Contains(current[i]))
                    {
                        cycles.Add(i, instructionIndex / instructions.Count);
                    }
                }
                if (cycles.Count == ends.Count)
                {
                    return cycles.Values.Aggregate((a, b) => a * b) * instructions.Count;
                }
            }
        }

        return instructionIndex;
    }

    private static IEnumerable<int> ParseInstructions(string line)
    {
        return line.ToCharArray().Select(c => c == 'L' ? 0 : 1);
    }

    private static Node ParseNode(string line)
    {
        var words = line.Words();
        return new Node(
            Name: words.First(),
            Left: words.Third().Substring(1, 3),
            Right: words.Fourth().Substring(0, 3),
            Paths: new List<Node>()
        );
    }

    record Node(string Name, string Left, string Right, List<Node> Paths);
}
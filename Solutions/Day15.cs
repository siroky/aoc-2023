namespace AOC;

public static class Day15
{
    public static IEnumerable<object> Solve(List<string> lines)
    {
        var sequence = lines.Join();
        var steps = sequence.Split(',').ToList();

        var hashes = steps.Select(s => Hash(s)).ToList();
        yield return hashes.Sum();

        var boxes = 0.To(255).ToDictionary(i => i, i => new List<Lens>());
        foreach (var step in steps)
        {
            var operationIndex = step.ToList().FindIndex(c => c == '-' || c == '=');
            var operation = step[operationIndex];
            var label = step.Substring(0, operationIndex);
            var boxIndex = Hash(label);
            var box = boxes[boxIndex];

            if (operation == '-')
            {
                box.RemoveAll(l => l.Label == label);
            }
            if (operation == '=')
            {
                var focalLength = step.Substring(operationIndex + 1).ToInt();
                var lens = new Lens(label, focalLength);
                var index = box.FindIndex(l => l.Label == label);
                if (index >= 0)
                {
                    box[index] = lens;
                }
                else
                {
                    box.Add(lens);
                }
            }
        }

        var powers = boxes.SelectMany(kv => kv.Value.Select((l, i) => (kv.Key + 1) * (i + 1) * l.FocalLength)).ToList();
        yield return powers.Sum();
    }

    private static int Hash(string s)
    {
        var hash = 0;
        foreach (var c in s)
        {
            hash = ((hash + c) * 17) % 256;
        }
        return hash;
    }

    record Lens(string Label, int FocalLength);
}
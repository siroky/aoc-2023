namespace AOC;

public static class Day12
{
    public static IEnumerable<object> Solve(List<string> lines)
    {
        var records = lines.Select(l => ParseRecord(l)).ToList();
        var counts = records.Select(r => CountArrangements(r.States, r.Groups)).ToList();
        yield return counts.Sum();

        var expandedRecords = lines.Select(l => ParseRecord(l, expansion: 5)).ToList();
        var expandedCounts = expandedRecords.Select(r => CountArrangements(r.States, r.Groups)).ToList();
        yield return expandedCounts.Sum();
    }

    private static Dictionary<string, long> Cache = new();

    private static long CountArrangements(List<string> states, List<int> groups)
    {
        var cacheKey = states.Join(".") + groups.Select(g => g.ToString()).Join(",");
        if (Cache.TryGetValue(cacheKey, out var count))
        {
            return count;
        }

        return Cache[cacheKey] = CalculateArrangements(states, groups);
    }

    private static long CalculateArrangements(List<string> states, List<int> groups)
    {
        // Terminate the computation when the result is clear.
        var damagedCount = states.Count(s => s.Contains('#'));
        var stateCounts = states.Select(s => s.Length).ToList();
        if (states.Count == 0)
        {
            return groups.Count == 0 ? 1 : 0;
        }
        if (groups.Count == 0)
        {
            return damagedCount == 0 ? 1 : 0;
        }
        if (groups.Count < damagedCount || groups.Sum() > stateCounts.Sum() || groups.Max() > stateCounts.Max())
        {
            return 0;
        }

        var result = 0L;
        var state = states[0];
        var group = groups[0];

        // If the state is unknown, count the branch where unknown is treated as operational.
        if (state[0] == '?')
        {
            result += CountArrangements(ShortenFirst(states, 1), groups);
        }

        // If the state is unknown or damaged, count the branch while trying to place the group.
        if (state.Length == group || state.Length > group && state[group] != '#')
        {
            result += CountArrangements(ShortenFirst(states, group + 1), groups.Skip(1).ToList());
        }

        return result;
    }

    private static List<string> ShortenFirst(List<string> states, int count)
    {
        var otherStates = states.Skip(1).ToList();
        return states[0].Length <= count ? otherStates.ToList() : states[0].Substring(count).Concat(otherStates).ToList();
    }

    private static Record ParseRecord(string line, int expansion = 1)
    {
        var parts = line.Words();
        var stateText = Enumerable.Repeat(parts.First(), expansion).Join("?");
        var groupText = Enumerable.Repeat(parts.Second(), expansion).Join(",");

        var states = stateText.Split(".", StringSplitOptions.RemoveEmptyEntries);
        var groups = groupText.Split(",").Select(s => s.ToInt());

        return new Record(states.ToList(), groups.ToList());
    }

    record Record(List<string> States, List<int> Groups);
}
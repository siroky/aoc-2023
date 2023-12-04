namespace AOC;

public static class Day04
{
    public static IEnumerable<object> Solve(List<string> lines)
    {
        var cards = lines.Select(l => ParseCard(l)).ToList();

        var matches = cards.ToDictionary(c => c.Id, c => c.Winning.Intersect(c.Guessed).Count());
        var values = matches.Values.Select(m => m == 0 ? 0 : Math.Pow(2, m - 1));
        yield return values.Sum();

        var cardCounts = cards.ToDictionary(c => c.Id, c => 1);
        foreach (var (id, count) in cardCounts)
        {
            for (var i = 1; i <= matches[id] && cardCounts.ContainsKey(id + i); i++)
            {
                cardCounts[id + i] += count;
            }
        }

        yield return cardCounts.Values.Sum();
    }

    private static Card ParseCard(string line)
    {
        var parts = line.Split(":");
        var id = parts.First().Words().Second().ToInt();
        var numberParts = parts.Second().Split("|");
        var numbers = numberParts.Select(p => p.Words().Select(n => n.ToInt()).ToHashSet());
        return new Card(id, numbers.First(), numbers.Second());
    }

    record Card(int Id, HashSet<int> Winning, HashSet<int> Guessed);
}
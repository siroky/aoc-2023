namespace AOC;

public static class Day07
{
    public static IEnumerable<object> Solve(List<string> lines)
    {
        var bids = lines.Select(l => ParseBid(l)).ToList();

        yield return CalculateWinnings(bids, jokers: false);
        yield return CalculateWinnings(bids, jokers: true);
    }

    private static long CalculateWinnings(List<Bid> bids, bool jokers)
    {
        var orderedBids = bids.OrderBy(b => Evaluate(b.Cards, jokers));
        var winnings = orderedBids.Select((b, i) => b.Value * (i + 1));

        return winnings.Sum();
    }

    private static long Evaluate(List<char> cards, bool jokers)
    {
        var unorderedGroups = cards.GroupBy(c => c).Select(g => g.ToList());
        var orderedGroups = unorderedGroups.OrderByDescending(c => c.Count()).ToList();
        var jokerGroups = orderedGroups.Where(g => jokers && g.First() == 'J').ToList();

        // Add jokers to the largest group.
        if (jokerGroups.NonEmpty() && orderedGroups.Count() > 1)
        {
            orderedGroups.Remove(jokerGroups.First());
            orderedGroups.First().AddRange(jokerGroups.First());
        }

        var groupSizes = orderedGroups.Select(g => g.Count()).ToList();
        var handTypeValue = groupSizes.First() switch
        {
            5 => 6L,
            4 => 5,
            3 => groupSizes.Second() == 2 ? 4 : 3,
            2 => groupSizes.Second() == 2 ? 2 : 1,
            _ => 0
        };

        var values = handTypeValue.Concat(cards.Select(c => Evaluate(c, jokers)));
        return values.Aggregate((a, b) => a * 100 + b);
    }

    private static long Evaluate(char card, bool jokers)
    {
        var cards = jokers ? "AKQT98765432J" : "AKQJT98765432";
        return cards.Length - cards.IndexOf(card);
    }

    private static Bid ParseBid(string bid)
    {
        var words = bid.Words();
        return new Bid(words.First().ToCharArray().ToList(), words.Second().ToLong());
    }

    record Bid(List<char> Cards, long Value);
}
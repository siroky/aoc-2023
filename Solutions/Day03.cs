namespace AOC;

public static class Day03
{
    public static IEnumerable<object> Solve(List<string> lines)
    {
        var grid = lines.Select((l, y) => l.Select((c, x) => (
            Position: new Vector2(x, y),
            Value: c
        )));

        var digitSequences = grid.SelectMany(l => l.SubsequencesBy(c => c.Value.IsDigit()));
        var numbers = digitSequences.Select(d => (
            Positions: d.Select(c => c.Position),
            Value: d.Select(c => c.Value).Join().ToInt()
        ));

        var scheme = grid.Flatten().ToDictionary(c => c.Position, c => c.Value);
        var parts = numbers.Where(n => n.Positions.SelectMany(p => p.Adjacent()).Any(p =>
            scheme.TryGetValue(p, out var c) && !c.IsDigit() && c != '.'
        )).ToList();

        yield return parts.Sum(n => n.Value);

        var asteriskPositions = grid.Flatten().Where(c => c.Value == '*').Select(c => c.Position);
        var asteriskParts = asteriskPositions.Select(p => p.Adjacent().SelectMany(p => parts.Where(n => n.Positions.Contains(p))).Distinct());
        var gearParts = asteriskParts.Where(p => p.Count() == 2);
        var gearRatios = gearParts.Select(g => g.Product(n => n.Value));

        yield return gearRatios.Sum();
    }
}
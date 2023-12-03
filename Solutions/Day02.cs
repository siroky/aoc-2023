namespace AOC;

public static class Day02
{
    public static IEnumerable<object> Solve(List<string> lines)
    {
        var games = lines.Select(l => ParseGame(l)).ToList();
        
        var bag = new Pull(R: 12, G: 13, B: 14);
        yield return games.Where(g => g.Pulls.All(p => LessOrEqual(p, bag))).Sum(g => g.Id);

        var minBags = games.Select(g => g.Pulls.Aggregate(Max));
        yield return minBags.Sum(b => b.R * b.G * b.B);
    }

    private static bool LessOrEqual(Pull a, Pull b)
    {
        return a.R <= b.R && a.G <= b.G && a.B <= b.B;
    }

    private static Pull Max(Pull a, Pull b)
    {
        return new Pull(
            R: Math.Max(a.R, b.R),
            G: Math.Max(a.G, b.G),
            B: Math.Max(a.B, b.B)
        );
    }

    private static Game ParseGame(string line)
    {
        var parts = line.Split(":");
        var id  = parts.First().Words().Second().ToInt();
        var pulls = parts.Second().Split("; ").Select(p => ParsePull(p)).ToList();
        return new Game(id, pulls);
    }

    private static Pull ParsePull(string pull)
    {
        var counts = pull.Split(", ").Select(p => p.Words());
        var cubes = counts.ToDictionary(
            p => p.Second(), 
            p => p.First().ToInt()
        );

        return new Pull(
            R: cubes.GetValueOrDefault("red"),
            G: cubes.GetValueOrDefault("green"),
            B: cubes.GetValueOrDefault("blue")
        );
    }

    record Game(int Id, List<Pull> Pulls);

    record Pull(int R, int G, int B);
}
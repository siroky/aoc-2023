namespace AOC;

public static class Day02
{
    public static IEnumerable<object> Solve(List<string> lines)
    {
        var games = lines.Select(l => ParseGame(l)).ToList();
        
        var bag = new Vector3(X: 12, Y: 13, Z: 14);
        yield return games.Where(g => g.Pulls.All(p => p.LessOrEqual(bag))).Sum(g => g.Id);

        var minBags = games.Select(g => g.Pulls.Aggregate((a, b) => a.Max(b)));
        yield return minBags.Sum(b => b.X * b.Y * b.Z);
    }

    private static Game ParseGame(string line)
    {
        var parts = line.Split(":");
        var id  = parts.First().Words().Second().ToInt();
        var pulls = parts.Second().Split("; ").Select(p => ParsePull(p)).ToList();
        return new Game(id, pulls);
    }

    private static Vector3 ParsePull(string pull)
    {
        var counts = pull.Split(", ").Select(p => p.Words());
        var cubes = counts.ToDictionary(
            p => p.Second(), 
            p => p.First().ToInt()
        );

        return new Vector3(
            X: cubes.GetValueOrDefault("red"),
            Y: cubes.GetValueOrDefault("green"),
            Z: cubes.GetValueOrDefault("blue")
        );
    }

    record Game(int Id, List<Vector3> Pulls);

    record Pull(int R, int G, int B);
}
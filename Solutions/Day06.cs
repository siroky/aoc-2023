namespace AOC;

public static class Day06
{
    public static IEnumerable<object> Solve(List<string> lines)
    {
        var digits = lines.Select(l => l.Words().Skip(1)).ToList();
        var numbers = digits.Select(d => d.Select(i => i.ToLong())).ToList();
        var records = numbers.First().Zip(numbers.Second(), (t, d) => new Result(t, d)).ToList();

        yield return records.Select(r => GetBetterResults(r)).Product(r => r.Count());

        var correctNumbers = digits.Select(d => d.Join().ToLong());
        var correctRecord = new Result(correctNumbers.First(), correctNumbers.Second());
        yield return GetBetterResults(correctRecord).Count();
    }

    private static IEnumerable<Result> GetBetterResults(Result record)
    {
        foreach (var holdTime in 1L.To(record.Time - 1))
        {
            var speed = holdTime;
            var rideTime = record.Time - holdTime;
            var distance = speed * rideTime;
            if (distance > record.Distance)
            {
                yield return new Result(record.Time, distance);
            }
        }
    }

    record Result(long Time, long Distance);
}
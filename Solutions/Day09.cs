﻿namespace AOC;

public static class Day09
{
    public static IEnumerable<object> Solve(List<string> lines)
    {
        var histories = lines.Select(l => l.Words().Select(w => w.ToLong()).ToList()).ToList();
        var extrapolations = histories.Select(h => Extrapolate(h)).ToList();

        yield return extrapolations.Sum(e => e.Next);
        yield return extrapolations.Sum(e => e.Prev);
    }

    private static (long Prev, long Next) Extrapolate(List<long> history)
    {
        var sequences = new List<List<long>>() { history };
        var sequence = history;
        while (sequences.Last().Any(i => i != 0))
        {
            sequence = sequence.Zip(sequence.Skip(1), (a, b) => b - a).ToList();
            sequences.Add(sequence);
        }

        var next = 0L;
        var prev = 0L;
        foreach (var s in sequences.AsEnumerable().Reverse())
        {
            next = s.Last() + next;
            prev = s.First() - prev;
        }

        return (prev, next);
    }
}
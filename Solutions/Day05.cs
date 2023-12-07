namespace AOC;

public static class Day05
{
    public static IEnumerable<object> Solve(List<string> lines)
    {
        var parts = lines.SubsequencesBy(l => l.NonEmpty());
        var seeds = parts.First().First().Words().Skip(1).Select(i => i.ToLong()).ToList();
        var maps = parts.Skip(1).Select(p => ParseMap(p).ToList()).ToList();

        yield return GetMinTarget(seeds.Select(s => new Range(Start: s, Length: 1)), maps);
        yield return GetMinTarget(seeds.Chunk(2).Select(c => new Range(Start: c.First(), Length: c.Second())), maps);
    }

    private static long GetMinTarget(IEnumerable<Range> sources, List<List<Mapping>> maps)
    {
        // One source range can be mapped to multiple target ranges.
        // Merging the target ranges, where possible, is not needed at the moment.
        var targets = maps.Aggregate(
            seed: sources.AsEnumerable(),
            func: (sources, map) => sources.SelectMany(source => Map(source, map))
        );

        return targets.Min(t => t.Start);
    }

    private static IEnumerable<Range> Map(Range source, List<Mapping> map)
    {
        var remainder = source;
        while (remainder.Length > 0)
        {
            var mapping = map.FirstOrDefault(m => m.Source <= remainder.Start && remainder.Start < m.Source + m.Length);
            if (mapping == null)
            {
                // Use artificial identity mapping in case of missing explicit mapping.
                // The identity mapping lasts until the next explicit mapping.
                // If there is no next explicit mapping, the identity mapping lasts until the end of the range.
                var nextSources = map.Select(m => m.Source).Where(x => x > remainder.Start);
                mapping = new Mapping(
                    Source: remainder.Start,
                    Target: remainder.Start,
                    Length: nextSources.Any() ? nextSources.Min() - remainder.Start : remainder.Length
                );
            }

            var offset = remainder.Start - mapping.Source;
            var target = new Range(
                Start: mapping.Target + offset,
                Length: Math.Min(mapping.Length - offset, remainder.Length)
            );

            yield return target;
            remainder = new Range(
                Start: remainder.Start + target.Length,
                Length: remainder.Length - target.Length
            );
        }
    }

    private static IEnumerable<Mapping> ParseMap(IEnumerable<string> lines)
    {
        var numbers = lines.Skip(1).Select(l => l.Words().Select(w => w.ToLong()));
        return numbers.Select(n => new Mapping(
            Source: n.Second(),
            Target: n.First(),
            Length: n.Third()
        ));
    }

    record Range(long Start, long Length);
    record Mapping(long Source, long Target, long Length);
}
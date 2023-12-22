namespace AOC;

public static class Day22
{
    public static IEnumerable<object> Solve(List<string> lines)
    {
        var bricks = lines.Select((l, i) => ParseBrick(i, l)).ToList();
        var below = Settle(bricks);
        var cascades = Cascades(below);

        yield return cascades.Count(kv => kv.Value.Count == 1);
        yield return cascades.Sum(kv => kv.Value.Count - 1);
    }

    private static Dictionary<Brick, HashSet<Brick>> Cascades(Dictionary<Brick, HashSet<Brick>> below)
    {
        var above = Above(below);
        var result = new Dictionary<Brick, HashSet<Brick>>();

        foreach (var brick in below.Keys.OrderByDescending(b => b.Min.Z))
        {
            var standing = result.Keys.ToHashSet();
            var fallen = brick.ToEnumerable().ToHashSet();
            while (true)
            {
                var next = standing.FirstOrDefault(sb => below[sb].NonEmpty() && below[sb].All(bb => fallen.Contains(bb)));
                if (next != null)
                {
                    foreach (var fallenBrick in result[next])
                    {
                        standing.Remove(fallenBrick);
                        fallen.Add(fallenBrick);
                    }
                }
                else
                {
                    break;
                }
            }

            result.Add(brick, fallen);
        }

        return result;
    }

    private static Dictionary<Brick, HashSet<Brick>> Above(Dictionary<Brick, HashSet<Brick>> below)
    {
        var above = below.Keys.ToDictionary(b => b, b => new HashSet<Brick>());
        foreach (var brick in above.Keys)
        {
            foreach (var brickBelow in below[brick])
            {
                above[brickBelow].Add(brick);
            }
        }

        return above;
    }

    private static Dictionary<Brick, HashSet<Brick>> Settle(List<Brick> bricks)
    {
        var result = new Dictionary<Brick, HashSet<Brick>>();
        var occupied = new Dictionary<Vector3, Brick>();

        foreach (var brick in bricks.OrderBy(b => b.Min.Z))
        {
            var length = brick.Max.ManhattanDistance(brick.Min);
            var cubes = 0L.To(length).Select(i => brick.Min.Add(brick.Direction.Multiply(i))).ToList();
            var bottom = cubes.Select(c => c.ProjectXY()).ToHashSet();

            var below = occupied.Where(kv => bottom.Contains(kv.Key.ProjectXY())).ToList();
            var height = below.NonEmpty() ? below.Max(kv => kv.Key.Z) : 0;
            var fall = new Vector3(0, 0, (height + 1) - brick.Min.Z);
            var fallenBrick = new Brick(brick.Id, brick.Min.Add(fall), brick.Max.Add(fall));

            var support = below.Where(kv => kv.Key.Z == height);
            var supportingBricks = support.Select(kv => kv.Value).ToHashSet();

            result.Add(fallenBrick, supportingBricks);
            foreach (var cube in cubes)
            {
                occupied.Add(cube.Add(fall), fallenBrick);
            }
        }

        return result;
    }

    private static Brick ParseBrick(long id, string line)
    {
        var parts = line.Split("~");
        var numbers = parts.Select(p => p.Split(",").Select(n => n.ToInt()).ToList()).ToList();
        var start = new Vector3(numbers[0][0], numbers[0][1], numbers[0][2]);
        var end = new Vector3(numbers[1][0], numbers[1][1], numbers[1][2]);

        return new Brick(id, start.Min(end), start.Max(end));
    }

    record Brick(long Id, Vector3 Min, Vector3 Max, Vector3 Diff, Vector3 Direction)
    {
        public Brick(long id, Vector3 min, Vector3 max)
            : this(id, min, max, max.Subtract(min), max.Subtract(min).Sign()) { }
    }
}
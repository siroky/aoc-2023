namespace AOC;

public static class Day18
{
    public static IEnumerable<object> Solve(List<string> lines)
    {
        var commands = lines.Select(l => ParseCommand(l)).ToList();
        yield return Dig(commands);

        var correctCommands = lines.Select(l => ParseCorrectCommand(l)).ToList();
        yield return Dig(correctCommands);
    }

    private static long Dig(List<Command> commands)
    {
        var points = CreatePoints(commands);
        var size = 0L;
        while (points.NonEmpty())
        {
            if (points.Count <= 4)
            {
                var diagonal = points.Max().Subtract(points.Min()).Add(Vector2.One);
                size += diagonal.X * diagonal.Y;
                break;
            }

            // Find a rectangle that can be safely removed from the shape. Or remove unnecessary points.
            for (var i = 0; i < points.Count; i++)
            {
                var p1 = GetPoint(points, i);
                var p2 = GetPoint(points, i + 1);
                var p3 = GetPoint(points, i + 2);
                var p4 = GetPoint(points, i + 3);

                var firstDiff = p2.Subtract(p1);
                var secondDiff = p3.Subtract(p2);
                var thirdDiff = p4.Subtract(p3);
                var firstDirection = firstDiff.Sign();
                var secondDirection = secondDiff.Sign();
                var thirdDirection = thirdDiff.Sign();

                if (firstDirection.Abs() == secondDirection.Abs())
                {
                    if (firstDirection.Inverse() == secondDirection)
                    {
                        var newP2 = p2.Add(firstDiff.Inverse().Shorter(secondDiff));
                        size += p2.ManhattanDistance(newP2);
                    }

                    points.Remove(p2);
                    break;
                }

                if (firstDirection.Inverse() == thirdDirection && firstDirection.TurnClockwise() == secondDirection)
                {
                    var shorterDiff = firstDiff.Inverse().Shorter(thirdDiff);
                    var newP1 = p2.Add(shorterDiff);
                    var newP4 = p3.Add(shorterDiff);
                    var rectangle = new[] { newP1, p2, p3, newP4 };
                    var min = rectangle.Min();
                    var max = rectangle.Max();

                    var expectedCount = firstDiff.Inverse() == thirdDiff ? 4 : 3;
                    if (points.Count(p => p.In(min, max)) == expectedCount)
                    {
                        var diagonal = max.Subtract(min).Subtract(firstDirection.Abs()).Add(Vector2.One);
                        size += diagonal.X * diagonal.Y;

                        if (points.Contains(newP1) && points.Contains(newP4))
                        {
                            points.Remove(p2);
                            points.Remove(p3);
                        }
                        else if (points.Contains(newP1))
                        {
                            points.Remove(p2);
                            points[points.IndexOf(p3)] = newP4;
                        }
                        else
                        {
                            points.Remove(p3);
                            points[points.IndexOf(p2)] = newP1;
                        }
                        break;
                    }
                }
            }
        }

        return size;
    }

    private static List<Vector2> CreatePoints(List<Command> commands)
    {
        var points = new List<Vector2>();
        var current = Vector2.Zero;
        foreach (var command in commands)
        {
            current = current.Add(command.Direction.Multiply(command.Count));
            points.Add(current);
        }

        // Make sure the points are in clockwise order.
        var minY = points.Min(p => p.Y);
        var corner = points.Where(p => p.Y == minY).MinBy(p => p.X);
        var cornerIndex = points.IndexOf(corner);
        var next = GetPoint(points, cornerIndex + 1);
        if (next.Y == minY)
        {
            points.Reverse();
        }

        return points;
    }

    private static Vector2 GetPoint(List<Vector2> points, int index)
    {
        return points[(index + points.Count) % points.Count];
    }

    private static Command ParseCommand(string line)
    {
        var words = line.Words();
        return new Command(words.Second().ToInt(), words.First() switch
        {
            "U" => Vector2.Up,
            "R" => Vector2.Right,
            "D" => Vector2.Down,
            "L" => Vector2.Left
        });
    }

    private static Command ParseCorrectCommand(string line)
    {
        var code = line.Words().Third().Skip(2).ToList();
        var countHex = code.Take(5).Join();
        return new Command(Convert.ToInt32(countHex, 16), code.ElementAt(5) switch
        {
            '0' => Vector2.Right,
            '1' => Vector2.Down,
            '2' => Vector2.Left,
            '3' => Vector2.Up
        });
    }

    record Command(long Count, Vector2 Direction);
}
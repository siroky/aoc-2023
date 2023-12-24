namespace AOC;

public static class Day24
{
    public static IEnumerable<object> Solve(List<string> lines)
    {
        var stones = lines.Select(l => ParseStone(l)).ToList();
        var min = 200000000000000L;
        var max = 400000000000000;
        var intersections = Intersections(stones, new Vector2(min, min), new Vector2(max, max)).ToList();
        yield return intersections.Count;

        Console.WriteLine($"pX, pY, pZ, vX, vY, vZ, t1, t2, t3 = symbols(\"pX, pY, pZ, vX, vY, vZ, t1, t2, t3\")");
        Console.WriteLine($"equations = []");
        for (var i = 1; i <= 3; i++)
        {
            var s = stones[i];
            Console.WriteLine($"equations.append({s.Position.X} + ({s.Velocity.X} * t{i}) - (pX + vX * t{i}))");
            Console.WriteLine($"equations.append({s.Position.Y} + ({s.Velocity.Y} * t{i}) - (pY + vY * t{i}))");
            Console.WriteLine($"equations.append({s.Position.Z} + ({s.Velocity.Z} * t{i}) - (pZ + vZ * t{i}))");
        }
        Console.WriteLine($"results = solve(equations)");
        Console.WriteLine($"print(results)");
        Console.WriteLine($"print(results[0][pX] + results[0][pY] + results[0][pZ])");
    }

    private static IEnumerable<(Stone, Stone)> Intersections(List<Stone> stones, Vector2 min, Vector2 max)
    {
        for (var i = 0; i < stones.Count; i++)
        {
            for (var j = i + 1; j < stones.Count; j++)
            {
                var a = stones[i];
                var b = stones[j];
                if (FutureIntersectionXY(a, b, min, max))
                {
                    yield return (a, b);
                }
            }
        }
    }

    private static bool FutureIntersectionXY(Stone s1, Stone s2, Vector2 min, Vector2 max)
    {
        var p1 = s1.Position.ProjectXY();
        var v1 = s1.Velocity.ProjectXY();
        var p2 = s2.Position.ProjectXY();
        var v2 = s2.Velocity.ProjectXY();
        var pd = p2.Subtract(p1);

        // Equation for intersection of paths defined by the stones:
        // p1 + v1 * t1 = p2 + v2 * t2
        //
        // Determine t1 using X coordinate version of the equation:
        // t1 = (p2.X + v2.X * t2 - p1.X) / v1.X
        // t1 = (pd.X + v2.X * t2) / v1.X
        //
        // Determine t2 using Y coordinate version of the equation:
        // p1.Y + [v1.Y * (pd.X + v2.X * t2) / v1.X] = p2.Y + v2.Y * t2
        //                               p1.Y - p2.Y = (v2.Y * t2) - [v1.Y * (pd.X + v2.X * t2) / v1.X]
        //                                    - pd.Y = (v2.Y * t2) - (v1.Y * pd.X / v1.X) - (v1.Y * v2.X * t2 / v1.X)
        //             - pd.Y + (v1.Y * pd.X / v1.X) = (v2.Y * t2) - (v1.Y * v2.X * t2 / v1.X)
        //                                        t2 = [- pd.Y + (v1.Y * pd.X / v1.X)] / [v2.Y - (v1.Y * v2.X / v1.X)]

        var numerator = -pd.Y + ((decimal)v1.Y * pd.X / v1.X);
        var denominator = v2.Y - ((decimal)v1.Y * v2.X / v1.X);
        if (denominator == 0)
        {
            // Parallel paths.
            return false;
        }

        var t2 = numerator / denominator;
        var t1 = (pd.X + v2.X * t2) / v1.X;
        if (t1 < 0 || t2 < 0)
        {
            // Intersection is in the past.
            return false;
        }

        var intersectionX = p2.X + v2.X * t2;
        var intersectionY = p2.Y + v2.Y * t2;
        if (intersectionX < min.X || intersectionX > max.X || intersectionY < min.Y || intersectionY > max.Y)
        {
            // Intersection is outside the bounds.
            return false;
        }

        return true;
    }

    private static Stone ParseStone(string line)
    {
        var parts = line.Split(" @ ");
        var numbers = parts.Select(p => p.Split(", ").Select(c => c.ToLong()));
        var vectors = numbers.Select(n => new Vector3(n.First(), n.Second(), n.Third()));
        return new Stone(vectors.First(), vectors.Second());
    }

    record Stone(Vector3 Position, Vector3 Velocity);
}
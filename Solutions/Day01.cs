namespace AOC;

static class Day01
{
    public static IEnumerable<object> Solve(List<string> lines)
    {
        yield return lines.Select(l => GetValue(l, textual: false)).Sum();
        yield return lines.Select(l => GetValue(l, textual: true)).Sum();
    }

    private static int GetValue(string line, bool textual)
    {
        var firstDigit = GetDigit(line, reverse: false, textual);
        var lastDigit = GetDigit(line, reverse: true, textual);
        return 10 * firstDigit + lastDigit;
    }

    private static int GetDigit(string line, bool reverse, bool textual)
    {
        var lastIndex = line.Length - 1;
        var startIndex = reverse ? lastIndex : 0;
        var endIndex = reverse ? 0 : lastIndex;

        foreach (var index in startIndex.To(endIndex))
        {
            if (line[index].IsDigit())
            {
                return line[index].ToInt();
            }
            if (textual)
            {
                for (var d = 0; d < Digits.Count; d++)
                {
                    var digit = Digits[d];
                    if (line.Substring(index).StartsWith(digit))
                    {
                        return d + 1;
                    }
                }
            }
        }

        return 0;
    }

    private static List<string> Digits = new List<string>
    {
        "one",
        "two",
        "three",
        "four",
        "five",
        "six",
        "seven",
        "eight",
        "nine"
    };
}
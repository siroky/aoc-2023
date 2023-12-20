namespace AOC;

public static class Day19
{
    public static IEnumerable<object> Solve(List<string> lines)
    {
        var blocks = lines.SplitBy(l => l.IsEmpty());
        var workflows = blocks.First().Select(l => ParseWorkflow(l)).ToDictionary(w => w.Name, w => w);
        var parts = blocks.Second().Select(l => ParsePart(l)).ToList();

        var results = parts.ToDictionary(p => p, p => Process(workflows, p, "in"));
        var accepted = results.Where(kv => kv.Value == "A").Select(kv => kv.Key).ToList();
        var ratings = accepted.Select(p => p.X + p.M + p.A + p.S).ToList();
        yield return ratings.Sum();

        var conditionSets = Reverse(workflows, "in", "A").ToList();
        var options = conditionSets.Select(s => CalculateOptions(s)).ToList();
        yield return options.Sum();
    }

    private static IEnumerable<List<RuleCondition>> Reverse(Dictionary<string, Workflow> workflows, string start, string end)
    {
        if (start == end)
        {
            yield return new List<RuleCondition>();
        }
        else
        {
            var sources = new List<(Workflow Workflow, List<RuleCondition> Conditions)>();
            foreach (var workflow in workflows.Values)
            {
                var gates = new List<RuleCondition>();
                foreach (var rule in workflow.Rules)
                {
                    if (rule.Target == end)
                    {
                        sources.Add((workflow, rule.Condition.Concat(gates).Where(c => c != null).ToList()));
                    }
                    if (rule.Condition != null)
                    {
                        gates.Add(Negate(rule.Condition));
                    }
                }
            }

            var sourceWorkflows = sources.Select(s => s.Workflow).ToHashSet();
            foreach (var workflow in sourceWorkflows)
            {
                var results = Reverse(workflows, start, workflow.Name).ToList();
                foreach (var result in results)
                {
                    foreach (var source in sources.Where(s => s.Workflow == workflow))
                    {
                        yield return result.Concat(source.Conditions).ToList();
                    }
                }
            }
        }
    }

    private static long CalculateOptions(List<RuleCondition> conditions)
    {
        var options = new[] { "x", "m", "a", "s" }.ToDictionary(p => p, p => 1.To(4000).ToList());
        foreach (var condition in conditions)
        {
            options[condition.Parameter].RemoveAll(i => !Evaluate(condition, i));
        }

        return options.Values.Product(l => l.Count);
    }

    private static string Process(Dictionary<string, Workflow> workflows, Part part, string start)
    {
        var current = start;
        while (current != "A" && current != "R")
        {
            foreach (var rule in workflows[current].Rules)
            {
                if (rule.Condition == null || Evaluate(rule.Condition, part))
                {
                    current = rule.Target;
                    break;
                }
            }
        }

        return current;
    }

    private static bool Evaluate(RuleCondition condition, Part part)
    {
        return Evaluate(condition, condition.Parameter switch
        {
            "x" => part.X,
            "m" => part.M,
            "a" => part.A,
            "s" => part.S,
        });
    }

    private static bool Evaluate(RuleCondition condition, int value)
    {
        return condition.Operation switch
        {
            "<" => value < condition.Value,
            ">" => value > condition.Value,
        };
    }

    private static RuleCondition Negate(RuleCondition condition)
    {
        return new RuleCondition(
            Parameter: condition.Parameter,
            Operation: condition.Operation == "<" ? ">" : "<",
            Value: condition.Value + (condition.Operation == "<" ? -1 : 1)
        );
    }

    private static Workflow ParseWorkflow(string line)
    {
        var part = line.Split('{');
        var name = part.First();
        var rulePart = part.Second().Replace("}", "");
        var rules = rulePart.Split(",").Select(t => ParseRule(t)).ToList();
        return new Workflow(name, rules);
    }

    private static Rule ParseRule(string text)
    {
        var parts = text.Split(":");
        var condition = parts.First();
        var target = parts.Last();

        return new Rule(target, parts.Count() == 1 ? null : new RuleCondition(
            Parameter: condition.Substring(0, 1),
            Operation: condition.Substring(1, 1),
            Value: condition.Substring(2).ToInt()
        ));
    }

    private static Part ParsePart(string line)
    {
        var parts = line.Substring(1, line.Length - 2).Split(',');
        var numbers = parts.Select(p => p.Split('=').Second().ToInt()).ToList();
        return new Part(numbers.First(), numbers.Second(), numbers.Third(), numbers.Fourth());
    }

    record Workflow(string Name, List<Rule> Rules);

    record Rule(string Target, RuleCondition Condition = null);

    record RuleCondition(string Parameter, string Operation, int Value);

    record Part(int X, int M, int A, int S);
}
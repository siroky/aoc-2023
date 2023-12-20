namespace AOC;

public static class Day20
{
    public static IEnumerable<object> Solve(List<string> lines)
    {
        var modules = lines.Select(l => ParseModule(l)).ToDictionary(m => m.Name, m => m);
        var result = Simulate(modules, new Pulse("button", "broadcaster", Value: false), max: 1000);
        yield return result;

        var index = Simulate(modules, new Pulse("button", "broadcaster", Value: false), target: "vd");
        yield return index + 1;
    }

    private static long Simulate(Dictionary<string, Module> modules, Pulse initial, long? max = null, string target = null)
    {
        var low = 0L;
        var high = 0L;
        var states = modules.Values.ToDictionary(m => m.Name, m => InitialState(modules, m));
        var sources = modules.Values.Where(m => m.Destinations.Contains(target)).Select(m => m.Name).ToHashSet();
        var periods = new Dictionary<string, long>();

        var i = 1;
        while (!max.HasValue || i <= max)
        {
            var pulses = new Queue<Pulse>(initial.ToEnumerable());
            while (pulses.NonEmpty())
            {
                var pulse = pulses.Dequeue();
                var _ = pulse.Value ? high++ : low++;
                if (sources.Contains(pulse.Source) && pulse.Value)
                {
                    periods.TryAdd(pulse.Source, i);
                    if (periods.Count == sources.Count)
                    {
                        return periods.Values.Product();
                    }
                }

                if (modules.TryGetValue(pulse.Module, out var module))
                {
                    var state = states[pulse.Module];
                    var result = Simulate(module, state, pulse);
                    if (result.HasValue)
                    {
                        foreach (var destination in module.Destinations)
                        {
                            pulses.Enqueue(new Pulse(pulse.Module, destination, result.Value));
                        }
                    }
                }
            }

            i++;
        }

        return high * low;
    }

    private static bool? Simulate(Module module, Dictionary<string, bool> state, Pulse pulse)
    {
        if (module.Type == ModuleType.Broadcaster)
        {
            return pulse.Value;
        }
        else if (module.Type == ModuleType.Flip && !pulse.Value)
        {
            return state["on"] = !state["on"];
        }
        else if (module.Type == ModuleType.Nand)
        {
            state[pulse.Source] = pulse.Value;
            return !state.Values.All(v => v);
        }
        
        return null;
    }

    private static Dictionary<string, bool> InitialState(Dictionary<string, Module> modules, Module module)
    {
        return module.Type switch
        {
            ModuleType.Broadcaster => new Dictionary<string, bool>(),
            ModuleType.Flip => new Dictionary<string, bool> { { "on", false } },
            ModuleType.Nand => modules.Values.Where(m => m.Destinations.Contains(module.Name)).ToDictionary(m => m.Name, m => false)
        };
    }

    private static Module ParseModule(string line)
    {
        var parts = line.Split(" -> ");
        var type = parts[0][0] switch
        {
            '%' => ModuleType.Flip,
            '&' => ModuleType.Nand,
            _ => ModuleType.Broadcaster
        };
        var name = parts[0].Substring(type == ModuleType.Broadcaster ? 0 : 1);
        var destinations = parts[1].Split(", ").ToList();
        return new Module(name, type, destinations);
    }

    public record Pulse(string Source, string Module, bool Value);

    public record Module(string Name, ModuleType Type, List<string> Destinations);

    public enum ModuleType
    {
        Broadcaster,
        Flip,
        Nand
    }
}
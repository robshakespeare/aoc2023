using System.Diagnostics;

namespace AoC.Day20;

public partial class Day20Solver : ISolver
{
    private const string Broadcaster = "broadcaster";

    public string DayName => "Pulse Propagation";

    public long? SolvePart1(string input)
    {
        var modules = ParseModuleConfiguration(input);

        const int numButtonPresses = 1000;

        long numLowPulsesSent = 0;
        long numHighPulsesSent = 0;

        for (var buttonPress = 1; buttonPress <= numButtonPresses; buttonPress++)
        {
            List<SendPulse> sendPulses = [new SendPulse(Pulse.Low, Broadcaster, "button")];

            while (sendPulses.Count > 0)
            {
                List<SendPulse> newSendPulses = [];

                foreach (var sendPulse in sendPulses)
                {
                    if (sendPulse.Pulse == Pulse.Low)
                    {
                        numLowPulsesSent++;
                    }
                    else
                    {
                        numHighPulsesSent++;
                    }

                    var destinationModule = modules[sendPulse.DestinationModuleName];

                    newSendPulses.AddRange(destinationModule.ReceivePulse(sendPulse.Pulse, sendPulse.InputModuleName));
                }

                sendPulses = newSendPulses;
            }
        }

        return numLowPulsesSent * numHighPulsesSent;
    }

    public long? SolvePart2(string input)
    {
        var modules = ParseModuleConfiguration(input);

        // Display inputs:
        foreach (var module in modules.Values.OrderBy(x => x.Name))
        {
            Console.WriteLine($"{module.FullName} has inputs: {string.Join(", ", module.InputNames)}");
        }

        Console.WriteLine();

        // Work backwards from `rx`, building our "tree"
        //var rx = modules["rx"];
        //var paths = new List<List<Module>>();

        List<Module[]> paths = [];

        void Enumerate(string name, Module[] path)
        {
            var module = modules[name];
            Module[] nextPath = [.. path, module];

            if (name == Broadcaster)
            {
                Console.WriteLine($"Path to Broadcaster found: {string.Join(" > ", nextPath.Select(x => x.FullName))}");
                paths.Add(path);
            }
            else
            {
                foreach (var input in module.InputNames)
                {
                    var alreadySeen = path.Any(x => x.Name == input);

                    if (!alreadySeen)
                    {
                        Enumerate(input, nextPath);
                    }
                }
            }
        }

        Enumerate("rx", []);

        var distinctModulesNeeded = paths.SelectMany(p => p).Distinct().ToArray();

        Console.WriteLine($"distinctModulesNeeded: {distinctModulesNeeded.Length}");
        Console.WriteLine($"#modules: {modules.Count}");

        return null;

        throw new NotImplementedException("rs-todo: solve!");

        const int numButtonPresses = int.MaxValue;

        long numLowPulsesSent = 0;
        long numHighPulsesSent = 0;

        var stopwatch = Stopwatch.StartNew();
        var reportInterval = TimeSpan.FromSeconds(5);
        var nextReportAt = reportInterval;
        var hf = (ConjunctionModule)modules["hf"];

        var report = (int buttonPress, int sendPulsesCount) =>
        {
            if (stopwatch.Elapsed > nextReportAt)
            {
                Console.WriteLine($"buttonPress: {buttonPress} -- sendPulsesCount: {sendPulsesCount} -- hf memory: {hf.MemoryAsString()}");
                nextReportAt += reportInterval;
            }
        };

        for (var buttonPress = 1; buttonPress <= numButtonPresses; buttonPress++)
        {
            List<SendPulse> sendPulses = [new SendPulse(Pulse.Low, Broadcaster, "button")];

            while (sendPulses.Count > 0)
            {
                List<SendPulse> newSendPulses = [];

                foreach (var sendPulse in sendPulses)
                {
                    report(buttonPress, sendPulses.Count);

                    if (sendPulse.Pulse == Pulse.Low && sendPulse.DestinationModuleName == "rx")
                    {
                        return buttonPress;
                    }

                    if (sendPulse.Pulse == Pulse.Low)
                    {
                        numLowPulsesSent++;
                    }
                    else
                    {
                        numHighPulsesSent++;
                    }

                    var destinationModule = modules[sendPulse.DestinationModuleName];

                    newSendPulses.AddRange(destinationModule.ReceivePulse(sendPulse.Pulse, sendPulse.InputModuleName));
                }

                sendPulses = newSendPulses;
            }
        }

        return null;
    }

    enum Pulse
    {
        Low,
        High
    }

    record SendPulse(Pulse Pulse, string DestinationModuleName, string InputModuleName);

    abstract record Module(string Name, string[] DestinationNames, string[] InputNames)
    {
        public abstract string FullName { get; }

        public abstract IEnumerable<SendPulse> ReceivePulse(Pulse pulse, string InputModuleName);
    }

    sealed record UntypedModule(string Name, string[] InputNames) : Module(Name, Array.Empty<string>(), InputNames)
    {
        public override string FullName { get; } = Name;

        public override IEnumerable<SendPulse> ReceivePulse(Pulse pulse, string InputModuleName) => Array.Empty<SendPulse>();
    }

    sealed record FlipFlopModule(string Name, string[] DestinationNames, string[] InputNames) : Module(Name, DestinationNames, InputNames)
    {
        public override string FullName { get; } = '%' + Name;

        public bool IsOn { get; private set; } = false;

        public override IEnumerable<SendPulse> ReceivePulse(Pulse pulse, string InputModuleName)
        {
            if (pulse == Pulse.High)
            {
                return Array.Empty<SendPulse>();
            }

            IsOn = !IsOn;
            var pulseToSend = IsOn ? Pulse.High : Pulse.Low;
            return DestinationNames.Select(destinationName => new SendPulse(pulseToSend, destinationName, Name));
        }
    }

    sealed record ConjunctionModule(string Name, string[] DestinationNames, string[] InputNames) : Module(Name, DestinationNames, InputNames)
    {
        public override string FullName { get; } = '&' + Name;

        private readonly Dictionary<string, Pulse> memory = InputNames.ToDictionary(name => name, _ => Pulse.Low);

        public string MemoryAsString() => string.Join(", ", memory.Select(m => $"{m.Key}:{m.Value}"));

        public override IEnumerable<SendPulse> ReceivePulse(Pulse pulse, string InputModuleName)
        {
            if (!memory.ContainsKey(InputModuleName))
            {
                throw new Exception($"Unexpected input module {InputModuleName} for {Name}");
            }

            memory[InputModuleName] = pulse;

            var isHighPulsesForAllInputs = memory.Values.All(pulseMemory => pulseMemory == Pulse.High);
            var pulseToSend = isHighPulsesForAllInputs ? Pulse.Low : Pulse.High;
            return DestinationNames.Select(destinationName => new SendPulse(pulseToSend, destinationName, Name));
        }
    }

    sealed record BroadcastModule(string Name, string[] DestinationNames) : Module(Name, DestinationNames, Array.Empty<string>())
    {
        public override string FullName { get; } = Name;

        public override IEnumerable<SendPulse> ReceivePulse(Pulse pulse, string InputModuleName) =>
            DestinationNames.Select(destinationName => new SendPulse(pulse, destinationName, Name));
    }

    static FrozenDictionary<string, Module> ParseModuleConfiguration(string input)
    {
        var intermediates = ParseModuleConfigurationRegex().Matches(input).Select(match => new
        {
            type = match.Groups["type"].Value,
            name = match.Groups["name"].Value,
            destinations = match.Groups["destinations"].Value.Trim().Split(", "),
            destinationsHash = match.Groups["destinations"].Value.Trim().Split(", ").ToHashSet(),
        }).ToArray();

        var mainModuleNames = intermediates.Select(m => m.name);
        var destinationModuleNames = intermediates.SelectMany(m => m.destinations);
        var untypedModuleNames = destinationModuleNames.Except(mainModuleNames);
        var getInputNames = (string destinationName) => intermediates.Where(x => x.destinationsHash.Contains(destinationName)).Select(x => x.name).ToArray();

        var modules = intermediates
            .Select(item => item.type switch
            {
                "%" => (Module)new FlipFlopModule(item.name, item.destinations, getInputNames(item.name)),
                "&" => new ConjunctionModule(item.name, item.destinations, getInputNames(item.name)),
                _ when item.name == Broadcaster => new BroadcastModule(item.name, item.destinations),
                var invalidType => throw new Exception("Invalid module type: " + invalidType)
            })
            .Concat(untypedModuleNames.Select(untypedName => new UntypedModule(untypedName, getInputNames(untypedName))))
            .ToFrozenDictionary(module => module.Name, module => module);

        // Validate:

        if (modules.Values.OfType<BroadcastModule>().Single().Name != Broadcaster)
        {
            throw new Exception("Broadcaster not valid");
        }

        foreach (var conjunctionModule in modules.Values.OfType<ConjunctionModule>())
        {
            if (conjunctionModule.InputNames.Length == 0)
            {
                throw new Exception($"Conjunction module {conjunctionModule.Name} has no inputs.");
            }
        }

        foreach (var untypedModule in modules.Values.OfType<UntypedModule>())
        {
            if (untypedModule.Name.First() is '%' or '&')
            {
                throw new Exception($"Invalid untyped module {untypedModule.Name}");
            }
        }

        var missingModules = string.Join(", ", modules.Values.SelectMany(m => m.DestinationNames).Except(modules.Keys));
        if (missingModules != "")
        {
            throw new Exception($"Missing modules: {missingModules} - untyped module names are: {string.Join(", ", untypedModuleNames)}");
        }

        return modules;
    }

    [GeneratedRegex("""((?<type>[%&])?(?<name>.+)) -> (?<destinations>.+)""", RegexOptions.Compiled)]
    private static partial Regex ParseModuleConfigurationRegex();
}

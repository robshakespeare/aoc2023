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

        for (var i = 1; i <= numButtonPresses; i++)
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

    //enum ModuleType
    //{
    //    /// <summary>
    //    /// Prefix: %
    //    /// </summary>
    //    FlipFlop,

    //    /// <summary>
    //    /// Prefix: &
    //    /// </summary>
    //    Conjunction,
    //}

    enum Pulse
    {
        Low,
        High
    }

    record SendPulse(Pulse Pulse, string DestinationModuleName, string InputModuleName);

    abstract record Module(string Name, string[] DestinationNames)
    {
        public abstract IEnumerable<SendPulse> ReceivePulse(Pulse pulse, string InputModuleName);
    }

    sealed record UntypedModule(string Name) : Module(Name, Array.Empty<string>())
    {
        public override IEnumerable<SendPulse> ReceivePulse(Pulse pulse, string InputModuleName) => Array.Empty<SendPulse>();
    }

    sealed record FlipFlopModule(string Name, string[] DestinationNames) : Module(Name, DestinationNames)
    {
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

    sealed record ConjunctionModule(string Name, string[] DestinationNames, string[] InputNames) : Module(Name, DestinationNames)
    {
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

    sealed record BroadcastModule(string Name, string[] DestinationNames) : Module(Name, DestinationNames)
    {
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
        var extraModuleNames = destinationModuleNames.Except(mainModuleNames);

        var modules = intermediates
            .Select(item => item.type switch
            {
                "%" => (Module)new FlipFlopModule(item.name, item.destinations),
                "&" => new ConjunctionModule(item.name, item.destinations, intermediates.Where(x => x.destinationsHash.Contains(item.name)).Select(x => x.name).ToArray()),
                _ when item.name == Broadcaster => new BroadcastModule(item.name, item.destinations),
                var invalidType => throw new Exception("Invalid module type: " + invalidType)
            })
            .Concat(extraModuleNames.Select(extraName => new UntypedModule(extraName)))
            .ToFrozenDictionary(module => module.Name, module => module);

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
            throw new Exception($"Missing modules: {missingModules} - extra module names are: {string.Join(", ", extraModuleNames)}");
        }

        return modules;

        //foreach (var item in intermediates)
        //{
        //    Module module = item.type switch
        //    {
        //        "%" => new FlipFlopModule(item.name, item.destinations),
        //        "&" => new ConjunctionModule(item.name, item.destinations, intermediates.Where(x => x.destinationsHash.Contains(item.name)).Select(x => x.name).ToArray()),
        //        _ when item.name == "broadcaster" => new BroadcastModule(item.name, item.destinations),
        //        var invalidType => throw new Exception("Invalid module type: " + invalidType)
        //    };

        //    modules.Add(module.Name, module);
        //}
    }

    [GeneratedRegex("""((?<type>[%&])?(?<name>.+)) -> (?<destinations>.+)""", RegexOptions.Compiled)]
    private static partial Regex ParseModuleConfigurationRegex();
}

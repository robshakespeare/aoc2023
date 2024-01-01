namespace AoC.Day19;

public partial class Day19Solver : ISolver
{
    public string DayName => "Aplenty";

    public long? SolvePart1(string input)
    {
        var pipeline = Pipeline.Parse(input);

        var parts = ParsePartRegex().Matches(input).Select(match =>
            new Part(match.Groups["rating"].Captures.Zip(match.Groups["value"].Captures).ToDictionary(x => x.First.Value.Single(), x => int.Parse(x.Second.Value)))).ToArray();

        foreach (var part in parts)
        {
            pipeline.In.Process(part, pipeline.Workflows);
        }

        return pipeline.Accepted.Parts.Sum(part => part.GetTotalRating());
    }

    public long? SolvePart2(string input)
    {
        // We only care about being accepted, so mayeb we could run thorugh with "ranges", ultimately creating slices of each of the 4 ratings that would get accepted.
        // So, initially we start with:
        // x: 1 to 4000
        // m: 1 to 4000
        // a: 1 to 4000
        // s: 1 to 4000
        // Which would be 4000 * 4000 * 4000 * 4000 combinations
        // Then we pass it through the pipeline, working out each eventuality



        return null;
    }

    record Rule(char Rating, char Op, int Threshold, string Destination)
    {
        public bool IsMatch(Part part)
        {
            var value = part.Ratings[Rating];
            return Op switch
            {
                '<' => value < Threshold,
                '>' => value > Threshold,
                _ => throw new Exception("Invalid op: " + Op)
            };
        }
    }

    record Workflow(string Name, Rule[] Rules, string DefaultDestination)
    {
        public virtual void Process(Part part, FrozenDictionary<string, Workflow> workflows) =>
            workflows[Rules.FirstOrDefault(rule => rule.IsMatch(part))?.Destination ?? DefaultDestination].Process(part, workflows);
    }

    sealed record FinalWorkflow(string Name) : Workflow(Name, [], "")
    {
        public List<Part> Parts { get; } = [];

        public override void Process(Part part, FrozenDictionary<string, Workflow> workflows) => Parts.Add(part);
    }

    record Part(Dictionary<char, int> Ratings)
    {
        public long GetTotalRating() => Ratings.Values.Sum();
    }

    record Range(int Start, int End);

    record Pipeline(FrozenDictionary<string, Workflow> Workflows, FinalWorkflow Accepted, FinalWorkflow Rejected)
    {
        public Workflow In { get; } = Workflows["in"];

        public static Pipeline Parse(string input)
        {
            var accepted = new FinalWorkflow("A");
            var rejected = new FinalWorkflow("R");

            var workflows = ParseWorkflowRegex().Matches(input)
                .Select(match => new Workflow(
                    match.Groups["name"].Value,
                    match.Groups["rating"].Captures.Zip(match.Groups["op"].Captures, match.Groups["threshold"].Captures).Zip(match.Groups["destination"].Captures)
                        .Select(x => new Rule(
                            Rating: x.First.First.Value.Single(),
                            Op: x.First.Second.Value.Single(),
                            Threshold: int.Parse(x.First.Third.Value),
                            Destination: x.Second.Value))
                        .ToArray(),
                    match.Groups["defaultDestination"].Value))
                .Concat([accepted, rejected])
                .ToFrozenDictionary(x => x.Name, x => x);

            var inflow = workflows["in"];

            return new Pipeline(workflows, accepted, rejected);
        }
    }

    [GeneratedRegex("""(?<name>\w+){(?:(?<rating>\w)(?<op>[<>])(?<threshold>\d+):(?<destination>\w+),)+(?<defaultDestination>\w+)}""")]
    private static partial Regex ParseWorkflowRegex();

    [GeneratedRegex("""{(?:(?<rating>\w)=(?<value>\d+),?)+}""")]
    private static partial Regex ParsePartRegex();
}

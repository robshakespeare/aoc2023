namespace AoC.Day19;

public partial class Day19Solver : ISolver
{
    public string DayName => "Aplenty";

    public long? SolvePart1(string input)
    {
        var accepted = new FinalWorkflow("A");
        var rejected = new FinalWorkflow("R");

        var workflows = ParseWorkflowRegex().Matches(input).Select(match => new Workflow(
            match.Groups["name"].Value,
            match.Groups["rating"].Captures.Zip(match.Groups["op"].Captures, match.Groups["threshold"].Captures).Zip(match.Groups["destination"].Captures)
                .Select(x => Rule.Create(
                    rating: x.First.First.Value.Single(),
                    op: x.First.Second.Value.Single(),
                    threshold: int.Parse(x.First.Third.Value),
                    destination: x.Second.Value))
                .ToArray(),
            match.Groups["defaultDestination"].Value))
            .Concat([accepted, rejected])
            .ToFrozenDictionary(x => x.Name, x => x);

        var inflow = workflows["in"];

        var parts = ParsePartRegex().Matches(input).Select(match =>
            new Part(match.Groups["rating"].Captures.Zip(match.Groups["value"].Captures).ToDictionary(x => x.First.Value.Single(), x => int.Parse(x.Second.Value)))).ToArray();

        foreach (var part in parts)
        {
            inflow.Process(part, workflows);
        }

        //workflows.Dump();
        //parts.Dump();

        return accepted.Parts.Sum(part => part.GetTotalRating());
    }

    public long? SolvePart2(string input)
    {
        return null;
    }

    record Rule(char Rating, string Destination, Func<int, bool> Delegate)
    {
        public bool IsMatch(Part part) => Delegate(part.Ratings[Rating]);

        public static Rule Create(char rating, char op, int threshold, string destination) => new(rating, destination, op switch
        {
            '<' => value => value < threshold,
            '>' => value => value > threshold,
            _ => throw new Exception("Invalid op: " + op)
        });
    }

    record Workflow(string Name, Rule[] Rules, string DefaultDestination)
    {
        public virtual void Process(Part part, FrozenDictionary<string, Workflow> workflows) =>
            workflows[Rules.FirstOrDefault(rule => rule.IsMatch(part))?.Destination ?? DefaultDestination].Process(part, workflows);
    }

    record FinalWorkflow(string Name) : Workflow(Name, [], "")
    {
        public List<Part> Parts { get; } = [];

        public override void Process(Part part, FrozenDictionary<string, Workflow> workflows) => Parts.Add(part);
    }

    record Part(Dictionary<char, int> Ratings)
    {
        public long GetTotalRating() => Ratings.Values.Sum();
    }

    //[GeneratedRegex("""(?<name>\w+){(?<rule>\w(?<op>[<>])(?<threshold>\d+):(?<destination>\w+),)+(?<defaultDestination>\w+)}""", RegexOptions.Compiled)]
    //private static partial Regex ParseWorkflowRegex();

    [GeneratedRegex("""(?<name>\w+){(?:(?<rating>\w)(?<op>[<>])(?<threshold>\d+):(?<destination>\w+),)+(?<defaultDestination>\w+)}""")]
    private static partial Regex ParseWorkflowRegex();

    [GeneratedRegex("""{(?:(?<rating>\w)=(?<value>\d+),?)+}""")]
    private static partial Regex ParsePartRegex();
}

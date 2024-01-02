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
        // We only care about being accepted, so we run through with "ranges", ultimately creating slices of each of the 4 ratings that would get accepted.
        // So, initially we start with:
        // x: 1 to 4000
        // m: 1 to 4000
        // a: 1 to 4000
        // s: 1 to 4000
        // Which would be 4000 * 4000 * 4000 * 4000 combinations
        // Then we pass it through the pipeline, working out each eventuality

        var pipeline = Pipeline.Parse(input);

        var initialPartRatingRange = new PartRatingRange(new Dictionary<char, Range>()
        {
            { 'x', new Range(1, 4000) },
            { 'm', new Range(1, 4000) },
            { 'a', new Range(1, 4000) },
            { 's', new Range(1, 4000) }
        });

        pipeline.In.Process(initialPartRatingRange, pipeline.Workflows);

        //foreach (var partRatingRange in pipeline.Accepted.PartRatingRanges)
        //{
        //    Console.WriteLine($"{partRatingRange.Ratings} :: {partRatingRange.GetNumOfCombinations()}");
        //}

        return pipeline.Accepted.PartRatingRanges.Sum(x => x.GetNumOfCombinations());
    }

    interface IRule
    {
        bool IsMatch(Part part);
        (PartRatingRange? MatchedRange, PartRatingRange? RemainingRange) SplitRange(PartRatingRange partRatingRange);
        string Destination { get; }
    }

    record DefaultRule(string Destination) : IRule
    {
        public bool IsMatch(Part part) => true;

        public (PartRatingRange? MatchedRange, PartRatingRange? RemainingRange) SplitRange(PartRatingRange partRatingRange) => (partRatingRange, null);
    }

    record Rule(char Rating, char Op, int Threshold, string Destination) : IRule
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

        public (PartRatingRange? MatchedRange, PartRatingRange? RemainingRange) SplitRange(PartRatingRange partRatingRange)
        {
            var range = partRatingRange.Ratings[Rating];

            var (matchedRange, remainingRange) = Op switch
            {
                '<' => (new Range(range.Start, Threshold - 1), new Range(Threshold, range.End)),
                '>' => (new Range(Threshold + 1, range.End), new Range(range.Start, Threshold)),
                _ => throw new Exception("Invalid op: " + Op)
            };

            PartRatingRange? BuildPartRatingRange(Range newRange) =>
                newRange.Size > 0
                    ? new PartRatingRange(new Dictionary<char, Range>(partRatingRange.Ratings) { [Rating] = newRange })
                    : null;

            return (BuildPartRatingRange(matchedRange), BuildPartRatingRange(remainingRange));
        }
    }

    interface IWorkflow
    {
        string Name { get; }
        void Process(Part part, FrozenDictionary<string, IWorkflow> workflows);
        void Process(PartRatingRange partRatingRange, FrozenDictionary<string, IWorkflow> workflows);
    }

    sealed record Workflow(string Name, IRule[] Rules) : IWorkflow
    {
        public void Process(Part part, FrozenDictionary<string, IWorkflow> workflows) =>
            workflows[Rules.First(rule => rule.IsMatch(part)).Destination].Process(part, workflows);

        public void Process(PartRatingRange partRatingRange, FrozenDictionary<string, IWorkflow> workflows)
        {
            // Go through each rule, "splitting" the "range" based on the rule, the remaining gets passed on to the next, and the final remainder gets passed on to the default
            foreach (var rule in Rules)
            {
                var (matchedRange, remainingRange) = rule.SplitRange(partRatingRange);

                if (matchedRange != null)
                {
                    workflows[rule.Destination].Process(matchedRange, workflows);
                }

                if (remainingRange == null)
                {
                    return;
                }

                partRatingRange = remainingRange;
            }
        }
    }

    sealed record FinalWorkflow(string Name) : IWorkflow
    {
        public List<Part> Parts { get; } = [];

        public List<PartRatingRange> PartRatingRanges { get; } = [];

        public void Process(Part part, FrozenDictionary<string, IWorkflow> workflows) => Parts.Add(part);

        public void Process(PartRatingRange partRatingRange, FrozenDictionary<string, IWorkflow> workflows) => PartRatingRanges.Add(partRatingRange);
    }

    record Part(IReadOnlyDictionary<char, int> Ratings)
    {
        public long GetTotalRating() => Ratings.Values.Sum();
    }

    record PartRatingRange(IReadOnlyDictionary<char, Range> Ratings)
    {
        public long GetNumOfCombinations() => Ratings.Values.Select(x => x.Size).Aggregate((acc, cur) => acc * cur);
    }

    record Range(int Start, int End)
    {
        public long Size { get; } = End - (Start - 1L);
    }

    record Pipeline(FrozenDictionary<string, IWorkflow> Workflows, FinalWorkflow Accepted, FinalWorkflow Rejected)
    {
        public IWorkflow In { get; } = Workflows["in"];

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
                        .Append<IRule>(new DefaultRule(match.Groups["defaultDestination"].Value))
                        .ToArray()))
                .Concat<IWorkflow>([accepted, rejected])
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

namespace AoC.Day06;

public partial class Day6Solver : ISolver
{
    public string DayName => "Wait For It";

    public long? SolvePart1(string input) => ParseInput(input).Select(x => x.GetWaysToWin().Length).Aggregate((agg, cur) => agg * cur);

    public long? SolvePart2(string input)
    {
        input = FixKerningRegex().Replace(input, match => match.Groups["keep"].Value);

        var race = ParseInput(input).Single();

        Console.WriteLine(race);

        return null;
    }

    record Race(long RaceDuration, long RecordDistance)
    {
        public RaceResult[] GetWaysToWin() => new Range(1, (int)RaceDuration).ToEnumerable()
            .Select(speed => new RaceResult(speed, RaceDuration - speed))
            .Where(result => result.DistanceTravelled > RecordDistance)
            .ToArray();
    }

    record RaceResult(long Speed, long RemainingDuration)
    {
        public long DistanceTravelled { get; } = Speed * RemainingDuration;
    }

    static Race[] ParseInput(string input)
    {
        var lines = input.ReadLines().Select(line => line.Split(" ", StringSplitOptions.RemoveEmptyEntries).Skip(1).Select(long.Parse)).ToArray();

        return lines[0].Zip(lines[1]).Select(x => new Race(x.First, x.Second)).ToArray();
    }

    [GeneratedRegex(@"(?<keep>\d+)[ ]+")]
    private static partial Regex FixKerningRegex();
}

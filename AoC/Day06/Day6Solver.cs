namespace AoC.Day06;

public class Day6Solver : ISolver
{
    public string DayName => "Wait For It";

    public long? SolvePart1(string input)
    {
        var races = ParseInput(input);

        //Console.WriteLine();

        return ParseInput(input).Select(x => x.GetWaysToWin().Length).Aggregate((agg, cur) => agg * cur);

        return null;
    }

    public long? SolvePart2(string input)
    {
        return null;
    }

    record Race(int RaceDuration, int RecordDistance)
    {
        public RaceResult[] GetWaysToWin() => new Range(1, RaceDuration).ToEnumerable()
            .Select(speed => new RaceResult(speed, RaceDuration - speed)) //, this))
            .Where(result => result.DistanceTravelled > RecordDistance)
            .ToArray();
    }

    record RaceResult(int Speed, int RemainingDuration) //, Race Race)
    {
        public long DistanceTravelled { get; } = (long)Speed * RemainingDuration;
    }

    static Race[] ParseInput(string input)
    {
        var lines = input.ReadLines().Select(line => line.Split(" ", StringSplitOptions.RemoveEmptyEntries).Skip(1).Select(int.Parse)).ToArray();

        return lines[0].Zip(lines[1]).Select(x => new Race(x.First, x.Second)).ToArray();
    }
}

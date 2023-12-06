namespace AoC.Day06;

public partial class Day6Solver : ISolver
{
    public string DayName => "Wait For It";

    public long? SolvePart1(string input) => ParseInput(input).Select(race => race.CalculateNumOfWaysToWin()).Aggregate((agg, cur) => agg * cur);

    public long? SolvePart1NotOptimised(string input) => ParseInput(input).Select(race => race.EnumerateWaysToWin().Count()).Aggregate((agg, cur) => agg * cur);

    public long? SolvePart2(string input)
    {
        input = FixKerningRegex().Replace(input, match => match.Groups["keep"].Value);
        var race = ParseInput(input).Single();
        return race.CalculateNumOfWaysToWin();
    }

    record Race(long RaceDuration, long RecordDistance)
    {
        public IEnumerable<RaceResult> EnumerateWaysToWin() => SpeedRange()
            .Select(speed => new RaceResult(speed, RaceDuration - speed))
            .Where(result => result.DistanceTravelled > RecordDistance);

        private IEnumerable<long> SpeedRange()
        {
            for (long speed = 1; speed < RaceDuration; speed++)
                yield return speed;
        }

        public long CalculateNumOfWaysToWin()
        {
            var minSpeed = FindLowestSpeedToBeatRecord();
            var maxSpeed = RaceDuration - minSpeed;
            return maxSpeed - minSpeed + 1;
        }

        public long FindLowestSpeedToBeatRecord()
        {
            var startSpeed = 1L;
            var endSpeed = RaceDuration - 1;

            while (startSpeed != endSpeed)
            {
                var midSpeed = (startSpeed + endSpeed) / 2;
                var distance = midSpeed * (RaceDuration - midSpeed);

                if (distance <= RecordDistance)
                {
                    startSpeed = midSpeed + 1; // our distance doesn't beat record, so we need more speed
                }
                else if (distance > RecordDistance)
                {
                    endSpeed = midSpeed; // our distance beats record, but can we go lower?
                }
            }

            return startSpeed;
        }
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

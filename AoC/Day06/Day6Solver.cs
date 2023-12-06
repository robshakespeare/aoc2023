namespace AoC.Day06;

public partial class Day6Solver : ISolver
{
    public string DayName => "Wait For It";

    public long? SolvePart1(string input) => ParseInput(input).Select(x => x.CountGetWaysToWinOptimised()).Aggregate((agg, cur) => agg * cur);

    public long? SolvePart2(string input)
    {
        //{
        //    var races = ParseInput(input);

        //    foreach (var race2 in races)
        //    {
        //        Console.WriteLine(race2);
        //        Console.WriteLine(Race.FindLowest(race2));
        //        //Console.WriteLine(Race.FindHighest(race2));
        //        Console.WriteLine(string.Join(Environment.NewLine, race2.GetWaysToWin().Select(x => x.ToString())));
        //    }
        //}

        input = FixKerningRegex().Replace(input, match => match.Groups["keep"].Value);

        var race = ParseInput(input).Single();

        //Console.WriteLine(race);
        ////Console.WriteLine(Race.FindLowest(race));

        //// So, thinking can use binary search again!!

        //var (raceDuration, recordDistance) = race;

        //var minSpeed = 1L;
        //var maxSpeed = raceDuration - 1;

        //// Find the lowest
        ////{
        ////    var start = minSpeed;
        ////    var end = maxSpeed;

        ////    while (something)
        ////    {
        ////        var midSpeed = (minSpeed + maxSpeed) / 2;

        ////        // DistanceTravelled = Speed * (RaceDuration - Speed)
        ////        var distance = midSpeed * (raceDuration - midSpeed);

        ////        if (distance <= recordDistance)
        ////        {
        ////            // our distance doesn't beat record, so we need more speed
        ////            start = midSpeed;
        ////        }
        ////        else if (distance > recordDistance)
        ////        {
        ////            // our distance beats record, but can we go lower?
        ////            end = midSpeed;
        ////        }
        ////    }
        ////}

        //// Find the highest

        return race.CountGetWaysToWinOptimised();
    }

    record Race(long RaceDuration, long RecordDistance)
    {
        public IEnumerable<RaceResult> GetWaysToWin() => Range()
            .Select(speed => new RaceResult(speed, RaceDuration - speed))
            .Where(result => result.DistanceTravelled > RecordDistance);

        public long CountGetWaysToWin() => Range()
            .Select(speed => speed * (RaceDuration - speed))
            .LongCount(distanceTravelled => distanceTravelled > RecordDistance);

        public long CountGetWaysToWinOptimised()
        {
            var lowest = FindLowest(this);
            var highest = RaceDuration - lowest;

            var numOfWins = highest - lowest + 1;

            return numOfWins;
        }

        private IEnumerable<long> Range()
        {
            long value = FindLowest(this);

            while (value < RaceDuration)
            {
                yield return value;
                value++;
            }
        }

        public static long FindLowest(Race race)
        {
            //var minSpeed = 1L;
            //var maxSpeed = RaceDuration - 1;

            var (raceDuration, recordDistance) = race;

            var start = 1L;
            var end = raceDuration - 1;

            while (start != end) // && start < end)
            {
                var midSpeed = (start + end) / 2;

                // DistanceTravelled = Speed * (RaceDuration - Speed)
                var distance = midSpeed * (raceDuration - midSpeed);

                if (distance <= recordDistance)
                {
                    // our distance doesn't beat record, so we need more speed
                    start = midSpeed + 1;
                }
                else if (distance > recordDistance)
                {
                    // our distance beats record, but can we go lower?
                    end = midSpeed;
                }
            }

            return start;
        }

        public static long FindHighest(Race race)
        {
            //var minSpeed = 1L;
            //var maxSpeed = RaceDuration - 1;

            var (raceDuration, _) = race;

            var currentBestDistance = race.RecordDistance;
            long? currentBestSpeed = null;

            var start = 1L;
            var end = raceDuration - 1;

            while (start != end) // && start < end)
            {
                var midSpeed = (start + end) / 2;

                // DistanceTravelled = Speed * (RaceDuration - Speed)
                var distance = midSpeed * (raceDuration - midSpeed);

                if (distance <= currentBestDistance)
                {
                    // our distance doesn't beat record, so we need more speed
                    start = midSpeed + 1;
                }
                else if (distance > currentBestDistance)
                {
                    currentBestSpeed = midSpeed;
                    currentBestDistance = distance;

                    // our distance beats record, but can we go higher?
                    start = midSpeed + 1;

                    //if ()
                }
            }

            return currentBestSpeed ?? -1;
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

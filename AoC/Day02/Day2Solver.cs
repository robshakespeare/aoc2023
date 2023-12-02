namespace AoC.Day02;

public partial class Day2Solver : ISolver
{
    public string DayName => "Cube Conundrum";

    public long? SolvePart1(string input) => ParseInput(input).Where(game => game.IsPossible).Sum(game => game.GameId);

    public long? SolvePart2(string input)
    {
        Console.WriteLine(ParseInput(input).Dump());

        return ParseInput(input).Sum(game => game.MinSetOfCubes.Power);
    }

    //record Game(long GameId, Dictionary<string, int>[] SetsOfCubes)
    //{
    //    public bool IsPossible { get; } = SetsOfCubes.All(set =>
    //        set.All(cube => cube.));
    //}

    record Game(long GameId, SetOfCubes[] SetsOfCubes)
    {
        public bool IsPossible { get; } = SetsOfCubes.All(x => x.IsPossible);

        public SetOfCubes MinSetOfCubes { get; } = new SetOfCubes(
            new[] { "red", "green", "blue" }
                .Select(color => new CubeCount(color, SetsOfCubes.Max(set => set.GetCountForColor(color)))).ToArray());
    }

    record SetOfCubes(CubeCount[] CubeCounts)
    {
        public bool IsPossible { get; } = CubeCounts.All(x => x.IsPossible);

        public long Power { get; } = CubeCounts.Aggregate(1L, (agg, cur) => agg * cur.Count);

        public int GetCountForColor(string color) => CubeCounts.FirstOrDefault(x => x.Color == color)?.Count ?? 0;
    }

    record CubeCount(string Color, int Count)
    {
        public bool IsPossible { get; } = Count <= MaxCountPerColor[Color];
    }

    static readonly Dictionary<string, int> MaxCountPerColor = new()
    {
        { "red", 12 },
        { "green", 13 },
        { "blue", 14 },
    };

    //Game[] ParseInput(string input)
    //{
    //    return ParseGamesRegex().Matches(input).Select(match =>
    //    {
    //        var gameId = int.Parse(match.Groups["gameId"].Value);

    //        var setsOfCubes = match.Groups["setsOfCubes"].Value.Split("; ").Select(
    //            set => set.Split(", ").Select(cubes =>
    //            {
    //                var pair = cubes.Split(" ");
    //                return new { count = int.Parse(pair[0]), color = pair[1] };
    //            }).ToDictionary(x => x.color, x => x.count))
    //        .ToArray();

    //        return new Game(gameId, setsOfCubes);
    //    }).ToArray();
    //}

    Game[] ParseInput(string input)
    {
        return ParseGamesRegex().Matches(input).Select(match =>
        {
            var gameId = int.Parse(match.Groups["gameId"].Value);

            var setsOfCubes = match.Groups["setsOfCubes"].Value.Split("; ").Select(
                set => new SetOfCubes(set.Split(", ").Select(cubes =>
                {
                    var pair = cubes.Split(" ");
                    return new CubeCount(Count: int.Parse(pair[0]), Color: pair[1].Trim());
                }).ToArray()))
            .ToArray();

            return new Game(gameId, setsOfCubes);
        }).ToArray();
    }

    [GeneratedRegex(@"Game (?<gameId>\d+): (?<setsOfCubes>.+)")]
    private static partial Regex ParseGamesRegex();
}

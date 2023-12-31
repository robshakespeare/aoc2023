namespace AoC.Day02;

public partial class Day2Solver : ISolver
{
    public string DayName => "Cube Conundrum";

    public long? SolvePart1(string input) => ParseInput(input).Where(game => game.IsPossible).Sum(game => game.GameId);

    public long? SolvePart2(string input) => ParseInput(input).Sum(game => game.MinSetOfCubes.Power);

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
        public bool IsPossible { get; } = Count <= Color switch
        {
            "red" => 12,
            "green" => 13,
            "blue" => 14,
            _ => throw new InvalidOperationException("Invalid color: " + Color),
        };
    }

    Game[] ParseInput(string input) => ParseGamesRegex().Matches(input).Select(match => new Game(
        GameId: int.Parse(match.Groups["gameId"].Value),
        SetsOfCubes: match.Groups["setsOfCubes"].Value.Split("; ")
            .Select(set => new SetOfCubes(set.Split(", ").Select(cubes => cubes.Split(" "))
                .Select(pair => new CubeCount(Count: int.Parse(pair[0]), Color: pair[1].Trim())).ToArray())).ToArray())).ToArray();

    [GeneratedRegex(@"Game (?<gameId>\d+): (?<setsOfCubes>.+)")]
    private static partial Regex ParseGamesRegex();
}

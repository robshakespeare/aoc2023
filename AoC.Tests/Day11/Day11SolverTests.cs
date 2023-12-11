using AoC.Day11;

namespace AoC.Tests.Day11;

public class Day11SolverTests
{
    private readonly Day11Solver _sut = new();

    private const string ExampleInput = """
        ...#......
        .......#..
        #.........
        ..........
        ......#...
        .#........
        .........#
        ..........
        .......#..
        #...#.....
        """;

    [Test]
    public void Part1ExampleWalkthrough()
    {
        // ACT & ASSERT - Expand
        var expandedUniverse = Day11Solver.ParseAndExpandInput(ExampleInput);
        expandedUniverse.Should().BeEquivalentTo("""
            ....#........
            .........#...
            #............
            .............
            .............
            ........#....
            .#...........
            ............#
            .............
            .............
            .........#...
            #....#.......
            """.ReadLines(), opts => opts.WithStrictOrdering());

        // ACT & ASSERT - Galaxy IDs
        var universe = Day11Solver.ParseUniverse(expandedUniverse);

        universe.Galaxies.Select(g => g.Id).Min().Should().Be(1);
        universe.Galaxies.Select(g => g.Id).Max().Should().Be(9);

        // ACT & ASSERT - Galaxy Pairs
        universe.GalaxyPairs.Should().HaveCount(36);

        // ACT & ASSERT - Distances
        Day11Solver.GalaxyPair GetGalaxyPair(int idA, int idB) => universe.GalaxyPairs.First(p => p.GalaxyA.Id == idA && p.GalaxyB.Id == idB);

        GetGalaxyPair(5, 9).Distance.Should().Be(9);
        GetGalaxyPair(1, 7).Distance.Should().Be(15);
        GetGalaxyPair(3, 6).Distance.Should().Be(17);
        GetGalaxyPair(8, 9).Distance.Should().Be(5);
    }

    [Test]
    public void Part1Example()
    {
        // ACT
        var part1ExampleResult = _sut.SolvePart1(ExampleInput);

        // ASSERT
        part1ExampleResult.Should().Be(374);
    }

    [Test]
    public void Part1ReTest()
    {
        // ACT
        var part1Result = _sut.SolvePart1();

        // ASSERT
        part1Result.Should().BeGreaterThan(9606388);
        part1Result.Should().Be(null);
    }

    [Test]
    public void Part2Example()
    {
        // ACT
        var part2ExampleResult = _sut.SolvePart2(ExampleInput);

        // ASSERT
        part2ExampleResult.Should().Be(null);
    }

    [Test]
    public void Part2ReTest()
    {
        // ACT
        var part2Result = _sut.SolvePart2();

        // ASSERT
        part2Result.Should().Be(null);
    }
}

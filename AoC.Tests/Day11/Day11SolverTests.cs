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
        var universe = Day11Solver.ParseAndExpandUniverse(ExampleInput);
        var expandedUniverse = universe.Galaxies.ToStringGrid(g => g.Position, _ => '#', '.');
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
        universe.Galaxies.Select(g => g.Id).Min().Should().Be(1);
        universe.Galaxies.Select(g => g.Id).Max().Should().Be(9);

        // ACT & ASSERT - Galaxy Pairs
        var galaxyPairs = universe.GetGalaxyPairs();
        galaxyPairs.Should().HaveCount(36);

        // ACT & ASSERT - Distances
        Day11Solver.GalaxyPair GetGalaxyPair(int idA, int idB) => galaxyPairs.First(p => p.GalaxyA.Id == idA && p.GalaxyB.Id == idB);

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
        part1Result.Should().BeGreaterThan(9067847);
        part1Result.Should().Be(9684228);
    }

    [Test]
    public void Part2Example()
    {
        Day11Solver.ParseExpandAndSumDistances(ExampleInput).Should().Be(374);
        Day11Solver.ParseExpandAndSumDistances(ExampleInput, 10).Should().Be(1030);
        Day11Solver.ParseExpandAndSumDistances(ExampleInput, 100).Should().Be(8410);
    }

    [Test]
    public void Part2ReTest()
    {
        // ACT
        var part2Result = _sut.SolvePart2();

        // ASSERT
        part2Result.Should().BeLessThan(483845200392);
        part2Result.Should().Be(null);
    }
}

using AoC.Day23;

namespace AoC.Tests.Day23;

public class Day23SolverTests
{
    private readonly Day23Solver _sut = new();

    private const string ExampleInput = """
        #.#####################
        #.......#########...###
        #######.#########.#.###
        ###.....#.>.>.###.#.###
        ###v#####.#v#.###.#.###
        ###.>...#.#.#.....#...#
        ###v###.#.#.#########.#
        ###...#.#.#.......#...#
        #####.#.#.#######.#.###
        #.....#.#.#.......#...#
        #.#####.#.#.#########v#
        #.#...#...#...###...>.#
        #.#.#v#######v###.###v#
        #...#.>.#...>.>.#.###.#
        #####v#.#.###v#.#.###.#
        #.....#...#...#.#.#...#
        #.#########.###.#.#.###
        #...###...#...#...#.###
        ###.###.#.###v#####v###
        #...#...#.#.>.>.#.>.###
        #.###.###.#.###.#.#v###
        #.....###...###...#...#
        #####################.#
        """;

    [Test]
    public void Part1Example_ShouldHaveExpectedNumberOfNodesInGraph_and_ExpectedNumberOfEdges()
    {
        // ACT
        var result = Day23Solver.ParseInputAndBuildGraph(ExampleInput);

        // ASSERT
        using var _ = new AssertionScope();
        result.Nodes.Should().HaveCount(7 + 2);
        result.Nodes.Select(x => x.Edges.Count).Should().BeEquivalentTo(new[]
        {
            1,
            2,
            2,
            2,
            2,
            1,
            1,
            1,
            0
        });
        result.Nodes.SelectMany(node => node.Edges).Should().HaveCount(12);

        static string EdgeTextId(Edge edge) => $"Len: {edge.Length} // {edge.Start.Position}|{edge.End.Position} // {string.Join(":", edge.Path)}";
        result.Nodes.SelectMany(node => node.Edges).DistinctBy(EdgeTextId).Should().HaveCount(12);

        result.Nodes.SelectMany(node => node.Edges).DistinctBy(edge => edge.Id).Should().HaveCount(12);
    }

    [Test]
    public void Part1Example()
    {
        // ACT
        var part1ExampleResult = _sut.SolvePart1(ExampleInput);

        // ASSERT
        part1ExampleResult.Should().Be(94);
    }

    [Test]
    public void Part1ReTest()
    {
        // ACT
        var part1Result = _sut.SolvePart1();

        // ASSERT
        part1Result.Should().Be(2306);
    }

    [Test]
    public void Part2Example()
    {
        // ACT
        var part2ExampleResult = _sut.SolvePart2(ExampleInput);

        // ASSERT
        part2ExampleResult.Should().Be(154);
    }

    [Test]
    [Ignore("Verrrrrrrrrry long running")]
    public void Part2ReTest()
    {
        // ACT
        var part2Result = _sut.SolvePart2();

        // ASSERT
        part2Result.Should().Be(null);
    }
}

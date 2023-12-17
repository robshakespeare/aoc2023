using AoC.Day17;

namespace AoC.Tests.Day17;

public class Day17SolverTests
{
    private readonly Day17Solver _sut = new();

    private const string ExampleInput = """
        2413432311323
        3215453535623
        3255245654254
        3446585845452
        4546657867536
        1438598798454
        4457876987766
        3637877979653
        4654967986887
        4564679986453
        1224686865563
        2546548887735
        4322674655533
        """;

    [Test]
    public void Part1Example()
    {
        // ACT
        var part1ExampleResult = _sut.SolvePart1(ExampleInput);

        // ASSERT
        part1ExampleResult.Should().Be(102);
    }

    [Test]
    [LongRunningTest("~1 second")]
    public void Part1ReTest()
    {
        // ACT
        var part1Result = _sut.SolvePart1();

        // ASSERT
        part1Result.Should().Be(886);
    }

    [Test]
    public void Part2Example()
    {
        // ACT
        var part2ExampleResult = _sut.SolvePart2(ExampleInput);

        // ASSERT
        part2ExampleResult.Should().Be(94);
    }

    [Test]
    public void Part2Example2()
    {
        // ACT
        var part2ExampleResult = _sut.SolvePart2("""
            111111111111
            999999999991
            999999999991
            999999999991
            999999999991
            """);

        // ASSERT
        part2ExampleResult.Should().Be(71);
    }

    [Test]
    [LongRunningTest("~3 seconds")]
    public void Part2ReTest()
    {
        // ACT
        var part2Result = _sut.SolvePart2();

        // ASSERT
        part2Result.Should().Be(1055);
    }
}

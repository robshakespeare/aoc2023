using AoC.Day21;

namespace AoC.Tests.Day21;

public class Day21SolverTests
{
    private readonly Day21Solver _sut = new();

    private const string ExampleInput = """
        ...........
        .....###.#.
        .###.##..#.
        ..#.#...#..
        ....#.#....
        .##..S####.
        .##..#...#.
        .......##..
        .##.#.####.
        .##..##.##.
        ...........
        """;

    [Test]
    public void Part1Example()
    {
        using var _ = new AssertionScope();

        // ACT & ASSERT
        _sut.SolvePart1(ExampleInput, 0).Should().Be(1);
        _sut.SolvePart1(ExampleInput, 1).Should().Be(2);
        _sut.SolvePart1(ExampleInput, 2).Should().Be(4);
        _sut.SolvePart1(ExampleInput, 3).Should().Be(6);
        _sut.SolvePart1(ExampleInput, 6).Should().Be(16);
    }

    [Test]
    public void Part1ReTest()
    {
        // ACT
        var part1Result = _sut.SolvePart1();

        // ASSERT
        part1Result.Should().Be(3748);
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

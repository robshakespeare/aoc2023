using AoC.Day06;

namespace AoC.Tests.Day06;

public class Day6SolverTests
{
    private readonly Day6Solver _sut = new();

    private const string ExampleInput = """
        Time:      7  15   30
        Distance:  9  40  200
        """;

    [Test]
    public void Part1Example()
    {
        // ACT
        var part1ExampleResult = _sut.SolvePart1(ExampleInput);
        var part1ExampleResultNotOptimised = _sut.SolvePart1NotOptimised(ExampleInput);

        // ASSERT
        const int expectedResult = 288;
        part1ExampleResult.Should().Be(expectedResult);
        part1ExampleResultNotOptimised.Should().Be(expectedResult);
    }

    [Test]
    public void Part1ReTest()
    {
        // ACT
        var part1Result = _sut.SolvePart1();
        var part1ResultNotOptimised = _sut.SolvePart1NotOptimised(_sut.GetInputLoader().PuzzleInputPart1);

        // ASSERT
        const int expectedResult = 275724;
        part1Result.Should().Be(expectedResult);
        part1ResultNotOptimised.Should().Be(expectedResult);
    }

    [Test]
    public void Part2Example()
    {
        // ACT
        var part2ExampleResult = _sut.SolvePart2(ExampleInput);

        // ASSERT
        part2ExampleResult.Should().Be(71503);
    }

    [Test]
    public void Part2ReTest()
    {
        // ACT
        var part2Result = _sut.SolvePart2();

        // ASSERT
        part2Result.Should().Be(37286485);
    }
}

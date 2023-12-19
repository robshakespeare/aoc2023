using AoC.Day18;

namespace AoC.Tests.Day18;

public class Day18SolverTests
{
    private readonly Day18Solver _sut = new();

    private const string ExampleInput = """
        R 6 (#70c710)
        D 5 (#0dc571)
        L 2 (#5713f0)
        D 2 (#d2c081)
        R 2 (#59c680)
        D 2 (#411b91)
        L 5 (#8ceee2)
        U 2 (#caa173)
        L 1 (#1b58a2)
        U 2 (#caa171)
        R 2 (#7807d2)
        U 3 (#a77fa3)
        L 2 (#015232)
        U 2 (#7a21e3)
        """;

    [Test]
    public void Part1Example()
    {
        const int expectedResult = 62;

        // ACT
        var part1ExampleResult = _sut.SolvePart1(ExampleInput);

        // ASSERT
        using var _ = new AssertionScope();
        part1ExampleResult.Should().Be(expectedResult);
        Day18SolverPart1Original.SolvePart1(ExampleInput).Should().Be(expectedResult);
    }

    [Test]
    public void Part1MyExample1()
    {
        const string input = """
            R 3
            D 3
            L 3
            U 3
            """;
        const int expectedResult = 16;

        // ACT
        var part1ExampleResult = _sut.SolvePart1(input);

        // ASSERT
        using var _ = new AssertionScope();
        part1ExampleResult.Should().Be(expectedResult);
        Day18SolverPart1Original.SolvePart1(input).Should().Be(expectedResult);
    }

    [Test]
    public void Part1MyExample2()
    {
        const string input = """
            R 3
            D 3
            L 2
            D 2
            L 1
            U 5
            """;
        const int expectedResult = 20;

        // ACT
        var part1ExampleResult = _sut.SolvePart1(input);

        // ASSERT
        using var _ = new AssertionScope();
        part1ExampleResult.Should().Be(expectedResult);
        Day18SolverPart1Original.SolvePart1(input).Should().Be(expectedResult);
    }

    [Test]
    public void Part1MyExample3()
    {
        const string input = """
            R 4
            D 4
            L 2
            D 2
            R 1
            D 2
            L 10
            U 4
            R 2
            D 2
            R 5
            U 6
            """;
        const int expectedResult = 67;

        // ACT
        var part1ExampleResult = _sut.SolvePart1(input);

        // ASSERT
        using var _ = new AssertionScope();
        part1ExampleResult.Should().Be(expectedResult);
        Day18SolverPart1Original.SolvePart1(input).Should().Be(expectedResult);
    }

    [Test]
    public void Part1MyExample4()
    {
        const string input = """
            R 4
            D 3
            R 5
            D 2
            R 3
            D 4
            L 5
            U 2
            L 2
            D 4
            L 2
            U 6
            L 3
            U 5
            """;
        const int expectedResult = 92;

        // ACT
        var part1ExampleResult = _sut.SolvePart1(input);

        // ASSERT
        using var _ = new AssertionScope();
        part1ExampleResult.Should().Be(expectedResult);
        Day18SolverPart1Original.SolvePart1(input).Should().Be(expectedResult);
    }

    [Test]
    public void Part1ReTest()
    {
        const int expectedResult = 108909;

        // ACT
        var part1Result = _sut.SolvePart1();

        // ASSERT
        using var _ = new AssertionScope();
        part1Result.Should().Be(expectedResult);
        Day18SolverPart1Original.SolvePart1(_sut.GetInputLoader().PuzzleInputPart1).Should().Be(expectedResult);
    }

    [Test]
    public void Part2Example()
    {
        // ACT
        var part2ExampleResult = _sut.SolvePart2(ExampleInput);

        // ASSERT
        part2ExampleResult.Should().Be(952408144115);
    }

    [Test]
    public void Part2ReTest()
    {
        // ACT
        var part2Result = _sut.SolvePart2();

        // ASSERT
        part2Result.Should().Be(133125706867777);
    }
}

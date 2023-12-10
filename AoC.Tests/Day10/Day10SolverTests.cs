using AoC.Day10;

namespace AoC.Tests.Day10;

public class Day10SolverTests
{
    private readonly Day10Solver _sut = new();

    [Test]
    public void Part1Example1()
    {
        // ACT
        var part1Example1Result = _sut.SolvePart1("""
            -L|F7
            7S-7|
            L|7||
            -L-J|
            L|-JF
            """);

        // ASSERT
        part1Example1Result.Should().Be(4);
    }

    [Test]
    public void Part1Example2()
    {
        // ACT
        var part1Example2Result = _sut.SolvePart1("""
            7-F7-
            .FJ|7
            SJLL7
            |F--J
            LJ.LJ
            """);

        // ASSERT
        part1Example2Result.Should().Be(8);
    }

    [Test]
    public void Part1ReTest()
    {
        // ACT
        var part1Result = _sut.SolvePart1();

        // ASSERT
        part1Result.Should().Be(6864);
    }

    [Test]
    public void Part2Example()
    {
        // ACT
        var part2ExampleResult = _sut.SolvePart2("rs-todo: ExampleInput");

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

using AoC.Day08;

namespace AoC.Tests.Day08;

public class Day8SolverTests
{
    private readonly Day8Solver _sut = new();

    private const string ExampleInput1 = """
        RL

        AAA = (BBB, CCC)
        BBB = (DDD, EEE)
        CCC = (ZZZ, GGG)
        DDD = (DDD, DDD)
        EEE = (EEE, EEE)
        GGG = (GGG, GGG)
        ZZZ = (ZZZ, ZZZ)
        """;

    private const string ExampleInput2 = """
        LLR

        AAA = (BBB, BBB)
        BBB = (AAA, ZZZ)
        ZZZ = (ZZZ, ZZZ)
        """;

    [Test]
    public void Part1Example1()
    {
        // ACT
        var part1ExampleResult = _sut.SolvePart1(ExampleInput1);

        // ASSERT
        part1ExampleResult.Should().Be(2);
    }

    [Test]
    public void Part1Example2()
    {
        // ACT
        var part1ExampleResult = _sut.SolvePart1(ExampleInput2);

        // ASSERT
        part1ExampleResult.Should().Be(6);
    }

    [Test]
    public void Part1ReTest()
    {
        // ACT
        var part1Result = _sut.SolvePart1();

        // ASSERT
        part1Result.Should().Be(18673);
    }

    [Test]
    public void Part2Example1()
    {
        // ACT
        var part2ExampleResult = _sut.SolvePart2(ExampleInput1);

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

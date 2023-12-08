using AoC.Day08;

namespace AoC.Tests.Day08;

public class Day8SolverTests
{
    private readonly Day8Solver _sut = new();

    [Test]
    public void Part1Example1()
    {
        // ACT
        var part1ExampleResult = _sut.SolvePart1("""
            RL

            AAA = (BBB, CCC)
            BBB = (DDD, EEE)
            CCC = (ZZZ, GGG)
            DDD = (DDD, DDD)
            EEE = (EEE, EEE)
            GGG = (GGG, GGG)
            ZZZ = (ZZZ, ZZZ)
            """);

        // ASSERT
        part1ExampleResult.Should().Be(2);
    }

    [Test]
    public void Part1Example2()
    {
        // ACT
        var part1ExampleResult = _sut.SolvePart1("""
            LLR

            AAA = (BBB, BBB)
            BBB = (AAA, ZZZ)
            ZZZ = (ZZZ, ZZZ)
            """);

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
    public void Part2Example3()
    {
        // ACT
        var part2ExampleResult = _sut.SolvePart2("""
            LR

            11A = (11B, XXX)
            11B = (XXX, 11Z)
            11Z = (11B, XXX)
            22A = (22B, XXX)
            22B = (22C, 22C)
            22C = (22Z, 22Z)
            22Z = (22B, 22B)
            XXX = (XXX, XXX)
            """);

        // ASSERT
        part2ExampleResult.Should().Be(6);
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

using AoC.Day01;

namespace AoC.Tests.Day01;

public class Day1SolverTests
{
    private readonly Day1Solver _sut = new();

    [Test]
    public void Part1Example()
    {
        // ACT
        var part1ExampleResult = _sut.SolvePart1("""
            1abc2
            pqr3stu8vwx
            a1b2c3d4e5f
            treb7uchet
            """);

        // ASSERT
        part1ExampleResult.Should().Be(142);
    }

    [Test]
    public void Part1ReTest()
    {
        // ACT
        var part1Result = _sut.SolvePart1();

        // ASSERT
        part1Result.Should().Be(54239);
    }

    [Test]
    public void Part2Example()
    {
        // ACT
        var part2ExampleResult = _sut.SolvePart2("""
            two1nine
            eightwothree
            abcone2threexyz
            xtwone3four
            4nineeightseven2
            zoneight234
            7pqrstsixteen
            """);

        // ASSERT
        part2ExampleResult.Should().Be(281);
    }

    [Test]
    public void Part2ReTest()
    {
        // ACT
        var part2Result = _sut.SolvePart2();

        // ASSERT
        part2Result.Should().BeGreaterThan(55330);
        part2Result.Should().BeLessThan(55345);
        part2Result.Should().Be(55343);
    }
}

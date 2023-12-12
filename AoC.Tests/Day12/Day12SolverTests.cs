using AoC.Day12;
using static AoC.Day12.Day12Solver;

namespace AoC.Tests.Day12;

public class Day12SolverTests
{
    private readonly Day12Solver _sut = new();

    private const string ExampleInput = """
        ???.### 1,1,3
        .??..??...?##. 1,1,3
        ?#?#?#?#?#?#?#? 1,3,1,6
        ????.#...#... 4,1,1
        ????.######..#####. 1,6,5
        ?###???????? 3,2,1
        """;

    [Test]
    public void Part1IndividualExamples()
    {
        using var _ = new AssertionScope();

        ConditionReport.Parse("???.### 1,1,3").CountPossibleArrangements().Should().Be(1);
        ConditionReport.Parse(".??..??...?##. 1,1,3").CountPossibleArrangements().Should().Be(4);
        ConditionReport.Parse("?#?#?#?#?#?#?#? 1,3,1,6").CountPossibleArrangements().Should().Be(1);
        ConditionReport.Parse("????.#...#... 4,1,1").CountPossibleArrangements().Should().Be(1);
        ConditionReport.Parse("????.######..#####. 1,6,5").CountPossibleArrangements().Should().Be(4);
        ConditionReport.Parse("?###???????? 3,2,1").CountPossibleArrangements().Should().Be(10);
    }

    [Test]
    public void Part1Example()
    {
        // ACT
        var part1ExampleResult = _sut.SolvePart1(ExampleInput);

        // ASSERT
        part1ExampleResult.Should().Be(21);
    }

    [Test]
    public void Part1ReTest()
    {
        // ACT
        var part1Result = _sut.SolvePart1();

        // ASSERT
        part1Result.Should().Be(7118);
    }

    [Test]
    public void Part2Example()
    {
        // ACT
        var part2ExampleResult = _sut.SolvePart2(ExampleInput);

        // ASSERT
        part2ExampleResult.Should().Be(525152);
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

using AoC.Day15;

namespace AoC.Tests.Day15;

public class Day15SolverTests
{
    private readonly Day15Solver _sut = new();

    private const string ExampleInput = "rn=1,cm-,qp=3,cm=2,qp-,pc=4,ot=9,ab=5,pc-,pc=6,ot=7";

    [TestCase("HASH", 52)]
    [TestCase("rn=1", 30)]
    [TestCase("cm-", 253)]
    [TestCase("rn", 0)]
    [TestCase("qp", 1)]
    public void Part1IndividualExamples(string input, long expectedResult)
    {
        // ACT
        var result = Day15Solver.HASH(input);

        // ASSERT
        result.Should().Be(expectedResult);
    }

    [Test]
    public void Part1Example()
    {
        // ACT
        var part1ExampleResult = _sut.SolvePart1(ExampleInput);

        // ASSERT
        part1ExampleResult.Should().Be(1320);
    }

    [Test]
    public void Part1ReTest()
    {
        // ACT
        var part1Result = _sut.SolvePart1();

        // ASSERT
        part1Result.Should().Be(506869);
    }

    [Test]
    public void Part2Example()
    {
        // ACT
        var part2ExampleResult = _sut.SolvePart2(ExampleInput);

        // ASSERT
        part2ExampleResult.Should().Be(145);
    }

    [Test]
    public void Part2ReTest()
    {
        // ACT
        var part2Result = _sut.SolvePart2();

        // ASSERT
        part2Result.Should().Be(271384);
    }
}

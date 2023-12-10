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
    public void Part2Example1()
    {
        // ACT
        var part2Example1Result = _sut.SolvePart2("""
            ...........
            .S-------7.
            .|F-----7|.
            .||.....||.
            .||.....||.
            .|L-7.F-J|.
            .|..|.|..|.
            .L--J.L--J.
            ...........
            """);

        // ASSERT
        part2Example1Result.Should().Be(4);
    }

    [Test]
    public void Part2Example2()
    {
        // ACT
        var part2Example2Result = _sut.SolvePart2("""
            ..........
            .S------7.
            .|F----7|.
            .||....||.
            .||....||.
            .|L-7F-J|.
            .|..||..|.
            .L--JL--J.
            ..........
            """);

        // ASSERT
        part2Example2Result.Should().Be(4);
    }

    [Test]
    public void Part2Example3()
    {
        // ACT
        var part2Example3Result = _sut.SolvePart2("""
            .F----7F7F7F7F-7....
            .|F--7||||||||FJ....
            .||.FJ||||||||L7....
            FJL7L7LJLJ||LJ.L-7..
            L--J.L7...LJS7F-7L7.
            ....F-J..F7FJ|L7L7L7
            ....L7.F7||L7|.L7L7|
            .....|FJLJ|FJ|F7|.LJ
            ....FJL-7.||.||||...
            ....L---J.LJ.LJLJ...
            """);

        // ASSERT
        part2Example3Result.Should().Be(8);
    }

    [Test]
    public void Part2Example4()
    {
        // ACT
        var part2Example4Result = _sut.SolvePart2("""
            FF7FSF7F7F7F7F7F---7
            L|LJ||||||||||||F--J
            FL-7LJLJ||||||LJL-77
            F--JF--7||LJLJ7F7FJ-
            L---JF-JLJ.||-FJLJJ7
            |F|F-JF---7F7-L7L|7|
            |FFJF7L7F-JF7|JL---7
            7-L-JL7||F7|L7F-7F7|
            L.L7LFJ|||||FJL7||LJ
            L7JLJL-JLJLJL--JLJ.L
            """);

        // ASSERT
        part2Example4Result.Should().Be(10);
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

namespace AoC.Day01;

public class Day1Solver : ISolver
{
    public string DayName => "Trebuchet?!";

    static long GetDigit(IEnumerable<char> line) => long.Parse($"{line.SkipWhile(c => !char.IsDigit(c)).First()}");

    public long? SolvePart1(string input) =>
        input.ReadLines()
            .Select(line => long.Parse($"{GetDigit(line)}{GetDigit(line.Reverse())}"))
            .Sum();

    public long? SolvePart2(string input)
    {
        return null;
    }
}

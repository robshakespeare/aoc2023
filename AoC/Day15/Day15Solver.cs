namespace AoC.Day15;

public class Day15Solver : ISolver
{
    public string DayName => "Lens Library";

    public long? SolvePart1(string input) => input.Split(',').Sum(HASH);

    public long? SolvePart2(string input)
    {
        return null;
    }

    public static long HASH(string s) => s.Aggregate(0L, (agg, cur) => ((agg + cur) * 17) % 256);
}

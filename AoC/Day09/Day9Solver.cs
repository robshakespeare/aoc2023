namespace AoC.Day09;

public class Day9Solver : ISolver
{
    public string DayName => "Mirage Maintenance";

    public long? SolvePart1(string input) => Solve(input);

    public long? SolvePart2(string input) => Solve(input, reverse: true);

    static long Solve(string input, bool reverse = false) => input.ReadLines()
        .Select(line => line.Split(" ").Select(long.Parse))
        .Select(v => (reverse ? v.Reverse() : v).ToArray())
        .Select(AnalyzeHistory)
        .Sum(x => x.ExtrapolatedValue);

    record AnalyzedHistory(long[][] Sequences)
    {
        public long ExtrapolatedValue { get; } = GetExtrapolatedValue(Sequences);

        static long GetExtrapolatedValue(long[][] sequences, int pointer = 0) =>
            pointer >= sequences.Length
                ? 0
                : sequences[pointer][^1] + GetExtrapolatedValue(sequences, pointer + 1);
    }

    static AnalyzedHistory AnalyzeHistory(long[] history)
    {
        var results = new List<long[]>();
        var current = history;

        while (current != null)
        {
            results.Add(current);
            current = GetNextSequence(current);
        }

        return new AnalyzedHistory([.. results]);
    }

    static long[]? GetNextSequence(long[] sequence)
    {
        if (sequence.All(n => n == 0))
        {
            return null;
        }

        var results = new long[sequence.Length - 1];
        for (var i = 1; i < sequence.Length; i++)
        {
            results[i - 1] = sequence[i] - sequence[i - 1];
        }

        return results;
    }
}

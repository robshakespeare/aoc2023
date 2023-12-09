namespace AoC.Day09;

public class Day9Solver : ISolver
{
    public string DayName => "Mirage Maintenance";

    public long? SolvePart1(string input) => ParseInput(input).Sum(x => x.ExtrapolatedValue);

    public long? SolvePart2(string input) => ParseInput(input).Sum(x => x.PrependedValue);

    record AnalyzedHistory(long[][] Sequences)
    {
        public long ExtrapolatedValue { get; } = GetExtrapolatedValue(Sequences);

        public long PrependedValue { get; } = GetPrependedValue(Sequences);

        static long GetExtrapolatedValue(long[][] sequences, int pointer = 0) =>
            pointer >= sequences.Length
                ? 0
                : sequences[pointer][^1] + GetExtrapolatedValue(sequences, pointer + 1);

        static long GetPrependedValue(long[][] sequences, int pointer = 0) =>
            pointer >= sequences.Length
                ? 0
                : sequences[pointer][0] - GetPrependedValue(sequences, pointer + 1);
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

    static IEnumerable<AnalyzedHistory> ParseInput(string input) =>
        input.ReadLines().Select(line => line.Split(" ").Select(long.Parse).ToArray()).Select(AnalyzeHistory);
}

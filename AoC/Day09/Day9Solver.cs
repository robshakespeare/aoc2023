namespace AoC.Day09;

public class Day9Solver : ISolver
{
    public string DayName => "Mirage Maintenance";

    public long? SolvePart1(string input)
    {
        var analyzedHistories = ParseInput(input).Select(AnalyzeHistory);

        return analyzedHistories.Sum(x => x.ExtrapolatedValue);
    }

    public long? SolvePart2(string input)
    {
        var analyzedHistories = ParseInput(input).Select(AnalyzeHistory);

        analyzedHistories.Dump();

        return analyzedHistories.Sum(x => x.PreappendedValue);
    }

    record AnalyzedHistory(long[][] Sequences)
    {
        public long ExtrapolatedValue { get; } = GetExtrapolatedValue(Sequences);

        public long PreappendedValue { get; } = GetPreappendedValue(Sequences);

        static long GetExtrapolatedValue(long[][] sequences, int pointer = 0) =>
            pointer >= sequences.Length
                ? 0
                : sequences[pointer][^1] + GetExtrapolatedValue(sequences, pointer + 1);

        static long GetPreappendedValue(long[][] sequences, int pointer = 0) =>
            pointer >= sequences.Length
                ? 0
                : sequences[pointer][0] - GetPreappendedValue(sequences, pointer + 1);
    }

    static AnalyzedHistory AnalyzeHistory(long[] history)
    {
        var results = new List<long[]>();
        var current = history;

        // rs-todo: tidy?
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
            //yield return sequence[i] - sequence[i - 1]; // rs-todo: tidy?
        }

        return results;
    }

    static long[][] ParseInput(string input) => input.ReadLines().Select(line => line.Split(" ").Select(long.Parse).ToArray()).ToArray();
}

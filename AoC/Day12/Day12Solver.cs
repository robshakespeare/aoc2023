namespace AoC.Day12;

public class Day12Solver : ISolver
{
    public string DayName => "Hot Springs";

    public long? SolvePart1(string input) => ParseInput(input).Sum(x => x.GetPossibleArrangements().Length);

    public long? SolvePart2(string input)
    {
        return null;
    }

    public record ConditionReport(string RawConditionRecords, int[] SizeOfEachContiguousGroupOfDamagedSprings)
    {
        public static ConditionReport Parse(string line)
        {
            var split = line.Split(' ');
            return new(split[0], split[1].Split(',').Select(int.Parse).ToArray());
        }

        public string[] GetPossibleArrangements()
        {
            var permutationsOfConditionRecords = EnumeratePermutationsOfConditionRecords(RawConditionRecords);
            var expectedCounts = SizeOfEachContiguousGroupOfDamagedSprings;
            var arrangements = permutationsOfConditionRecords
                .Select(records => new
                {
                    records,
                    ActualSizeOfEachContiguousGroupOfDamagedSprings = records
                        .ContiguousGroupBy(c => c)
                        .Where(group => group.Key == '#')
                        .Select(group => group.Count())
                })
                .Where(x => Enumerable.SequenceEqual(expectedCounts, x.ActualSizeOfEachContiguousGroupOfDamagedSprings));

            return arrangements.Select(x => x.records).ToArray();
        }
    }

    static ConditionReport[] ParseInput(string input) => input.ReadLines().Select(ConditionReport.Parse).ToArray();

    static string[] EnumeratePermutationsOfConditionRecords(string rawConditionRecords)
    {
        var results = new List<string>([""]);

        foreach (var sourceChar in rawConditionRecords)
        {
            char[] appendChars = sourceChar switch
            {
                '.' or '#' => [sourceChar],
                '?' => ['.', '#'],
                _ => throw new Exception("Unexpected char: " + sourceChar)
            };

            var nextResults = new List<string>();

            foreach (var result in results)
            {
                foreach (var appendChar in appendChars)
                {
                    nextResults.Add(result + appendChar);
                }
            }

            results = nextResults;
        }

        return results.Select(result => result.ToString()).ToArray();
    }
}

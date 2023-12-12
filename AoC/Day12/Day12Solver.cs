namespace AoC.Day12;

public class Day12Solver : ISolver
{
    public string DayName => "Hot Springs";

    public long? SolvePart1(string input)
    {
        var rawInput = ParseInput(input).Select(item =>
        {
            var permutationsOfConditionRecords = EnumeratePermutationsOfConditionRecords(item.RawConditionRecords);
            var expectedCounts = item.SizeOfEachContiguousGroupOfDamagedSprings;
            var arrangements = permutationsOfConditionRecords
                .Select(records => records.ContiguousGroupBy(c => c).Where(group => group.Key == '#').Select(group => group.Count()))
                .Where(actualCounts => Enumerable.SequenceEqual(expectedCounts, actualCounts))
                .ToArray();

            return new
            {
                item.RawConditionRecords,
                item.SizeOfEachContiguousGroupOfDamagedSprings,
                permutationsOfConditionRecords,

            };
        });

        return null;
    }

    public long? SolvePart2(string input)
    {
        return null;
    }

    //public static string[] GetPossibleArrangements(ConditionReport conditionReport)
    //{
    //    var permutationsOfConditionRecords = EnumeratePermutationsOfConditionRecords(conditionReport.RawConditionRecords);
    //    var expectedCounts = conditionReport.SizeOfEachContiguousGroupOfDamagedSprings;
    //    var arrangements = permutationsOfConditionRecords
    //        .Select(records => new
    //        {
    //            records,
    //            ActualSizeOfEachContiguousGroupOfDamagedSprings = records.ContiguousGroupBy(c => c).Where(group => group.Key == '#').Select(group => group.Count())
    //        })
    //        .Where(x => Enumerable.SequenceEqual(expectedCounts, x.ActualSizeOfEachContiguousGroupOfDamagedSprings));

    //    return arrangements.Select(x => x.records).ToArray();
    //}

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
                    ActualSizeOfEachContiguousGroupOfDamagedSprings = records.ContiguousGroupBy(c => c).Where(group => group.Key == '#').Select(group => group.Count())
                })
                .Where(x => Enumerable.SequenceEqual(expectedCounts, x.ActualSizeOfEachContiguousGroupOfDamagedSprings));

            return arrangements.Select(x => x.records).ToArray();
        }
    }

    static IEnumerable<(string RawConditionRecords, int[] SizeOfEachContiguousGroupOfDamagedSprings)> ParseInput(string input) =>
        input.ReadLines()
            .Select(line => line.Split(' '))
            .Select(split => (split[0], split[1].Split(',').Select(int.Parse).ToArray()));

    //static IEnumerable<(string RawConditionRecords, int[] SizeOfEachContiguousGroupOfDamagedSprings)> ParseInput(string input) =>
    //    input.ReadLines()
    //        .Select(line => line.Split(' '))
    //        .Select(split => (split[0], split[1].Split(',').Select(int.Parse).ToArray()));

    static string[] EnumeratePermutationsOfConditionRecords(string rawConditionRecords)
    {
        var results = new List<StringBuilder>([new StringBuilder()]);

        foreach (var sourceChar in rawConditionRecords)
        {
            char[] appendChars = sourceChar switch
            {
                '.' or '#' => [sourceChar],
                '?' => ['.', '#'],
                _ => throw new Exception("Unexpected char: " + sourceChar)
            };

            foreach (var (appendChar, createNew) in appendChars.Select((appendChar, i) => (appendChar, CreateNew: i > 0)))
            {
                var additionalResults = new List<StringBuilder>();
                foreach (var result in results)
                {
                    if (createNew)
                    {
                        var additionalResult = new StringBuilder(result.ToString());
                        additionalResult.Append(appendChar);
                        additionalResults.Add(additionalResult);
                    }
                    else
                    {
                        result.Append(appendChar);
                    }
                }
                results.AddRange(additionalResults);
            }
        }

        return results.Select(result => result.ToString()).ToArray();
    }
}

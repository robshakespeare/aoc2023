using System.Linq;
using System.Text.RegularExpressions;
using Spectre.Console;

namespace AoC.Day12;

public class Day12Solver : ISolver
{
    public string DayName => "Hot Springs";

    public long? SolvePart1(string input)
    {
        return Solve(input);

        //HashSet<string>? validArrangementsCache = new();
        //return ParseInput(input).Sum(x => x.CountPossibleArrangements(null));
    }

    public long? SolvePart2(string input)
    {
        return Solve(input, true);
    }

    private long? Solve(string input, bool fold = false)
    {
        var conditionReports = ParseInput(input);

        if (fold)
        {
            conditionReports = conditionReports
            .Select(conditionReport => new ConditionReport(
                string.Join('?', Enumerable.Repeat(conditionReport.RawConditionRecords, 5)),
                Enumerable.Repeat(conditionReport.SizeOfEachContiguousGroupOfDamagedSprings, 5).SelectMany(x => x).ToArray()))
            .ToArray();
        }

        HashSet<string>? validArrangementsCache = null; // new(); // rs-todo: doesn't look like this speeds things up!
        Dictionary<string, int>? countPossibleArrangementsCache = new();

        var totalPossibleArrangements = 0;
        foreach (var conditionReport in conditionReports)
        {
            //int possibleArrangements;

            if (!countPossibleArrangementsCache.TryGetValue(conditionReport.CacheKey, out var possibleArrangements))
            {
                possibleArrangements = conditionReport.CountPossibleArrangements(validArrangementsCache);
                countPossibleArrangementsCache.Add(conditionReport.CacheKey, possibleArrangements);
            }

            totalPossibleArrangements += possibleArrangements;

            
        }

        return totalPossibleArrangements;
    }

    public record ConditionReport(string RawConditionRecords, int[] SizeOfEachContiguousGroupOfDamagedSprings)
    {
        public string CacheKey { get; } = $"{RawConditionRecords}_{string.Join(",", SizeOfEachContiguousGroupOfDamagedSprings)}";

        public static ConditionReport Parse(string line)
        {
            var split = line.Split(' ');
            return new(split[0], split[1].Split(',').Select(int.Parse).ToArray());
        }

        public int CountPossibleArrangements(HashSet<string>? validArrangementsCache = null)
        {
            //var start = new ConditionRecords(RawConditionRecords);



            //return 0;

            var (permutationsOfConditionRecords, endsReached) = EnumeratePermutationsOfConditionRecords(RawConditionRecords);

            //var test = permutationsOfConditionRecords.ToArray();

            //return test.Length;

            ////// debug:
            //////Console.WriteLine($"{RawConditionRecords} -- {permutationsOfConditionRecords.Count()}");

            ////return 0;

            var expectedCounts = SizeOfEachContiguousGroupOfDamagedSprings;

            //var newArrangementsCount = 0;

            //foreach (var records in permutationsOfConditionRecords)
            //{
            //    var groupIndex = 0;
            //    var overflow = false;

            //    foreach (var grouping in records.ContiguousGroupBy(c => c).Where(group => group.Key == '#'))
            //    {
            //        if (groupIndex >= expectedCounts.Length)
            //        {
            //            overflow = true;
            //            break;
            //        }
            //        if (grouping.Count() != expectedCounts[groupIndex])
            //        {
            //            break;
            //        }
            //        groupIndex++;
            //    }

            //    if (groupIndex == expectedCounts.Length && !overflow)
            //    {
            //        newArrangementsCount++;
            //    }
            //}

            //return newArrangementsCount;

            var counts = string.Join(",", SizeOfEachContiguousGroupOfDamagedSprings);

            var arrangementsCount = 0;

            foreach (var permutationsOfConditionRecord in permutationsOfConditionRecords)
            {
                var cacheKey = string.Concat(permutationsOfConditionRecord) + counts;

                if (validArrangementsCache != null && validArrangementsCache.Contains(cacheKey))
                {
                    arrangementsCount++;
                }
                else
                {
                    var actualCounts = permutationsOfConditionRecord.ContiguousGroupBy(c => c)
                        .Where(group => group.Key == '#')
                        .Select(group => group.Count());

                    var valid = Enumerable.SequenceEqual(expectedCounts, actualCounts);

                    if (valid)
                    {
                        arrangementsCount++;
                        validArrangementsCache?.Add(cacheKey);
                    }
                }
            }

            //var arrangementsCount = permutationsOfConditionRecords
            //    .Select(records => records
            //        .ContiguousGroupBy(c => c)
            //        .Where(group => group.Key == '#')
            //        .Select(group => group.Count()))
            //    .Where(actualCounts => Enumerable.SequenceEqual(expectedCounts, actualCounts))
            //    .Count();

            //var endsReachedCount = endsReached.Count;

            //var expectedCounts = SizeOfEachContiguousGroupOfDamagedSprings;
            //var arrangements = permutationsOfConditionRecords
            //    .Select(records => new
            //    {
            //        records,
            //        ActualSizeOfEachContiguousGroupOfDamagedSprings = records
            //            .ContiguousGroupBy(c => c)
            //            .Where(group => group.Key == '#')
            //            .Select(group => group.Count())
            //    })
            //    .Where(x => Enumerable.SequenceEqual(expectedCounts, x.ActualSizeOfEachContiguousGroupOfDamagedSprings))
            //    .Select(x => x.records)
            //    .ToArray();

            // debug:
            //Console.WriteLine($"{RawConditionRecords} -- {permutationsOfConditionRecords.Count()} -- {arrangementsCount} -- {endsReachedCount}");

            return arrangementsCount;
        }
    }

    static ConditionReport[] ParseInput(string input) => input.ReadLines().Select(ConditionReport.Parse).ToArray();

    record ConditionRecords(string RawRecords, string Path = "") //, int Pointer = 0)
    {
        public int Pointer { get; } = Path.Length;

        public ConditionRecords[] GetSuccessors()
        {
            if (Pointer >= RawRecords.Length)
            {
                return [];
            }

            var record = RawRecords[Pointer];

            return record switch
            {
                '.' or '#' => [Append(record)],
                '?' => [Append('.'), Append('#')],
                _ => throw new Exception("Unexpected char: " + record)
            };
        }

        ConditionRecords Append(char next) => new ConditionRecords(RawRecords, Path + next);
    }

    public class EndsReached
    {
        public int Count { get; set; }
    }

    static (IEnumerable<IEnumerable<char>>, EndsReached) EnumeratePermutationsOfConditionRecords(string rawConditionRecords)
    {
        /*
            Good:
            var possibleChars = new[] { '.', '#' };

            return rawConditionRecords.Aggregate(
                (IEnumerable<IEnumerable<char>>)new[] { Enumerable.Empty<char>() },
                (current, record) =>
                {
                    return record switch
                    {
                        '.' or '#' => current.Select(records => records.Append(record)),
                        '?' => current.SelectMany(records => possibleChars.Select(c => records.Append(c))),
                        _ => throw new Exception("Unexpected char: " + record)
                    };
                });
         */

        var possibleChars = new[] { '.', '#' };

        var result = rawConditionRecords.Aggregate(
            (IEnumerable<IEnumerable<char>>)new[] { Enumerable.Empty<char>() },
            (current, record) =>
            {
                return record switch
                {
                    '.' or '#' => current.Select(records => records.Append(record)),
                    '?' => current.SelectMany(records => possibleChars.Select(c => records.Append(c))),
                    _ => throw new Exception("Unexpected char: " + record)
                };
            });


        var endsReached = new EndsReached();
        IEnumerable<char> End()
        {
            endsReached.Count++;
            return Enumerable.Empty<char>();
        }

        result = result.Select(line => line.Concat(End()));

        return (result, endsReached);

        //return rawConditionRecords.Skip(1).Aggregate(
        //    (IEnumerable<IEnumerable<char>>)new[] { new[] { rawConditionRecords[0] } },
        //    (current, record) =>
        //    {
        //        return record switch
        //        {
        //            '.' or '#' => current.Select(records => records.Append(record)),
        //            '?' => current.SelectMany(records => possibleChars.Select(c => records.Append(c))),
        //            _ => throw new Exception("Unexpected char: " + record)
        //        };
        //    });

        //return rawConditionRecords.Aggregate(Enumerable.Empty<IEnumerable<char>>(), (current, record) =>
        //{
        //    return record switch
        //    {
        //        '.' or '#' => current.Select(records => records.Append(record)),
        //        '?' => current.SelectMany(records => possibleChars.Select(c => records.Append(c))),
        //        _ => throw new Exception("Unexpected char: " + record)
        //    };
        //});

        //var results = new List<string>([""]);

        //foreach (var sourceChar in rawConditionRecords)
        //{
        //    char[] appendChars = sourceChar switch
        //    {
        //        '.' or '#' => [sourceChar],
        //        '?' => ['.', '#'],
        //        _ => throw new Exception("Unexpected char: " + sourceChar)
        //    };

        //    var nextResults = new List<string>();

        //    foreach (var result in results)
        //    {
        //        foreach (var appendChar in appendChars)
        //        {
        //            nextResults.Add(result + appendChar);
        //        }
        //    }

        //    results = nextResults;
        //}

        //return results.Select(result => result.ToString()).ToArray();
    }
}

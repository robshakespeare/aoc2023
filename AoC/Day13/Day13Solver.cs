using static System.Environment;

namespace AoC.Day13;

public class Day13Solver : ISolver
{
    public string DayName => "Point of Incidence";

    public long? SolvePart1(string input) => Grid.ParseInput(input).Sum(grid => grid.GetMirrorLine(smudge: false));

    public long? SolvePart2(string input) => Grid.ParseInput(input).Sum(grid => grid.GetMirrorLine(smudge: true));

    public record Grid(string GridFull, string[] Rows)
    {
        public string[] Columns { get; } = TransposeRowsToColumns(Rows);

        public long GetMirrorLine(bool smudge) =>
            GetMirrorLine(Rows, isHorizontal: true, smudge) ?? GetMirrorLine(Columns, isHorizontal: false, smudge) ?? throw new Exception("Couldn't find mirror line");

        static long? GetMirrorLine(string[] lines, bool isHorizontal, bool smudge)
        {
            var targetDifferences = smudge ? 1 : 0;
            static int CountDifferences(string a, string b) => a.Zip(b).Count(pair => pair.First != pair.Second);

            for (var candidate = 0; candidate < lines.Length - 1; candidate++)
            {
                var endReached = false;
                var differences = 0;

                for (int i = 0; i <= candidate; i++)
                {
                    var idx = candidate - i;
                    var inverseIdx = candidate + 1 + i;

                    if (inverseIdx >= lines.Length)
                    {
                        endReached = true;
                        break;
                    }

                    differences += CountDifferences(lines[idx], lines[inverseIdx]);

                    if (differences <= 1)
                    {
                        endReached = idx == 0;
                    }
                    else
                    {
                        break;
                    }
                }

                if (endReached && differences == targetDifferences)
                {
                    return (candidate + 1) * (isHorizontal ? 100L : 1L);
                }
            }

            return null;
        }

        public static Grid[] ParseInput(string input) => input.Split(NewLine + NewLine).Select(Parse).ToArray();

        static Grid Parse(string grid) => new(grid, grid.ReadLines().ToArray());

        static string[] TransposeRowsToColumns(string[] Rows)
        {
            string GetColumn(int x) => string.Concat(Rows.Select(line => line[x]));
            return Enumerable.Range(0, Rows[0].Length).Select(GetColumn).ToArray();
        }
    }
}

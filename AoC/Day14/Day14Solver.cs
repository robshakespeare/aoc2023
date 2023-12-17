namespace AoC.Day14;

public class Day14Solver : ISolver
{
    public string DayName => "Parabolic Reflector Dish";

    public long? SolvePart1(string input)
    {
        var tilted = TransposeRowsToColumns(input.ReadLines().ToArray())
            .Select(line => TiltLine(new StringBuilder(line)))
            .ToArray();

        return tilted.SelectMany(line => line.Select((c, i) => c == 'O' ? line.Length - i : 0)).Sum();
    }

    public long? SolvePart2(string input)
    {
        return null;
    }

    static string[] TransposeRowsToColumns(string[] Rows)
    {
        string GetColumn(int x) => string.Concat(Rows.Select(line => line[x]));
        return Enumerable.Range(0, Rows[0].Length).Select(GetColumn).ToArray();
    }

    static string TiltLine(StringBuilder line)
    {
        // Working from Beginning to End, move each rounded rock O to Beginning, while there is space or it reaches Beginning

        for (var pos = 0; pos < line.Length; pos++)
        {
            if (line[pos] == 'O')
            {
                for (var idx = pos - 1; idx >= 0; idx--)
                {
                    if (line[idx] == '.')
                    {
                        line[idx] = 'O';
                        line[idx + 1] = '.';
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        return line.ToString();
    }
}

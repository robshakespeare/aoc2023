namespace AoC.Day14;

public class Day14Solver : ISolver
{
    public string DayName => "Parabolic Reflector Dish";

    public long? SolvePart1(string input)
    {
        var grid = ParseInput(input);

        ITilternator northTilternator = new NorthTilternator(grid);

        //Console.WriteLine();
        //Console.WriteLine(string.Join(Environment.NewLine, grid.Select(line => line.ToString())));

        northTilternator.Tilt();
        //Tilt(northTilternator);

        //Console.WriteLine();
        //Console.WriteLine(string.Join(Environment.NewLine, grid.Select(line => line.ToString())));

        return CalculateNorthSupportBeamLoad(grid);

        //var tilted = TransposeRowsToColumns(input.ReadLines().ToArray())
        //    .Select(line => TiltLine(new StringBuilder(line)))
        //    .ToArray();

        //return tilted.SelectMany(line => line.Select((c, i) => c == 'O' ? line.Length - i : 0)).Sum();
    }

    public long? SolvePart2(string input)
    {
        var grid = ParseInput(input);

        List<ITilternator> tilternators = [
            new NorthTilternator(grid),
            new WestTilternator(grid),
            new SouthTilternator(grid),
            new EastTilternator(grid)
        ];

        void Cycle() => tilternators.ForEach(tilternator => tilternator.Tilt());

        //void Cycle()
        //{
        //    foreach (var tilternator in tilternators)
        //    {
        //        tilternator.Tilt();
        //    }
        //}

        string GridToString() => string.Join(Environment.NewLine, grid.Select(line => line.ToString()));

        const int totalCycles = 1000000000;

        // Find the cycle size
        Dictionary<string, int> gridCycleNums = [];

        var cycleSize = 0;
        //var prevMatchingCycleNum = 0;
        var cycleNum = 0;

        while (cycleSize == 0)
        //for (int cycleNum = 1; cycleNum <= 1000; cycleNum++)
        {
            Cycle();
            cycleNum++;

            var gridString = GridToString();

            if (gridCycleNums.TryGetValue(gridString, out var prevMatchingCycleNum))
            {
                cycleSize = cycleNum - prevMatchingCycleNum;

                //Console.WriteLine($"Matching cycle {prevCycleNum} and {cycleNum} (delta: {cycleNum - prevCycleNum}):");
                //Console.WriteLine(GridToString());
                //Console.WriteLine();
            }
            else
            {
                gridCycleNums[gridString] = cycleNum;
            }

            //Console.WriteLine($"After {cycleNum} cycle:");
            //Console.WriteLine(GridToString());
            //Console.WriteLine();
        }

        // Time warp!
        cycleNum += (totalCycles - cycleNum) / cycleSize * cycleSize;

        // Finish the run!
        for (; cycleNum < totalCycles; cycleNum++)
        {
            Cycle();
        }

        return CalculateNorthSupportBeamLoad(grid);
    }

    // Good:
    //static void Tilt(ITilternator tilternator)
    //{
    //    for (var axis = 0; axis < tilternator.AxisLength; axis++)
    //    {
    //        // Working from Beginning to End, move each rounded rock O to Beginning, while there is space or it reaches Beginning
    //        for (var pos = 0; pos < tilternator.OperableLength; pos++)
    //        {
    //            if (tilternator[pos, axis] == 'O')
    //            {
    //                for (var idx = pos - 1; idx >= 0; idx--)
    //                {
    //                    if (tilternator[idx, axis] == '.')
    //                    {
    //                        tilternator[idx, axis] = 'O';
    //                        tilternator[idx + 1, axis] = '.';
    //                    }
    //                    else
    //                    {
    //                        break;
    //                    }
    //                }
    //            }
    //        }
    //    }
    //}

    static StringBuilder[] ParseInput(string input) => input.ReadLines().ToArray()
        .Select(line => new StringBuilder(line))
        .ToArray();

    static long CalculateNorthSupportBeamLoad(StringBuilder[] grid) => Enumerable.Range(0, grid.Length).Sum(y =>
        {
            long loadPerRock = grid.Length - y;
            return grid[y].ToString().LongCount(c => c == 'O') * loadPerRock;
        });

    //static string[] TransposeRowsToColumns(string[] Rows)
    //{
    //    string GetColumn(int x) => string.Concat(Rows.Select(line => line[x]));
    //    return Enumerable.Range(0, Rows[0].Length).Select(GetColumn).ToArray();
    //}

    //static string TiltLine(StringBuilder line)
    //{
    //    // Working from Beginning to End, move each rounded rock O to Beginning, while there is space or it reaches Beginning

    //    for (var pos = 0; pos < line.Length; pos++)
    //    {
    //        if (line[pos] == 'O')
    //        {
    //            for (var idx = pos - 1; idx >= 0; idx--)
    //            {
    //                if (line[idx] == '.')
    //                {
    //                    line[idx] = 'O';
    //                    line[idx + 1] = '.';
    //                }
    //                else
    //                {
    //                    break;
    //                }
    //            }
    //        }
    //    }

    //    return line.ToString();
    //}
}

public interface ITilternator
{
    int AxisLength { get; } // For north and south, this is the columns. For east and west, this is the rows.
    int OperableLength { get; } // For north and south, this is the rows. For east and west, this is the columns. North and west normal, South and East inverted.
    char this[int operableIndex, int axisIndex] { get; set; }

    void Tilt()
    {
        for (var axis = 0; axis < AxisLength; axis++)
        {
            // Working from Beginning to End, move each rounded rock O to Beginning, while there is space or it reaches Beginning
            for (var pos = 0; pos < OperableLength; pos++)
            {
                if (this[pos, axis] == 'O')
                {
                    for (var idx = pos - 1; idx >= 0; idx--)
                    {
                        if (this[idx, axis] == '.')
                        {
                            this[idx, axis] = 'O';
                            this[idx + 1, axis] = '.';
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
        }
    }
}

public class NorthTilternator(StringBuilder[] grid) : ITilternator
{
    public int AxisLength { get; } = grid[0].Length;

    public int OperableLength { get; } = grid.Length;

    public char this[int operableIndex, int axisIndex /* axis = columns/X for North */]
    {
        get => grid[operableIndex][axisIndex];
        set => grid[operableIndex][axisIndex] = value;
    }
}

public class WestTilternator(StringBuilder[] grid) : ITilternator
{
    public int AxisLength { get; } = grid.Length;

    public int OperableLength { get; } = grid[0].Length;

    public char this[int operableIndex, int axisIndex /* axis = rows/Y for West */]
    {
        get => grid[axisIndex][operableIndex];
        set => grid[axisIndex][operableIndex] = value;
    }
}

public class SouthTilternator(StringBuilder[] grid) : ITilternator
{
    public int AxisLength { get; } = grid[0].Length;

    public int OperableLength { get; } = grid.Length;

    public char this[int operableIndex, int axisIndex /* axis = columns/X for South */]
    {
        get => grid[OperableLength - 1 - operableIndex][axisIndex];
        set => grid[OperableLength - 1 - operableIndex][axisIndex] = value;
    }
}

public class EastTilternator(StringBuilder[] grid) : ITilternator
{
    public int AxisLength { get; } = grid.Length;

    public int OperableLength { get; } = grid[0].Length;

    public char this[int operableIndex, int axisIndex /* axis = rows/Y for East */]
    {
        get => grid[axisIndex][OperableLength - 1 - operableIndex];
        set => grid[axisIndex][OperableLength - 1 - operableIndex] = value;
    }
}

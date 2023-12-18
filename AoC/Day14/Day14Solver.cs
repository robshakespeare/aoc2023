namespace AoC.Day14;

public class Day14Solver : ISolver
{
    public string DayName => "Parabolic Reflector Dish";

    public long? SolvePart1(string input)
    {
        var grid = ParseInput(input);
        ITilternator northTilternator = new NorthTilternator(grid);
        northTilternator.Tilt();
        return CalculateNorthSupportBeamLoad(grid);
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

        const int totalCycles = 1000000000;
        Dictionary<string, int> gridCycleNums = [];
        var warped = false;

        for (var cycleNum = 1; cycleNum <= totalCycles; cycleNum++)
        {
            tilternators.ForEach(tilternator => tilternator.Tilt()); // Cycle (tilt N, W, S, E)

            if (!warped)
            {
                var gridString = string.Join(Environment.NewLine, grid.Select(line => line));

                if (gridCycleNums.TryGetValue(gridString, out var prevMatchingCycleNum))
                {
                    // Engage Warp Speed, Captain!
                    var cycleSize = cycleNum - prevMatchingCycleNum;
                    cycleNum += (totalCycles - cycleNum) / cycleSize * cycleSize;
                    warped = true;
                }
                else
                {
                    gridCycleNums[gridString] = cycleNum;
                }
            }
        }

        return CalculateNorthSupportBeamLoad(grid);
    }

    static StringBuilder[] ParseInput(string input) => input.ReadLines().Select(line => new StringBuilder(line)).ToArray();

    static long CalculateNorthSupportBeamLoad(StringBuilder[] grid) => Enumerable.Range(0, grid.Length).Sum(y =>
    {
        long loadPerRock = grid.Length - y;
        return grid[y].ToString().Count(c => c == 'O') * loadPerRock;
    });
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
                    for (var idx = pos - 1; idx >= 0 && this[idx, axis] == '.'; idx--)
                    {
                        this[idx, axis] = 'O';
                        this[idx + 1, axis] = '.';
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

    public char this[int operableIndex, int axisIndex]
    {
        get => grid[operableIndex][axisIndex];
        set => grid[operableIndex][axisIndex] = value;
    }
}

public class SouthTilternator(StringBuilder[] grid) : ITilternator
{
    public int AxisLength { get; } = grid[0].Length;

    public int OperableLength { get; } = grid.Length;

    public char this[int operableIndex, int axisIndex]
    {
        get => grid[OperableLength - 1 - operableIndex][axisIndex];
        set => grid[OperableLength - 1 - operableIndex][axisIndex] = value;
    }
}

public class WestTilternator(StringBuilder[] grid) : ITilternator
{
    public int AxisLength { get; } = grid.Length;

    public int OperableLength { get; } = grid[0].Length;

    public char this[int operableIndex, int axisIndex]
    {
        get => grid[axisIndex][operableIndex];
        set => grid[axisIndex][operableIndex] = value;
    }
}

public class EastTilternator(StringBuilder[] grid) : ITilternator
{
    public int AxisLength { get; } = grid.Length;

    public int OperableLength { get; } = grid[0].Length;

    public char this[int operableIndex, int axisIndex]
    {
        get => grid[axisIndex][OperableLength - 1 - operableIndex];
        set => grid[axisIndex][OperableLength - 1 - operableIndex] = value;
    }
}

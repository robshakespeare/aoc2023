namespace AoC.Day21;

public class Day21Solver : ISolver
{
    public string DayName => "Step Counter";

    public long? SolvePart1(string input) => SolvePart1(input, 64);

    public long? SolvePart1(string input, int maxSteps)
    {
        var grid = input.ReadLines().ToArray();

        // Find start
        var start = grid.SelectMany((line, y) => line.Select((c, x) => (c, pos: new Vector2(x, y)))).First(i => i.c == 'S').pos;

        // Explore out
        HashSet<Vector2> positions = [start];
        HashSet<Vector2> visited = [];

        HashSet<Vector2> visitedEvenSteps = [];
        HashSet<Vector2> visitedOddSteps = [];

        for (int stepCount = 0; stepCount <= maxSteps; stepCount++)
        {
            HashSet<Vector2> nextPositions = [];

            foreach (var position in positions)
            {
                visited.Add(position);

                var isEvenStep = stepCount % 2 == 0;
                if (isEvenStep)
                {
                    visitedEvenSteps.Add(position);
                }
                else
                {
                    visitedOddSteps.Add(position);
                }

                foreach (var nextPosition in GridUtils.DirectionsExcludingDiagonal.Select(dir => position + dir))
                {
                    if (grid.TryGet(nextPosition, out var nextTile)
                        && nextTile == '.'
                        && !visited.Contains(nextPosition))
                    {
                        nextPositions.Add(nextPosition);
                    }
                }
            }

            positions = nextPositions;
        }

        var gridItems = input.ToGrid((p, c) => new { p, c }).SelectMany(line => line);

        gridItems
            .Concat(visited.Select(p => new { p, c = 'V' }))
            .ToStringGrid(i => i.p, i => i.c, 'X')
            .RenderGridToConsole();

        var isMaxStepsEven = maxSteps % 2 == 0;
        return isMaxStepsEven ? visitedEvenSteps.Count : visitedOddSteps.Count;
    }

    public long? SolvePart2(string input)
    {
        return null;
    }
}

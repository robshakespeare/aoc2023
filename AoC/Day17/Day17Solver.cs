namespace AoC.Day17;

public class Day17Solver : ISolver
{
    public string DayName => "Clumsy Crucible";

    public long? SolvePart1(string input)
    {
        // Dijkstra Search. Node has position, current direction, and up to 2 previous directions; Cost is the heat loss value.
        var grid = input.ReadLines().ToArray();
        var goalPosition = new Vector2(grid[0].Length - 1, grid.Length - 1);
        var search = new AStarSearch<CrucibleNode>(
            getSuccessors: node =>
            {
                var mustTurn = node.DirCount == 3;
                var prevPos = node.Position + (node.Dir * -1);

                return GridUtils.DirectionsExcludingDiagonal // rule: can go any non-diagonal direction
                    .Where(dir => !mustTurn || dir != node.Dir) // rule: at most three blocks in a single direction before it must turn
                    .Select(dir => (pos: node.Position + dir, dir))
                    .Where(x => x.pos != prevPos) // rule: can't go backwards
                    .Select(x => (x.pos, x.dir, heatLoss: grid.TryGet(x.pos, out var c) ? (c - '0') : -1))
                    .Where(x => x.heatLoss > -1)
                    .Select(x => new CrucibleNode(x.pos, x.heatLoss, x.dir, x.dir == node.Dir ? node.DirCount + 1 : 1));
            });

        return search.FindShortestPath(
            starts: [new CrucibleNode(new Vector2(0, 0), default, default, default)],
            isGoal: node => node.Position == goalPosition).TotalCost;
    }

    public long? SolvePart2(string input)
    {
        return null;
    }
}

public sealed record CrucibleNode(Vector2 Position, int Cost, Vector2 Dir, int DirCount) : IAStarSearchNode;

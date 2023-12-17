namespace AoC.Day17;

public class Day17Solver : ISolver
{
    public string DayName => "Clumsy Crucible";

    public long? SolvePart1(string input) => Solve(input, mustTurnAt: 3); // At most three blocks in a single direction before it must turn

    public long? SolvePart2(string input)
    {
        // If we've not yet moved 4 in our current direction, then we must keep moving in that direction
        // At most 10 blocks in a single direction before ultra crucibles must turn
        return Solve(input, mustTurnAt: 10, sailAheadTheshold: 4);
    }

    static long Solve(string input, int mustTurnAt, int sailAheadTheshold = 1)
    {
        // Dijkstra Search. Node has position, current direction, and count of blocks its been heading that direction; Cost is the heat loss value.
        var grid = input.ReadLines().ToArray();
        var goalPosition = new Vector2(grid[0].Length - 1, grid.Length - 1);
        var search = new AStarSearch<CrucibleNode>(
            getSuccessors: node =>
            {
                var directions =
                    (node.Position == Vector2.Zero && node.Cost == 0)
                        ? [GridUtils.East, GridUtils.South] // If we're at the start, then we must go either south, or east.
                        : node.DirCount < sailAheadTheshold
                            ? [node.Dir] // If we've not yet moved X in our current direction, then we must keep moving in that direction
                            : GridUtils.DirectionsExcludingDiagonal; // Can go any non-diagonal direction

                var mustTurn = node.DirCount == mustTurnAt;
                var prevPos = node.Position + (node.Dir * -1);

                return directions
                    .Where(dir => !mustTurn || dir != node.Dir)
                    .Select(dir => (pos: node.Position + dir, dir))
                    .Where(x => x.pos != prevPos) // Can't go backwards
                    .Select(x => (x.pos, x.dir, heatLoss: grid.TryGet(x.pos, out var c) ? (c - '0') : -1))
                    .Where(x => x.heatLoss > -1)
                    .Select(x => new CrucibleNode(x.pos, x.heatLoss, x.dir, x.dir == node.Dir ? node.DirCount + 1 : 1));
            });

        return search.FindShortestPath(
            starts: [new CrucibleNode(Vector2.Zero, default, default, default)],
            isGoal: node => node.Position == goalPosition && node.DirCount >= sailAheadTheshold).TotalCost;
    }

    sealed record CrucibleNode(Vector2 Position, int Cost, Vector2 Dir, int DirCount) : IAStarSearchNode;
}

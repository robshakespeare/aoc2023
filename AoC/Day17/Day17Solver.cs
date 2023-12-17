namespace AoC.Day17;

public class Day17Solver : ISolver
{
    public string DayName => "Clumsy Crucible";

    public long? SolvePart1(string input)
    {
        // Thinking A* Search, try Dijkstra Search first
        // Node has position, current direction, and up to 2 previous directions
        // Cost is the heat loss value, i.e the number of the cell corresponding to the position

        var grid = input.ReadLines().ToArray();
        var goalPosition = new Vector2(grid[0].Length - 1, grid.Length - 1);

        var search = new AStarSearch<CrucibleNode>(
            getSuccessors: node =>
            {
                // Can go any direction, excluding backwards, and also at most three blocks in a single direction before it must turn

                var mustTurn = node.Dir == node.Dir1 && node.Dir == node.Dir2;
                var prevPos = node.Position + (node.Dir * -1);

                return GridUtils.DirectionsExcludingDiagonal
                    .Where(dir => !mustTurn || dir != node.Dir) // rule: at most three blocks in a single direction before it must turn
                    .Select(dir => (pos: node.Position + dir, dir))
                    .Where(x => x.pos != prevPos) // rule: can't go backwards
                    .Select(x => (x.pos, x.dir, heatLoss: grid.TryGet(x.pos, out var c) ? (c - '0') : -1))
                    .Where(x => x.heatLoss > -1)
                    .Select(x => new CrucibleNode(x.pos, x.heatLoss, x.dir, node.Dir, node.Dir1));

                //IEnumerable<Vector2> GetDirections()
                //{
                //    yield return node;
                //}

                //yield return node;

                //if (node.Dir != null && node.Dir == node.Dir1 && node.Dir == node.Dir2)
                //{

                //}
            },
            node => MathUtils.ManhattanDistance(node.Position, goalPosition));

        var path = search.FindShortestPath(
            start: new CrucibleNode(new Vector2(0, 0), default, default, default, default),
            goal: new CrucibleNode(goalPosition, default, default, default, default));

        return path.TotalCost;
    }

    public long? SolvePart2(string input)
    {
        return null;
    }

    //static IEnumerable<CrucibleNode> GetSuccessors(CrucibleNode node)
    //{

    //}
}

public sealed record CrucibleNode(Vector2 Position, int HeatLoss, Vector2 Dir, Vector2 Dir1, Vector2 Dir2) : IAStarSearchNode
{
    public int Cost => HeatLoss;

    public bool Equals(CrucibleNode? other) => other != null && other.Position == Position;

    public override int GetHashCode() => Position.GetHashCode();
}

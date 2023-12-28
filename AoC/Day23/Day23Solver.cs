namespace AoC.Day23;

public class Day23Solver : ISolver
{
    public string DayName => "A Long Walk";

    const char PathTile = '.';
    const char ForestTile = '#';

    public long? SolvePart1(string input)
    {
        var (start, end, _) = ParseInputAndBuildGraph(input);

        return FindLongestPath(start, end);
    }

    public long? SolvePart2(string input)
    {
        var (start, end, nodes) = ParseInputAndBuildGraph(input);

        // Build our "return" edges
        foreach (var edge in nodes.SelectMany(node => node.Edges))
        {
            var newEdge = new Edge(edge.EdgeId, edge.End, edge.Start, edge.Path.Reverse().ToArray());
            newEdge.Start.Edges.Add(newEdge);
        }

        return FindLongestPath(start, end);
    }

    static long FindLongestPath(Node start, Node end)
    {
        var explore = new PriorityQueue<(Node Node, int[] Path, int PathHash, long TotalCost), long>(new[] { ((start, Array.Empty<int>(), 0, 0L), 0L) });
        var seen = new HashSet<int>(); // Note each `int` is the hash of the path seen
        var maxPath = 0L;
        var numPaths = 0L;

        var interval = TimeSpan.FromSeconds(5);
        var nextTickTime = DateTime.Now + interval;
        var startTime = DateTime.Now;
        string Status() => $"explorers: {explore.Count} // paths: {numPaths} // maxPath: {maxPath} // elapsed: {DateTime.Now - startTime}";

        while (explore.Count > 0)
        {
            var (node, path, pathHash, totalCost) = explore.Dequeue();

            if (node == end)
            {
                numPaths++;
                if (totalCost > maxPath)
                {
                    maxPath = totalCost;
                    Console.WriteLine($"NEW MAX PATH ** {Status()}");
                }
                else if ((float)totalCost / maxPath < 0.25)
                {
                    return maxPath; // Early exit optimisation, because of the priority queue, if we're starting to see paths which are 25% or less of the current max path, then we've seen our longest path and there's no need to continue searching
                }
            }
            else
            {
                if (!seen.Contains(pathHash))
                {
                    foreach (var newEdge in node.Edges)
                    {
                        if (!path.Contains(newEdge.End.NodeId))
                        {
                            var newTotalCost = totalCost + newEdge.Length;
                            explore.Enqueue((newEdge.End, [.. path, newEdge.End.NodeId], HashCode.Combine(pathHash, newEdge.End.NodeId), newTotalCost), -newTotalCost);
                        }
                    }

                    seen.Add(pathHash);
                }
            }

            if (DateTime.Now > nextTickTime)
            {
                Console.WriteLine(Status());
                nextTickTime = DateTime.Now + interval;
            }
        }

        return maxPath;
    }

    public static Graph ParseInputAndBuildGraph(string input)
    {
        var grid = input.ReadLines().ToArray();

        var nextEdgeId = 0;
        void AddEdge(Explorer explorer, Node nextNode) => explorer.Node.Edges.Add(new Edge(nextEdgeId++, explorer.Node, nextNode, explorer.Path));

        bool IsNodePosition(Vector2 position) => GridUtils.DirectionsExcludingDiagonal.Count(dir => grid.TryGet(position + dir, out var tile) && tile is '>' or '<' or 'v') > 1;

        Dictionary<Vector2, Node> nodes = [];
        Node AddNode(Node node)
        {
            nodes.Add(node.Position, node);
            return node;
        }

        var nextNodeId = 0;
        var start = AddNode(new Node(nextNodeId++, new Vector2(grid[0].IndexOf(PathTile), 0)));
        var end = AddNode(new Node(nextNodeId++, new Vector2(grid[^1].IndexOf(PathTile), grid.Length - 1)));

        List<Explorer> explorers = [new Explorer(start.Position, GridUtils.South, [], start)];

        while (explorers.Count > 0)
        {
            List<Explorer> newExplorers = [];

            foreach (var nextExplorer in explorers.Select(explorer => explorer.Next()))
            {
                if (nextExplorer.Position == end.Position)
                {
                    AddEdge(nextExplorer, end);
                    continue;
                }

                var nextPositions = GridUtils.DirectionsExcludingDiagonal.Select(dir => (dir, nextPos: nextExplorer.Position + dir))
                    .Where(n => n.nextPos.Y > 0 && !nextExplorer.Path.Contains(n.nextPos))
                    .Select(n => (n.nextPos, n.dir, tile: grid.Get(n.nextPos)))
                    .Where(n => n.tile == PathTile || (n.tile != ForestTile && SlopeToDir(n.tile) == n.dir))
                    .ToArray();

                if (nextPositions.Length == 0)
                {
                    throw new Exception("No next positions");
                }

                if (nextPositions.Length > 1 || IsNodePosition(nextExplorer.Position))
                {
                    var nodeAlreadySeen = true;

                    if (!nodes.TryGetValue(nextExplorer.Position, out var node))
                    {
                        node = AddNode(new Node(nextNodeId++, nextExplorer.Position)); // i.e. new node
                        nodeAlreadySeen = false;
                    }

                    AddEdge(nextExplorer, node);

                    if (!nodeAlreadySeen)
                    {
                        foreach (var nextPosition in nextPositions)
                        {
                            newExplorers.Add(new Explorer(nextPosition.nextPos, nextPosition.dir, [nextPosition.nextPos], node));
                        }
                    }
                }
                else
                {
                    newExplorers.Add(nextExplorer with { Direction = nextPositions.Single().dir }); // Our explorer might need to change direction
                }
            }

            explorers = newExplorers;
        }

        return new Graph(start, end, nodes.Values.ToArray());
    }

    static Vector2 SlopeToDir(char slope) => slope switch
    {
        '>' => GridUtils.East,
        '<' => GridUtils.West,
        'v' => GridUtils.South,
        _ => throw new Exception("Invalid slope char: " + slope)
    };

    record Explorer(Vector2 Position, Vector2 Direction, Vector2[] Path, Node Node)
    {
        public Explorer Next()
        {
            var nextPos = Position + Direction;
            return this with
            {
                Position = nextPos,
                Path = [.. Path, nextPos]
            };
        }
    }
}

public record Graph(Node Start, Node End, Node[] Nodes);

public sealed record Node(int NodeId, Vector2 Position)
{
    public bool Equals(Node? other) => other != null && other.NodeId == NodeId;

    public override int GetHashCode() => NodeId.GetHashCode();

    public HashSet<Edge> Edges { get; } = [];
}

public sealed record Edge(int EdgeId, Node Start, Node End, Vector2[] Path)
{
    public bool Equals(Edge? other) => other != null && other.EdgeId == EdgeId;

    public override int GetHashCode() => EdgeId.GetHashCode();

    public int Length => Path.Length;
}

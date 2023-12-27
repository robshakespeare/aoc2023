namespace AoC.Day23;

public class Day23Solver : ISolver
{
    public string DayName => "A Long Walk";

    const char PathTile = '.';
    const char ForestTile = '#';

    public long? SolvePart1(string input)
    {
        var (start, end, _) = ParseInputAndBuildGraph(input);

        var path = FindLongestPath(start, end);

        return path.TotalCost;
    }

    public long? SolvePart2(string input) => ParseInputAndFindLongestSteepHillHike(input).Count;

    static IReadOnlyCollection<Vector2> ParseInputAndFindLongestSteepHillHike(string input)
    {
        var grid = input.ReadLines().ToArray();
        var start = new Vector2(grid[0].IndexOf(PathTile), 0);
        var end = new Vector2(grid[^1].IndexOf(PathTile), grid.Length - 1);

        var explorers = new List<SteepSlopeExplorer> { new(start, []) };

        List<HashSet<Vector2>> paths = [];

        var maxPath = 0;

        while (explorers.Count > 0)
        {
            var newExplorers = new List<SteepSlopeExplorer>();

            foreach (var (pos, visited) in explorers)
            {
                var isNew = visited.Add(pos);

                if (isNew)
                {
                    if (pos == end)
                    {
                        paths.Add(visited);

                        maxPath = Math.Max(visited.Count, maxPath);
                    }
                    else
                    {
                        var nextPositions = GridUtils.DirectionsExcludingDiagonal.Select(dir => (dir, nextPos: pos + dir))
                            .Where(n => n.nextPos.Y > 0 && !visited.Contains(n.nextPos))
                            .Select(n => (n.nextPos, n.dir, tile: grid.Get(n.nextPos)))
                            .Where(n => n.tile != ForestTile)
                            .Select(n => n.nextPos);

                        foreach (var nextPosition in nextPositions)
                        {
                            newExplorers.Add(new SteepSlopeExplorer(nextPosition, [.. visited]));
                        }
                    }
                }
            }

            explorers = newExplorers;

            Console.WriteLine($"explorers: {explorers.Count} // paths: {paths.Count} // maxPath: {maxPath}");
        }

        return paths.MaxBy(path => path.Count) ?? throw new Exception("No paths found");
    }

    record SteepSlopeExplorer(Vector2 Pos, HashSet<Vector2> Visited);

    static (Edge[], long TotalCost) FindLongestPath(Node start, Node end)
    {
        var explore = new PriorityQueue<(Node Node, Edge[] Path, long TotalCost), long>(new[] { ((start, Array.Empty<Edge>(), 0L), 0L) });

        List<(Edge[], long TotalCost)> paths = [];

        while (explore.Count > 0)
        {
            var (node, edges, totalCost) = explore.Dequeue();

            if (node == end)
            {
                paths.Add((edges, totalCost));
            }
            else
            {
                foreach (var newEdge in node.Edges)
                {
                    var newTotalCost = totalCost + newEdge.Length;
                    explore.Enqueue((newEdge.End, [.. edges, newEdge], newTotalCost), newTotalCost);
                }
            }
        }

        return paths.MaxBy(path => path.TotalCost);
    }

    public static (Node Start, Node End, Node[] Nodes) ParseInputAndBuildGraph(string input)
    {
        var grid = input.ReadLines().ToArray();

        static void AddEdge(Explorer explorer, Node nextNode) => explorer.Node.Edges.Add(new Edge(explorer.Node, nextNode, explorer.Path));

        bool IsNodePosition(Vector2 position) => GridUtils.DirectionsExcludingDiagonal.Count(dir => grid.TryGet(position + dir, out var tile) && tile is '>' or '<' or 'v') > 1;

        Dictionary<Vector2, Node> nodes = [];
        Node AddNode(Node node)
        {
            nodes.Add(node.Position, node);
            return node;
        }

        var start = AddNode(new Node(new Vector2(grid[0].IndexOf(PathTile), 0)));
        var end = AddNode(new Node(new Vector2(grid[^1].IndexOf(PathTile), grid.Length - 1)));

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
                        node = AddNode(new Node(nextExplorer.Position)); // i.e. new node
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

        return (start, end, nodes.Values.ToArray());
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

public sealed record Node(Vector2 Position)
{
    public bool Equals(Node? other) => other != null && other.Position == Position;

    public override int GetHashCode() => Position.GetHashCode();

    public List<Edge> Edges { get; } = [];
}

public sealed record Edge(Node Start, Node End, Vector2[] Path)
{
    public int Length => Path.Length;
}

using System.Diagnostics;
using Spectre.Console;

namespace AoC.Day23;

public class Day23Solver : ISolver
{
    public string DayName => "A Long Walk";

    const char PathTile = '.';
    const char ForestTile = '#';

    public long? SolvePart1(string input)
    {
        var (start, end, nodes) = ParseInputAndBuildGraph(input);

        //var search = new AStarSearch<Edge>(edge => edge.End.ChildEdges);

        ////var found = 0;

        //foreach (var node in nodes)
        //{
        //    Console.WriteLine(node);
        //}

        //Console.WriteLine();

        //var path = search.FindShortestPath(start.ChildEdges, edge => edge.End == end);

        var path = FindLongestPath(start, end);

        return path.Sum(x => x.Length);
    }

    public long? SolvePart2(string input)
    {
        return null;
    }

    static Edge[] FindLongestPath(Node start, Node end)
    {
        var explore = new PriorityQueue<(Node Node, Edge[] Path, long TotalCost), long>(new[] { ((start, Array.Empty<Edge>(), 0L), 0L) });

        List<Edge[]> paths = [];

        while (explore.Count > 0)
        {
            var (node, edges, totalCost) = explore.Dequeue();

            if (node == end)
            {
                paths.Add(edges);
                //return edges;
            }
            else
            {
                foreach (var newEdge in node.ChildEdges)
                {
                    var newTotalCost = totalCost - newEdge.Length;
                    explore.Enqueue((newEdge.End, [.. edges, newEdge], newTotalCost), newTotalCost);
                }
            }
        }

        return paths.MaxBy(path => path.Sum(x => x.Length));
    }

    public static (Node Start, Node End, Node[] Nodes) ParseInputAndBuildGraph(string input)
    {
        var grid = input.ReadLines().ToArray();
        //var height = grid.Length;
        //var width = grid[0].Length;

        //var start = new Node(new Vector2(grid[0].IndexOf(PathTile)));
        //var end = new Node(new Vector2(grid[^1].IndexOf(PathTile)));

        static void AddEdge(Explorer explorer, Node nextNode) => explorer.Node.ChildEdges.Add(new Edge(explorer.Node, nextNode, explorer.Path));

        bool IsNodePosition(Vector2 position) => GridUtils.DirectionsExcludingDiagonal.Count(dir => grid.TryGet(position + dir, out var tile) && tile is '>' or '<' or 'v') > 1;

        //var nodesList = new List<Node>();

        Dictionary<Vector2, Node> nodes = [];
        Node AddNode(Node node)
        {
            //nodesList.Add(node);
            nodes.Add(node.Position, node);
            return node;
        }

        var start = AddNode(new Node(new Vector2(grid[0].IndexOf(PathTile), 0)));
        var end = AddNode(new Node(new Vector2(grid[^1].IndexOf(PathTile), grid.Length - 1)));

        //HashSet<Vector2> visited = [start.Position];

        List<Explorer> explorers = [new Explorer(start.Position, GridUtils.South, [], start)];

        var timeStarted = Stopwatch.StartNew();

        while (explorers.Count > 0)
        {
            List<Explorer> newExplorers = [];

            foreach (var nextExplorer in explorers.Select(explorer => explorer.Next()))
            {
                //var nextExplorer = explorer.Next();
                //if (visited.Contains(nextExplorer.Position))
                //{
                //    continue;
                //}

                //visited.Add(nextExplorer.Position);

                //Console.WriteLine(nextExplorer.Position);

                //if (timeStarted.Elapsed > TimeSpan.FromSeconds(1))
                //{
                //    throw new Exception("wtf!");
                //}

                if (nextExplorer.Position == end.Position)
                {
                    AddEdge(nextExplorer, end);
                    continue;
                }

                var nextPositions = GridUtils.DirectionsExcludingDiagonal.Select(dir => (dir, nextPos: nextExplorer.Position + dir))
                    .Where(n => n.nextPos.Y > 0 && !nextExplorer.Path.Contains(n.nextPos)) //!visited.Contains(n.nextPos))
                    .Select(n => (n.nextPos, n.dir, tile: grid.Get(n.nextPos)))
                    .Where(n => n.tile == PathTile || (n.tile != ForestTile && SlopeToDir(n.tile) == n.dir))
                    .ToArray();

                //var isNode = IsNodePosition(nextExplorer.Position);

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
                        //AddEdge(nextExplorer, node); // i.e. node already seen
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
                ////else if (nodes.TryGetValue(nextExplorer.Position, out var nodeAlreadySeen))
                ////{
                ////    AddEdge(nextExplorer, nodeAlreadySeen);
                ////}
                //else if (nextPositions.Length == 1)
                else
                {
                    newExplorers.Add(nextExplorer with { Direction = nextPositions.Single().dir }); // Our explorer might need to change direction
                }
                //else
                //{
                    
                //}

                // Get next possible positions
                // If there are more than one, then we have reached a "node"
                // So record that node, and work out the next possible routes for new edges (not re-visiting, and going only the correct directions based on slopes)
                // Otherwise, check whether we've reached the end
                // Otherwise, continue on our way


                //var nextPositions = GridUtils.DirectionsExcludingDiagonal.Select(dir => (dir, nextPos: explorer.Position + dir))
                //    .Where(n => n.nextPos.Y > 0)
                //    .Select(n => (n.nextPos, n.dir, tile: grid.Get(n.nextPos)))
                //    // rs-todo: does all the advanced filtering etc... stuff!!
                //    .Select(n => n.nextPos)
                //    .ToArray();
                ////var isNode = canidateNextPositions.Length > 2;

                //if (nextPositions.Length == 0)
                //{
                //    throw new Exception("No next positions");
                //}

                //foreach (var nextPosition in nextPositions)
                //{
                //    if (nodes.TryGetValue(nextPosition, out var nextNode))
                //    {
                //        explorer.Node.ChildEdges.Add(new Edge(explorer.Node, nextNode, [.. explorer.Path, nextPosition]));

                //        if (nextNode != end)
                //        {
                //            // rs-todo continue exploering!
                //        }
                //    }
                //}

                //if (isNode)
                //{
                //}
                //else
                //{
                //    var nextPosition = canidateNextPositions.Single(n => !visited.Contains(n.nextPos)).nextPos;
                //    var nextExplorer = new Explorer(nextPosition, [.. explorer.Path, nextPosition], explorer.Node);

                //    if (nextPosition == end.Position)
                //    {

                //    }
                //}


                //var canidateNextPositions = GridUtils.DirectionsExcludingDiagonal.Select(dir => (dir, nextPos: explorer.Position + dir))
                //    .Where(n => n.nextPos.Y > 0)
                //    .Select(n => (n.nextPos, n.dir, tile: grid.Get(n.nextPos)))
                //    .ToArray();
                //var isNode = canidateNextPositions.Length > 2;

                //if (isNode)
                //{
                //}
                //else
                //{
                //    var nextPosition = canidateNextPositions.Single(n => !visited.Contains(n.nextPos)).nextPos;
                //    var nextExplorer = new Explorer(nextPosition, [.. explorer.Path, nextPosition], explorer.Node);

                //    if (nextPosition == end.Position)
                //    {

                //    }
                //}
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

    public sealed record Node(Vector2 Position)
    {
        public bool Equals(Node? other) => other != null && other.Position == Position;

        public override int GetHashCode() => Position.GetHashCode();

        public List<Edge> ChildEdges { get; } = [];

        public int EdgeCount => ChildEdges.Count;

        public override string ToString() => $"{Position}, children: {ChildEdges.Count}{Environment.NewLine}Edges:{Environment.NewLine}{string.Join(Environment.NewLine, ChildEdges)}{Environment.NewLine}";
    }

    public sealed record Edge(Node Start, Node End, Vector2[] Path) /*: IAStarSearchNode*/
    {
        //public int Cost => -Path.Length;

        public int Length => Path.Length;

        public string Id { get; } = $"Len: {Path.Length} // {Start.Position}|{End.Position}__{string.Join(":", Path)}";

        public bool Equals(Edge? other) => other != null && other.Id == Id;

        public override int GetHashCode() => Id.GetHashCode();

        public override string ToString() => Id;
    }
}

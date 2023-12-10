using System.Linq;

namespace AoC.Day10;

public class Day10Solver : ISolver
{
    public string DayName => "Pipe Maze";

    public long? SolvePart1(string input) => ParseInput(input).PipeStarts.Select(Explore).Select(path => path.Length).Max() / 2;

    public long? SolvePart2(string input)
    {
        var (grid, explorers) = ParseInput(input);

        var loop = Explore(explorers.First());

        const int padding = 1;
        var loopMap = loop.ToStringGrid(x => x.Tile.Position, x => x.Tile.Char, '.', padding).RenderGridToString();

        var offset = new Vector2(loop.Min(i => i.Tile.Position.X) - padding, loop.Min(i => i.Tile.Position.Y) - padding);
        offset.ToString().Dump("OFFSET"); // debug

        var loopGrid = loopMap.ToGrid((pos, chr) => new { Position = pos + offset, Char = chr });
            //.SelectMany(line => line)
            //.ToDictionary(x => x.Position);

        // debug:
        //loop.ToStringGrid(x => x.Tile.Position, x => x.Tile.Char, '.', padding).RenderGridToConsole();
        loopGrid.SelectMany(x => x).ToStringGrid(x => x.Position, x => x.Char, '.').RenderGridToConsole(); // debug

        // Start in top left, and "fill out", squeezing down any gaps
        // Our remaining dots are the ones contained within the loop

        var visited = new HashSet<Vector2>();

        var explore = new Queue<Vector2>([loopGrid[0][0].Position]);

        var pipeTileLookup = loop.ToFrozenDictionary(x => x.Tile.Position);

        //bool InBounds(Vector2 position) => loopGrid.SafeGet(position) != null;

        var edgeNudges = GridUtils.DirectionsExcludingDiagonal.Select(x => x * 0.5f).ToArray();

        edgeNudges.Select(x => x.ToString()).Dump("edgeNudges"); // Debug

        while (explore.Count > 0)
        {
            var currentPosition = explore.Dequeue();
            //visited.Add(currentPos);

            var candidateLocations = GridUtils.DirectionsIncludingDiagonal.Select(dir => currentPosition + dir /*- offset*/)
                .Where(candidatePosition => !visited.Contains(candidatePosition))
                //.Where(InBounds)
                .Select(candidatePosition => loopGrid.SafeGet(candidatePosition - offset)) // Note -offset to go from world to grid space
                .Where(candidateLocation => candidateLocation != null)
                .Select(candidateLocation => candidateLocation!);

            foreach (var candidateLocation in candidateLocations)
            {
                var candidatePosition = candidateLocation.Position;
                var visit = false;

                if (candidateLocation.Char == '.')
                {
                    visit = true;
                }
                // else its a pipe, so we might be able to squeeze through
                else
                {
                    var pipeTile = pipeTileLookup[candidatePosition];

                    // We can squeeze through either of the pipe tile's 2 edges that join with our shared edge
                    // As long as the way through isn't blocked by a connection
                    var sharedEdge = edgeNudges.Select(x => (Vector2?)currentPosition + x)
                        .Intersect(edgeNudges.Select(x => (Vector2?)pipeTile.Tile.Position + x))
                        .Cast<Vector2?>()
                        .SingleOrDefault();

                    if (sharedEdge != null)
                    {
                        var ourDir = Vector2.Normalize(sharedEdge.Value - currentPosition);
                        var sharedEdgeDir = new Vector2(ourDir.Y, -ourDir.X);

                        var candidateEdges = new[] { 0.5f, -0.5f }.Select(nudge => currentPosition + (sharedEdgeDir * nudge) + ourDir);

                        var test = candidateEdges.Except(pipeTile.Tile.Connections);

                        if (test.Any())
                        {
                        }
                    }
                    //else
                    //{
                    //}

                    //var keh1 = edgeNudges.Select(x => currentPosition + x).ToArray();
                    //var keh2 = edgeNudges.Select(x => pipeTile.Tile.Position + x).ToArray();

                    //if (true)
                    //{
                    //}

                    //var sharedEdge = edgeNudges.Select(x => (Vector2?)currentPosition + x)
                    //    .Intersect(edgeNudges.Select(x => (Vector2?)pipeTile.Tile.Position + x))

                    //if (sharedEdge.Length > 1)
                    //{
                    //}

                    //var pipeTileCorners = GridUtils.DirectionsExcludingDiagonal;
                }

                if (visit)
                {
                    

                    if (!visited.Contains(candidatePosition))
                    {
                        explore.Enqueue(candidatePosition);
                        visited.Add(candidatePosition);
                    }
                }
            }
        }

        var finalGrid = visited.Select(pos => new { pos, chr = 'O' })
            .Concat(loop.Select(x => new { pos = x.Tile.Position, chr = ' ' }))
            .ToStringGrid(x => x.pos, x => x.chr, 'I')
            .RenderGridToString();

        Console.WriteLine(finalGrid); // debug:
        Console.WriteLine();

        return finalGrid.Count(c => c == 'I');
    }

    static PipeTile[] Explore(PipeTile pipeTile)
    {
        var path = new List<PipeTile>();

        do
        {
            path.Add(pipeTile);

            var nextPos = pipeTile.Tile.Position + pipeTile.Direction;
            var nextTile = pipeTile.Grid.Get(nextPos);
            if (nextTile.Connections.Length != 2)
            {
                throw new Exception("Next tile should always have only 2 connections");
            }

            var connection = nextTile.Connections.Single(c => c == pipeTile.Connection);
            var nextConnection = nextTile.Connections.Single(c => c != connection);
            var nextDirection = Vector2.Normalize(nextConnection - nextTile.Position);

            pipeTile = new PipeTile(nextTile, nextConnection, nextDirection, pipeTile.Grid);
        }
        while (pipeTile.Tile.Char != 'S');

        return [.. path];
    }

    static (Tile[][] Grid, PipeTile[] PipeStarts) ParseInput(string input)
    {
        Tile? start = null;

        var grid = input.ToGrid((pos, chr) =>
        {
            var tile = Tile.Create(pos, chr);
            if (chr == 'S')
            {
                start = tile;
            }
            return tile;
        });

        if (start == null)
        {
            throw new Exception("Start not found");
        }

        // Fix start's connections
        var candidateOurConnections = GridUtils.DirectionsExcludingDiagonal.Select(dir => start.Position + (dir * 0.5f));
        var candidateOtherConnections = GridUtils.DirectionsExcludingDiagonal
            .Select(dir => start.Position + dir)
            .Select(pos => grid.SafeGet(pos))
            .Where(tile => tile != null)
            .SelectMany(tile => tile!.Connections);

        var startConnections = candidateOtherConnections.Intersect(candidateOurConnections).ToArray();
        if (startConnections.Length != 2)
        {
            throw new Exception("Start should always have 2 connections");
        }

        start = start with { Connections = startConnections };
        grid[(int)start.Position.Y][(int)start.Position.X] = start;

        // Get the pipe connections of the start tile
        var pipeStarts = start.Connections.Select(c => new PipeTile(start, c, Vector2.Normalize(c - start.Position), grid)).ToArray();

        return (grid, pipeStarts);
    }

    record struct PipeTile(Tile Tile, Vector2 Connection, Vector2 Direction, Tile[][] Grid); // rs-todo: rename to LoopTile!

    record Tile(Vector2 Position, char Char, Vector2[] Connections)
    {
        public static Tile Create(Vector2 position, char chr)
        {
            var (x, y) = (position.X, position.Y);

            var north = new Vector2(x, y - 0.5f);
            var east = new Vector2(x + 0.5f, y);
            var south = new Vector2(x, y + 0.5f);
            var west = new Vector2(x - 0.5f, y);

            Vector2[] connections = chr switch
            {
                '|' => [north, south],
                '-' => [east, west],
                'L' => [north, east],
                'J' => [north, west],
                '7' => [south, west],
                'F' => [south, east],
                '.' or 'S' => [],
                _ => throw new Exception($"Invalid tile char {chr} at position {position}")
            };
            return new Tile(position, chr, connections);
        }
    }
}

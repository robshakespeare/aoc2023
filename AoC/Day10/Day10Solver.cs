namespace AoC.Day10;

public class Day10Solver : ISolver
{
    public string DayName => "Pipe Maze";

    public long? SolvePart1(string input) => ParseInput(input).LoopStarts.Select(Explore).Select(path => path.Length).Max() / 2;

    public long? SolvePart2(string input)
    {
        var (grid, loopStarts) = ParseInput(input);

        var loop = Explore(loopStarts.First());
        var doubleResGrid = BuildDoubleResolutionGrid(loop);

        // Start in top left, and "fill out". Our remaining dots are the ones contained within the loop.
        var visited = new HashSet<Vector2>();
        var explore = new Queue<Vector2>([doubleResGrid[0][0].Position]);

        while (explore.Count > 0)
        {
            var currentPosition = explore.Dequeue();

            var candidateLocations = GridUtils.DirectionsIncludingDiagonal.Select(dir => currentPosition + dir)
                .Where(candidatePosition => !visited.Contains(candidatePosition))
                .Select(candidatePosition => doubleResGrid.SafeGet(candidatePosition))
                .Where(candidateLocation => candidateLocation != null)
                .Select(candidateLocation => candidateLocation!);

            foreach (var candidateLocation in candidateLocations)
            {
                var candidatePosition = candidateLocation.Position;

                if (candidateLocation.Char == '.')
                {
                    if (!visited.Contains(candidatePosition))
                    {
                        explore.Enqueue(candidatePosition);
                        visited.Add(candidatePosition);
                    }
                }
            }
        }

        var doubleResGridFinal = doubleResGrid.SelectMany(x => x)
            .Concat(visited.Select(pos => new BasicTile(pos, 'O')))
            .ToStringGrid(x => x.Position, x => x.Char, 'X');

        var finalGridBuilder = new StringBuilder();

        for (int y = 1; y < doubleResGridFinal.Count; y += 2)
        {
            for (int x = 1; x < doubleResGridFinal[y].Length; x += 2)
            {
                finalGridBuilder.Append(doubleResGridFinal[y][x]);
            }

            finalGridBuilder.AppendLine();
        }

        var finalGrid = finalGridBuilder.ToString();

        Console.WriteLine(finalGrid);
        Console.WriteLine();

        return finalGrid.ToString().Count(c => c == '.');
    }

    static BasicTile[][] BuildDoubleResolutionGrid(LoopTile[] loop)
    {
        var positions = loop.Select(x => x.Tile.Position);
        var padBounds = new Vector2(0.5f);
        var minBounds = new Vector2(positions.Min(i => i.X), positions.Min(i => i.Y));
        var maxBounds = new Vector2(positions.Max(i => i.X), positions.Max(i => i.Y));
        var minBoundsPadded = minBounds - padBounds;
        var maxBoundsPadded = maxBounds + padBounds;

        var map = new Dictionary<Vector2, char>();

        for (var y = minBoundsPadded.Y; y <= maxBoundsPadded.Y; y += 0.5f)
        {
            for (var x = minBoundsPadded.X; x <= maxBoundsPadded.X; x += 0.5f)
            {
                map[new Vector2(x, y)] = '.';
            }
        }

        foreach (var pipePosition in loop.SelectMany(item => item.Tile.Positions))
        {
            map[pipePosition] = '#';
        }

        var doubleResMap = map.ToStringGrid(x => x.Key * 2, x => x.Value, 'X').RenderGridToString();
        var doubleResGrid = doubleResMap.ToGrid((pos, chr) => new BasicTile(pos, chr));

        return doubleResGrid;
    }

    record BasicTile(Vector2 Position, char Char);

    static LoopTile[] Explore(LoopTile loopTile)
    {
        var path = new List<LoopTile>();

        do
        {
            path.Add(loopTile);

            var nextPos = loopTile.Tile.Position + loopTile.Direction;
            var nextTile = loopTile.Grid.Get(nextPos);
            if (nextTile.Connections.Length != 2)
            {
                throw new Exception("Next tile should always have only 2 connections");
            }

            var connection = nextTile.Connections.Single(c => c == loopTile.Connection);
            var nextConnection = nextTile.Connections.Single(c => c != connection);
            var nextDirection = Vector2.Normalize(nextConnection - nextTile.Position);

            loopTile = new LoopTile(nextTile, nextConnection, nextDirection, loopTile.Grid);
        }
        while (loopTile.Tile.Char != 'S');

        return [.. path];
    }

    static (Tile[][] Grid, LoopTile[] LoopStarts) ParseInput(string input)
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
        var loopStarts = start.Connections.Select(connection => new LoopTile(start, connection, Vector2.Normalize(connection - start.Position), grid)).ToArray();

        return (grid, loopStarts);
    }

    record struct LoopTile(Tile Tile, Vector2 Connection, Vector2 Direction, Tile[][] Grid);

    record Tile(Vector2 Position, char Char, Vector2[] Connections)
    {
        public Vector2[] Positions { get; } = [Position, .. Connections];

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

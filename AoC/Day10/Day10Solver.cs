namespace AoC.Day10;

public class Day10Solver : ISolver
{
    public string DayName => "Pipe Maze";

    public long? SolvePart1(string input) => ParseInput(input).PipeStarts.Select(Explore).Select(path => path.Length).Max() / 2;

    public long? SolvePart2(string input)
    {
        var (grid, explorers) = ParseInput(input);

        var loop = Explore(explorers.First());

        loop.ToStringGrid(x => x.Tile.Position, x => x.Tile.Char, '.', padding: 1).RenderGridToConsole();

        return null;
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

    record struct PipeTile(Tile Tile, Vector2 Connection, Vector2 Direction, Tile[][] Grid);

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

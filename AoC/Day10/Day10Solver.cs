namespace AoC.Day10;

public class Day10Solver : ISolver
{
    public string DayName => "Pipe Maze";

    public long? SolvePart1(string input) => ParseInput(input).Explorers.Select(Explore).Select(path => path.Length).Max() / 2;

    public long? SolvePart2(string input)
    {
        var (_, grid, explorers) = ParseInput(input);

        var loop = Explore(explorers.First());

        return null;
    }

    static Tile[] Explore(Explorer explorer)
    {
        var path = new List<Tile>();

        do
        {
            path.Add(explorer.CurrentTile);

            var nextPos = explorer.CurrentTile.Position + explorer.Direction;
            var nextTile = explorer.Grid.Get(nextPos);
            if (nextTile.Connections.Length != 2)
            {
                throw new Exception("Next tile should always have only 2 connections");
            }

            var connection = nextTile.Connections.Single(c => c == explorer.NextConnection);
            var nextConnection = nextTile.Connections.Single(c => c != connection);
            var nextDirection = Vector2.Normalize(nextConnection - nextTile.Position);

            explorer = new Explorer(nextTile, nextConnection, nextDirection, explorer.Grid);
        }
        while (explorer.CurrentTile.Char != 'S');

        return path.ToArray();
    }

    static (Tile Start, Tile[][] Grid, Explorer[] Explorers) ParseInput(string input)
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

        // Get our explorers
        var explorers = start.Connections.Select(c => new Explorer(start, c, Vector2.Normalize(c - start.Position), grid)).ToArray();

        return (start, grid, explorers);
    }

    record struct Explorer(Tile CurrentTile, Vector2 NextConnection, Vector2 Direction, Tile[][] Grid);

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

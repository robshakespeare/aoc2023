using System.Linq;

namespace AoC.Day10;

public class Day10Solver : ISolver
{
    public string DayName => "Pipe Maze";

    public long? SolvePart1(string input)
    {
        var (start, grid) = ParseInput(input);

        //Console.WriteLine("grid:");
        //grid.SelectMany(x => x).ToStringGrid(x => x.Position, x => x.Char, '.').RenderGridToConsole(); // rs-todo: need an extension for this

        Console.WriteLine("start:");
        Console.WriteLine(start); // rs-todo: do need my own dump method! One for the todo list

        //start.Position.ToString().Dump();
        Console.WriteLine("start cons:");
        start.Connections.ElementAt(0).ToString().Dump();
        start.Connections.ElementAt(1).ToString().Dump();

        // Explore!
        //var directions = start.Connections.Select(c => Vector2.Normalize(c - start.Position)).ToArray();
        //var explorers = directions.Select(dir => new Explorer(start, dir, grid));

        var explorers = start.Connections.Select(c => new Explorer(start, c, Vector2.Normalize(c - start.Position), grid)).ToArray();
        var directions = explorers.Select(x => x.Direction).ToArray();

        Console.WriteLine("start dirs:");
        directions[0].ToString().Dump();
        directions[1].ToString().Dump();

        return explorers.Select(Explore).Select(path => path.Count).Max() / 2;
    }

    static List<Tile> Explore(Explorer explorer)
    {
        //long numOfSteps = 0;

        var path = new List<Tile>();

        do
        {
            //numOfSteps++;
            path.Add(explorer.CurrentTile);

            var nextPos = explorer.CurrentTile.Position + explorer.Direction;
            var nextTile = explorer.Grid.Get(nextPos);
            if (nextTile.Connections.Length != 2)
            {
                throw new Exception("Next tile should always have only 2 connections");
            }

            //var connection = explorer.CurrentTile.Connections.Intersect(nextTile.Connections).Single();
            var connection = nextTile.Connections.Single(c => c == explorer.NextConnection); // rs-todo: this just checks we did have a connection!

            var nextConnection = nextTile.Connections.Single(c => c != connection);

            var nextDirection = Vector2.Normalize(nextConnection - nextTile.Position);

            explorer = new Explorer(nextTile, nextConnection, nextDirection, explorer.Grid);
        }
        while (explorer.CurrentTile.Char != 'S');

        return path;
    }

    public long? SolvePart2(string input)
    {
        return null;
    }

    static (Tile Start, Tile[][] Grid) ParseInput(string input)
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

        return (start, grid);
    }

    record struct Explorer(Tile CurrentTile, Vector2 NextConnection, Vector2 Direction, Tile[][] Grid)
    {
        //public Explorer MoveTo(Tile otherTile)
        //{
        //    CurrentTile.Connections.Intersect(otherTile.Connections).Single
        //}
    }

    record Tile(Vector2 Position, char Char, Vector2[] Connections)
    {
        public static Tile Create(Vector2 position, char chr)
        {
            /*
                | is a vertical pipe connecting north and south.
                - is a horizontal pipe connecting east and west.
                L is a 90-degree bend connecting north and east.
                J is a 90-degree bend connecting north and west.
                7 is a 90-degree bend connecting south and west.
                F is a 90-degree bend connecting south and east.
                . is ground; there is no pipe in this tile.
                S is the starting position of the animal; there is a pipe on this tile, but your sketch doesn't show what shape the pipe has.

                '|' => [north, south]
                '-' => [east, west]
                'L' => [north, east]
                'J' => [north, west]
                '7' => [south, west]
                'F' => [south, east]

             */

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

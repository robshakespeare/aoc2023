namespace AoC.Day16;

public class Day16Solver : ISolver
{
    public string DayName => "The Floor Will Be Lava";

    public long? SolvePart1(string input)
    {
        HashSet<Beam> beams = [new Beam(new Vector2(-1, 0), GridUtils.East)];
        HashSet<Vector2> energizedTiles = [];
        var grid = input.Split(Environment.NewLine);

        List<int> energizedTilesCountByFrame = [];

        do
        {
            HashSet<Beam> newBeams = [];

            foreach (var (position, direction) in beams)
            {
                var newPosition = position + direction;

                //var newBeam = new Beam(position + direction, direction);

                //beam.Position += beam.Direction;

                if (grid.TryGet(newPosition, out var tile))
                {
                    energizedTiles.Add(newPosition);

                    if (tile == '|' && (direction == GridUtils.East || direction == GridUtils.West))
                    {
                        beam.Direction = GridUtils.North;
                        beams.Add(new Beam(beam.Position, GridUtils.South));
                    }
                    else if (tile == '-' && (beam.Direction == GridUtils.North || beam.Direction == GridUtils.South))
                    {
                        beam.Direction = GridUtils.East;
                        beams.Add(new Beam(beam.Position, GridUtils.West));
                    }
                    else if (tile == '/')
                    {
                        beam.Direction = new Vector2(-beam.Direction.Y, -beam.Direction.X);
                    }
                    else if (tile == '\\')
                    {
                        beam.Direction = new Vector2(beam.Direction.Y, beam.Direction.X);
                    }
                    else
                    {

                    }
                }
                //else
                //{
                //    beams.Remove(beam); // Off-grid, so remove beam
                //}
            }

            Console.WriteLine("#beams: " + beams.Count);

            energizedTilesCountByFrame.Add(energizedTiles.Count);
        }
        while (energizedTilesCountByFrame.Count < 3 || energizedTilesCountByFrame[^3..].Any(x => x != energizedTilesCountByFrame[^1]));

        energizedTiles.ToStringGrid(p => p, _ => '#', '.').RenderGridToConsole();

        return energizedTiles.Count;
    }

    public long? SolvePart2(string input)
    {
        return null;
    }

    record Beam(Vector2 Position, Vector2 Direction);

    //class Beam(Vector2 position, Vector2 direction)
    //{
    //    public Vector2 Position { get; set; } = position;

    //    public Vector2 Direction { get; set; } = direction;
    //}
}

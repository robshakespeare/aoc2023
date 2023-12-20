namespace AoC.Day16;

public class Day16Solver : ISolver
{
    public string DayName => "The Floor Will Be Lava";

    public long? SolvePart1(string input)
    {
        var grid = input.Split(Environment.NewLine);
        var start = new Beam(new Vector2(-1, 0), GridUtils.East);
        return GetEnergizedTilesCount(grid, start);
    }

    public long? SolvePart2(string input)
    {
        var grid = input.Split(Environment.NewLine);
        var height = grid.Length;
        var width = grid[0].Length;

        // Get all the possible starts, and brute force it! Note the nudge back so we start off the grid and enter it.
        var starts = Enumerable.Range(0, width).Select(x => new Beam(new Vector2(x, 0), GridUtils.South))
            .Concat(Enumerable.Range(0, width).Select(x => new Beam(new Vector2(x, height - 1), GridUtils.North)))
            .Concat(Enumerable.Range(0, height).Select(y => new Beam(new Vector2(0, y), GridUtils.East)))
            .Concat(Enumerable.Range(0, height).Select(y => new Beam(new Vector2(width - 1, y), GridUtils.West)))
            .Select(beam => new Beam(beam.Position + (beam.Direction * -1), beam.Direction));

        return starts.Max(start => GetEnergizedTilesCount(grid, start));
    }

    static int GetEnergizedTilesCount(string[] grid, Beam start)
    {
        List<Beam> beams = [start];
        HashSet<Vector2> energizedTiles = [];
        HashSet<(Vector2, Vector2)> visited = [];
        List<int> energizedTilesCountByFrame = [];
        const int compareBack = 10;

        do
        {
            foreach (var beam in beams.ToArray())
            {
                var beamKey = (beam.Position, beam.Direction);
                if (visited.Contains(beamKey))
                {
                    beams.Remove(beam);
                    continue;
                }

                visited.Add(beamKey);

                beam.Position += beam.Direction;

                if (grid.TryGet(beam.Position, out var tile))
                {
                    energizedTiles.Add(beam.Position);

                    if (tile == '|' && (beam.Direction == GridUtils.East || beam.Direction == GridUtils.West))
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
                }
                else
                {
                    beams.Remove(beam); // Off-grid, so remove beam
                }
            }

            energizedTilesCountByFrame.Add(energizedTiles.Count);
        }
        while (energizedTilesCountByFrame.Count < compareBack || energizedTilesCountByFrame[^compareBack..].Any(x => x != energizedTilesCountByFrame[^1]));

        return energizedTiles.Count;
    }

    class Beam(Vector2 position, Vector2 direction)
    {
        public Vector2 Position { get; set; } = position;

        public Vector2 Direction { get; set; } = direction;
    }
}

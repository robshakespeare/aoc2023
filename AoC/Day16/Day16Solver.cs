namespace AoC.Day16;

public class Day16Solver : ISolver
{
    public string DayName => "The Floor Will Be Lava";

    public long? SolvePart1(string input)
    {
        List<Beam> beams = [new Beam(new Vector2(-1, 0), GridUtils.East)];
        HashSet<Vector2> energizedTiles = [];
        HashSet<(Vector2, Vector2)> visited = [];
        //var visited = new HashSet<Beam>(new BeamComparer());
        var grid = input.Split(Environment.NewLine);

        List<int> energizedTilesCountByFrame = [];

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

            //Console.WriteLine("#beams: " + beams.Count + ", #visited: " + visited.Count);

            energizedTilesCountByFrame.Add(energizedTiles.Count);
        }
        while (energizedTilesCountByFrame.Count < 3 || energizedTilesCountByFrame[^3..].Any(x => x != energizedTilesCountByFrame[^1]));

        //energizedTiles.ToStringGrid(p => p, _ => '#', '.').RenderGridToConsole();

        return energizedTiles.Count;
    }

    public long? SolvePart2(string input)
    {
        return null;
    }

    class Beam(Vector2 position, Vector2 direction)
    {
        public Vector2 Position { get; set; } = position;

        public Vector2 Direction { get; set; } = direction;
    }

    //class BeamComparer : IEqualityComparer<Beam>
    //{
    //    public bool Equals(Beam? x, Beam? y) => x?.Position == y?.Position && x?.Direction == y?.Direction;

    //    public int GetHashCode([DisallowNull] Beam beam) => HashCode.Combine(beam.Position, beam.Direction);
    //}
}

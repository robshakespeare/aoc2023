namespace AoC.Day24;

public class Day24Solver : ISolver
{
    public string DayName => "Never Tell Me The Odds";

    public long? SolvePart1(string input)
    {
        var hailstones = input.ReadLines()
            .Select(line => line.Split(new[] { ',', ' ', '@' }, StringSplitOptions.RemoveEmptyEntries).Select(double.Parse).ToArray())
            .Select(comps => new Hailstone(new Vector(comps[..3]), new Vector(comps[3..])))
            .ToArray();

        var intersections = hailstones.SelectMany(
            hailstone => hailstones
                .Where(other => hailstone != other)
                .Select(other => (hailstone, other, intersection: Line.Intersection2D(hailstone.Line, other.Line)))
                .Where(x => x.intersection.HasValue)
                .Where(x =>
                {
                    var dir1 = (x.intersection!.Value - x.hailstone.Position).Normalize2D();
                    var dir2 = (x.intersection!.Value - x.other.Position).Normalize2D();
                    return dir1.AlmostEqual(x.hailstone.Direction.Normalize2D()) && dir2.AlmostEqual(x.other.Direction.Normalize2D());
                })
                .Select(x => x.intersection!.Value)).Distinct().ToArray();

        var isExample = hailstones.Length < 10;
        var min = isExample ? 7 : 200000000000000;
        var max = isExample ? 27 : 400000000000000;

        return intersections.Count(i =>
            i.X >= min && i.X <= max &&
            i.Y >= min && i.Y <= max);
    }

    public long? SolvePart2(string input)
    {
        return null;
    }

    readonly record struct Vector(double X, double Y, double Z)
    {
        public Vector(double[] c) : this(c[0], c[1], c[2])
        {
        }

        public static Vector operator +(Vector a, Vector b) => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);

        public static Vector operator -(Vector a, Vector b) => new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);

        public Vector Normalize2D()
        {
            var length = Math.Sqrt(X * X + Y * Y);
            return new Vector(X / length, Y / length, 0);
        }

        public bool AlmostEqual(Vector other) => AlmostEqual(X, other.X) && AlmostEqual(Y, other.Y) && AlmostEqual(Y, other.Y);

        public static bool AlmostEqual(double a, double b, double eps = 0.0000001) => Math.Abs(a - b) < eps;
    }

    readonly record struct Line(Vector Start, Vector End)
    {
        public static Vector? Intersection2D(Line line1, Line line2)
        {
            var a1 = line1.End.Y - line1.Start.Y;
            var b1 = line1.Start.X - line1.End.X;
            var c1 = a1 * line1.Start.X + b1 * line1.Start.Y;

            var a2 = line2.End.Y - line2.Start.Y;
            var b2 = line2.Start.X - line2.End.X;
            var c2 = a2 * line2.Start.X + b2 * line2.Start.Y;

            var delta = a1 * b2 - a2 * b1;

            // delta == 0 means lines are parallel
            return delta == 0 ? null
                : new Vector((b2 * c1 - b1 * c2) / delta, (a1 * c2 - a2 * c1) / delta, 0);
        }
    }

    record Hailstone(Vector Position, Vector Direction)
    {
        public Vector NextPosition { get; } = Position + Direction;

        public Line Line { get; } = new(Position, Position + Direction);
    }
}

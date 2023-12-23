using static System.Environment;

namespace AoC.Day22;

public class Day22Solver : ISolver
{
    public string DayName => "Sand Slabs";

    public long? SolvePart1(string input)
    {
        var bricks = ParseAndGroundBricks(input);

        // "Find out which bricks can safely be disintegrated"
        // A brick can be disintegrated if all the bricks it supports would still be supported by somebody else
        return bricks.Count(brick => brick.Supports.All(s => s.SupportedBy.Count > 1));
    }

    public long? SolvePart2(string input)
    {
        // "You'll need to figure out the best brick to disintegrate. For each brick, determine how many other bricks would fall if that brick were disintegrated."
        // "For each brick, determine how many other bricks would fall if that brick were disintegrated. What is the sum of the number of other bricks that would fall?"

        return null;
    }

    static IReadOnlyCollection<Brick> ParseAndGroundBricks(string input)
    {
        var bricks = input.ReadLines()
            .Select(line => line.Split('~', ','))
            .Select(coords => coords.Select(float.Parse).ToArray())
            .Select((coords, i) => new Brick(i, (char)('A' + i % 26), new Vector3(coords.AsSpan()[0..3]), new Vector3(coords.AsSpan()[3..6])))
            .ToArray();
        var isExample = bricks.Length < 10;
        void Log(Func<string> msg)
        {
            if (isExample)
            {
                Console.WriteLine(msg());
            }
        }

        // Ground the bricks
        List<Brick> groundedBricks = [];
        const int groundLevel = 1;
        foreach (var brick in bricks.OrderBy(b => b.Min.Z))
        {
            var thisBrick = brick;

            while (thisBrick.Min.Z > groundLevel)
            {
                thisBrick = thisBrick.NudgeDown();

                if (groundedBricks.Any(groundedBrick => groundedBrick.Intersects(thisBrick)))
                {
                    thisBrick = thisBrick.NudgeUp();
                    break;
                }
            }

            groundedBricks.Add(thisBrick);
        }

        Log(() => $"Height is now: {groundedBricks.Max(x => x.Max.Z)}{NewLine}");

        // Find out who supports who
        foreach (var brick in groundedBricks)
        {
            var nextZ = brick.Max.Z + 1;
            var shadow = brick.NudgeUp();
            brick.Supports.AddRange(groundedBricks.Where(g => g.Min.Z == nextZ && shadow.Intersects(g)));

            Log(() => $"Brick {brick.Letter} supports {string.Join(", ", brick.Supports.Select(s => s.Letter))}");
        }

        Log(() => "");

        // Find who is supported by who
        foreach (var brick in groundedBricks)
        {
            brick.SupportedBy.AddRange(groundedBricks.Where(g => g.Supports.Contains(brick)));

            Log(() => $"Brick {brick.Letter} is supported by {string.Join(", ", brick.SupportedBy.Select(s => s.Letter))}");
        }

        return groundedBricks;
    }

    record Brick(int Id, char Letter, Vector3 Min, Vector3 Max)
    {
        public int Height { get; } = (int)(Max.Z - Min.Z);

        public List<Brick> Supports { get; } = [];

        public List<Brick> SupportedBy { get; } = [];

        public Brick MoveToZ(int newZ) => this with
        {
            Min = Min with { Z = newZ },
            Max = Max with { Z = newZ + Height }
        };

        public Brick NudgeUp() => MoveToZ((int)Min.Z + 1);

        public Brick NudgeDown() => MoveToZ((int)Min.Z - 1);

        public bool Intersects(Brick b) =>
            Min.X <= b.Max.X &&
            Max.X >= b.Min.X &&
            Min.Y <= b.Max.Y &&
            Max.Y >= b.Min.Y &&
            Min.Z <= b.Max.Z &&
            Max.Z >= b.Min.Z;

        public IEnumerable<Vector3> GetPoints()
        {
            var dir = Vector3.Normalize(Max - Min);
            var pos = Min;
            yield return pos;
            do
            {
                yield return pos += dir;
            } while (pos != Max);
        }

        public override string ToString() => $"{Min.X},{Min.Y},{Min.Z}~{Max.X},{Max.Y},{Max.Z}   <- {Letter}";
    }
}

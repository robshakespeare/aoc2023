using System.Collections.Generic;

namespace AoC.Day22;

public class Day22Solver : ISolver
{
    public string DayName => "Sand Slabs";

    public long? SolvePart1(string input)
    {
        var bricks = input.ReadLines()
            .Select(line => line.Split('~', ','))
            .Select(coords => coords.Select(float.Parse).ToArray())
            .Select((coords, i) => new Brick(i, (char)('A' + i % 26), new Vector3(coords.AsSpan()[0..3]), new Vector3(coords.AsSpan()[3..6])))
            .ToArray();

        // 1,0,1~1,2,1
        //var roundtrip = string.Join(Environment.NewLine, bricks.Select(box => box.ToString()));
        //Console.WriteLine(roundtrip);

        //RenderProjection(0, bricks);
        //RenderProjection(1, bricks);

        //Console.WriteLine(roundtrip == input);

        //// Complete the bricks falling down
        //// Brute force: For each one from bottom to top, adjust it down to ground level, then if it intersects any that have already landed, move it up one, until it doesn't intersect any
        //List<Brick> groundedBricks = [];
        //const int justAboveGround = 1;
        //foreach (var brick in bricks.OrderBy(b => b.Min.Z))
        //{
        //    var thisBrick = brick.MoveToZ(justAboveGround);

        //    while (groundedBricks.Any(groundedBrick => groundedBrick.Intersects(thisBrick)))
        //    {
        //        thisBrick = thisBrick.NudgeUp();
        //    }

        //    groundedBricks.Add(thisBrick);
        //}

        //Console.WriteLine("Height is now: " + groundedBricks.Max(x => x.Max.Z));

        //var debug = string.Join(Environment.NewLine, groundedBricks.Select(box => box.ToString()));
        //Console.WriteLine(debug);

        //RenderProjection(0, groundedBricks);
        //RenderProjection(1, groundedBricks);

        // Attempt 2!:

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

        Console.WriteLine("Height is now: " + groundedBricks.Max(x => x.Max.Z));
        Console.WriteLine();

        // Find out who supports who
        foreach (var brick in groundedBricks)
        {
            var nextZ = brick.Max.Z + 1;
            var shadow = brick.NudgeUp();
            brick.Supports.AddRange(groundedBricks.Where(g => g.Min.Z == nextZ && shadow.Intersects(g)));

            Console.WriteLine($"Brick {brick.Letter} supports {string.Join(", ", brick.Supports.Select(s => s.Letter))}");
        }

        Console.WriteLine();

        // Find who is supported by who
        foreach (var brick in groundedBricks)
        {
            brick.SupportedBy.AddRange(groundedBricks.Where(g => g.Supports.Contains(brick)));

            Console.WriteLine($"Brick {brick.Letter} is supported by {string.Join(", ", brick.SupportedBy.Select(s => s.Letter))}");
        }

        // Find out which bricks can safely be disintegrated
        // A brick can be disintegrated if all the bricks it supports would still be supported by somebody else
        return groundedBricks.Count(brick => brick.Supports.All(s => s.SupportedBy.Count > 1));


        //var debug = string.Join(Environment.NewLine, groundedBricks.Select(box => box.ToString()));
        //Console.WriteLine(debug);

        //RenderProjection(0, groundedBricks);
        //RenderProjection(1, groundedBricks);

        return null;
    }

    public long? SolvePart2(string input)
    {
        return null;
    }

    static void RenderProjection(int coord, IEnumerable<Brick> bricks)
    {
        bricks.SelectMany(brick => brick.GetPoints().Select(p => (p, brick.Letter))).ToStringGrid(i => new Vector2(i.p[coord], i.p.Z), i => i.Letter, '.').Reverse().RenderGridToConsole();
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

        public override string ToString() => $"{Min.X},{Min.Y},{Min.Z}~{Max.X},{Max.Y},{Max.Z}   <- {Id}";
    }
}

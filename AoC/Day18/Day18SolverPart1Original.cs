namespace AoC.Day18;

public static class Day18SolverPart1Original
{
    public static long? SolvePart1(string input)
    {
        HashSet<Vector2> trenchMap = [];
        List<Vector2> northEdge = [];

        var position = Vector2.Zero;
        trenchMap.Add(position);
        northEdge.Add(position);

        var numLines = 0;
        foreach (var line in input.ReadLines())
        {
            numLines++;
            var split = line.Split(' ');
            var dir = split[0] switch
            {
                "U" => GridUtils.North,
                "D" => GridUtils.South,
                "L" => GridUtils.West,
                "R" => GridUtils.East,
                _ => throw new Exception("Invalid dir: " + split[0])
            };
            var amount = int.Parse(split[1]);

            for (var i = 0; i < amount; i++)
            {
                position += dir;
                trenchMap.Add(position);

                if (position.Y < northEdge.First().Y)
                {
                    northEdge.Clear();
                    northEdge.Add(position);
                }
                else if (position.Y == northEdge.First().Y)
                {
                    northEdge.Add(position);
                }
            }
        }

        // "The next step is to dig out the interior so that it is one meter deep as well"
        // So, go down from any north edge, and if we reach space, then start digging out (filling out) from there
        var start = northEdge.Select(n => n + GridUtils.South).First(v => !trenchMap.Contains(v));

        var digOut = new HashSet<Vector2>([start]);
        var explore = new Queue<Vector2>([start]);

        while (explore.Count > 0)
        {
            var currentPosition = explore.Dequeue();

            var nextPositions = GridUtils.DirectionsIncludingDiagonal.Select(dir => currentPosition + dir)
                .Where(candidatePosition => !trenchMap.Contains(candidatePosition));

            foreach (var nextPosition in nextPositions)
            {
                if (!digOut.Contains(nextPosition))
                {
                    explore.Enqueue(nextPosition);
                    digOut.Add(nextPosition);
                }
            }
        }

        // Visualise:
        if (numLines < 50)
        {
            Console.WriteLine(string.Join(Environment.NewLine, trenchMap.Concat(digOut).ToStringGrid(
                p => p,
                p => p == start
                        ? 'S'
                        : digOut.Contains(p)
                            ? 'D'
                            : northEdge.Contains(p)
                                ? 'N'
                                : '#',
                '.')));
        }

        var area = trenchMap.Count + digOut.Count;

        Console.WriteLine($"Original calculation's area: {area}");

        return area;
    }
}

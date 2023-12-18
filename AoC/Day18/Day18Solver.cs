namespace AoC.Day18;

public class Day18Solver : ISolver
{
    public string DayName => "Lavaduct Lagoon";

    public long? SolvePart1(string input)
    {
        HashSet<Vector2> trenchMap = [];

        var position = Vector2.Zero;
        trenchMap.Add(position);

        foreach (var line in input.ReadLines())
        {
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
            }
        }

        Console.WriteLine(string.Join(Environment.NewLine, trenchMap.ToStringGrid(p => p, _ => '#', '.')));

        return null;
    }

    public long? SolvePart2(string input)
    {
        return null;
    }
}

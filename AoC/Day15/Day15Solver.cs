namespace AoC.Day15;

public partial class Day15Solver : ISolver
{
    public string DayName => "Lens Library";

    public long? SolvePart1(string input) => input.Split(',').Sum(HASH);

    public long? SolvePart2(string input)
    {
        var boxes = Enumerable.Range(0, 256).Select(_ => new List<Lens>()).ToArray();
        var steps = ParseStepsRegex().Matches(input)
            .Select(m => new Step(
                m.Groups["label"].Value,
                m.Groups["op"].Value.Single(),
                int.Parse('0' + m.Groups["focalLength"].Value)));

        foreach (var step in steps)
        {
            var index = boxes[step.Hash].FindIndex(lens => lens.Label == step.Label);

            if (step.Operation == '-')
            {
                if (index >= 0)
                {
                    boxes[step.Hash].RemoveAt(index);
                }
            }
            else if (step.Operation == '=')
            {
                var lens = new Lens(step.Label, step.FocalLength);

                if (index >= 0)
                {
                    boxes[step.Hash].RemoveAt(index);
                    boxes[step.Hash].Insert(index, lens);
                }
                else
                {
                    boxes[step.Hash].Add(lens);
                }
            }
            else
            {
                throw new Exception("Invalid operation: " + step.Operation);
            }
        }

        return boxes.SelectMany((box, boxNum) => box.Select((lens, slotIdx) => lens.FocusingPower(boxNum, slotIdx + 1))).Sum();
    }

    public static long HASH(string s) => s.Aggregate(0L, (agg, cur) => ((agg + cur) * 17) % 256);

    public record Lens(string Label, int FocalLength)
    {
        public long FocusingPower(int boxNum, int slotNum) => (1 + boxNum) * slotNum * FocalLength;
    }

    public record Step(string Label, char Operation, int FocalLength)
    {
        public long Hash { get; } = HASH(Label);
    }

    [GeneratedRegex(@"(?<label>[^,]+)(?<op>[-=])(?<focalLength>\d+)?", RegexOptions.Compiled)]
    private static partial Regex ParseStepsRegex();
}

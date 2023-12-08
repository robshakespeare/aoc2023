using System.Collections.Frozen;

namespace AoC.Day08;

public partial class Day8Solver : ISolver
{
    public string DayName => "Haunted Wasteland";

    public long? SolvePart1(string input)
    {
        var (instructions, network) = ParseInput(input);

        var currentNode = "AAA";
        var instructionPointer = 0;
        var numOfSteps = 0;

        while (currentNode != "ZZZ")
        {
            var isLeft = instructions[instructionPointer] == 'L';
            instructionPointer = ++instructionPointer % instructions.Length;
            numOfSteps++;
            currentNode = isLeft ? network[currentNode].Left : network[currentNode].Right;
        }

        return numOfSteps;
    }

    public long? SolvePart2(string input)
    {
        return null;
    }

    record Document(string Instructions, IDictionary<string, (string Left, string Right)> Network);

    static Document ParseInput(string input)
    {
        var lines = input.ReadLines().ToArray();
        var instructions = lines[0];
        var network = lines.Skip(2)
            .Select(line => ParseLine().Match(line))
            .ToDictionary(match => match.Groups["key"].Value, match => (match.Groups["left"].Value, match.Groups["right"].Value))
            .ToFrozenDictionary();

        return new Document(instructions, network);
    }

    [GeneratedRegex(@"(?<key>\w+) = \((?<left>\w+), (?<right>\w+)\)")]
    private static partial Regex ParseLine();
}

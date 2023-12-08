using System.Collections.Frozen;
using System.Net;

namespace AoC.Day08;

public partial class Day8Solver : ISolver
{
    public string DayName => "Haunted Wasteland";

    public long? SolvePart1(string input)
    {
        return GetNumOfSteps(
            document: ParseInput(input),
            startNode: "AAA",
            isEnd: currentNode => currentNode != "ZZZ");

        //var currentNode = "AAA";
        //var instructionPointer = 0;
        //var numOfSteps = 0L;

        //while (currentNode != "ZZZ")
        //{
        //    var pair = instructions[instructionPointer] == 'L' ? 0 : 1;
        //    instructionPointer = ++instructionPointer % instructions.Length;
        //    numOfSteps++;
        //    currentNode = network[currentNode][pair];
        //}

        //return numOfSteps;
    }

    public long? SolvePart2(string input)
    {
        var (instructions, network) = ParseInput(input);

        var currentNodes = network.Keys.Where(key => key.EndsWith('A')).ToArray();

        var instructionPointer = 0;
        var numOfSteps = 0;

        while (currentNodes.Any(node => node[^1] != 'Z'))
        {
            var pair = instructions[instructionPointer] == 'L' ? 0 : 1;
            instructionPointer = ++instructionPointer % instructions.Length;
            numOfSteps++;
            currentNodes = currentNodes.Select(currentNode => network[currentNode][pair]).ToArray();

            var anyEnd = currentNodes.Where(node => node[^1] == 'Z').ToArray();

            if (anyEnd.Any())
            {
                Console.WriteLine($"step: {numOfSteps}, anyEnd: {string.Join(", ", anyEnd)}");
            }
        }

        return numOfSteps;
    }

    static long GetNumOfSteps(Document document, string startNode, Func<string, bool> isEnd)
    {
        var (instructions, network) = document;
        var currentNode = startNode;
        var instructionPointer = 0;
        var numOfSteps = 0L;

        while (isEnd(currentNode))
        {
            var pair = instructions[instructionPointer] == 'L' ? 0 : 1;
            instructionPointer = ++instructionPointer % instructions.Length;
            numOfSteps++;
            currentNode = network[currentNode][pair];
        }

        return numOfSteps;
    }

    record Document(string Instructions, IDictionary<string, string[]> Network);

    static Document ParseInput(string input)
    {
        var lines = input.ReadLines().ToArray();
        var instructions = lines[0];
        var network = lines.Skip(2)
            .Select(line => ParseLine().Match(line))
            .ToDictionary(match => match.Groups["key"].Value, match => match.Groups["pair"].Value.Split(", "))
            .ToFrozenDictionary();

        return new Document(instructions, network);
    }

    [GeneratedRegex(@"(?<key>\w+) = \((?<pair>[^)]+)\)")]
    private static partial Regex ParseLine();
}

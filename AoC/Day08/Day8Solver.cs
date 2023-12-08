namespace AoC.Day08;

public partial class Day8Solver : ISolver
{
    public string DayName => "Haunted Wasteland";

    public long? SolvePart1(string input) => GetNumOfSteps(
        document: ParseInput(input),
        startNode: "AAA",
        isEnd: currentNode => currentNode == "ZZZ");

    public long? SolvePart2(string input)
    {
        var document = ParseInput(input);
        var startNodes = document.Network.Keys.Where(key => key.EndsWith('A')).ToArray();
        static bool IsEnd(string currentNode) => currentNode.EndsWith('Z');

        var numOfSteps = startNodes.Select(startNode => GetNumOfSteps(document, startNode, IsEnd)).ToArray();

        Console.WriteLine($"numOfSteps: {string.Join(", ", numOfSteps)}");
        numOfSteps.Dump("numOfSteps");
        document.Instructions.Dump("document.Instructions");
        new { test = true, name = "Test", values = (int[])[1, 2, 3, 4] }.Dump();
        new TestObject("testObj", 12, true).Dump();
        (Test1: true, Test2: "Hello world").Dump();

        return MathUtils.LeastCommonMultiple(numOfSteps);
    }

    record TestObject(string Id, int Age, bool IsTest);

    static long GetNumOfSteps(Document document, string startNode, Func<string, bool> isEnd)
    {
        var (instructions, network) = document;
        var currentNode = startNode;
        var instructionPointer = 0;
        var numOfSteps = 0L;

        while (!isEnd(currentNode))
        {
            var pair = instructions[instructionPointer] == 'L' ? 0 : 1;
            instructionPointer = ++instructionPointer % instructions.Length;
            numOfSteps++;
            currentNode = network[currentNode][pair];

            if (currentNode.EndsWith('Z'))
            {
                currentNode.Dump();
            }
        }

        return numOfSteps;
    }

    record Document(string Instructions, IReadOnlyDictionary<string, string[]> Network);

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

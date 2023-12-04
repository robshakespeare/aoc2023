namespace AoC.Day04;

public partial class Day4Solver : ISolver
{
    public string DayName => "Scratchcards";

    public long? SolvePart1(string input) => ParseInput(input).Sum(scratchcard => scratchcard.GetValue());

    public long? SolvePart2(string input)
    {
        var cards = ParseInput(input);
        var numberOfInstancesPerCard = cards.ToDictionary(card => card.CardId, _ => 1L);

        foreach (var card in cards)
        {
            var numberOfInstances = numberOfInstancesPerCard[card.CardId];

            foreach (var winningCardId in card.GetNextWinningCardIds())
                numberOfInstancesPerCard[winningCardId] += numberOfInstances;
        }

        return numberOfInstancesPerCard.Sum(x => x.Value);
    }

    record Scratchcard(int CardId, int[] WinningsNumbers, int[] OurNumbers)
    {
        public int[] MatchingNumbers { get; } = WinningsNumbers.Intersect(OurNumbers).ToArray();

        public long GetValue() => (long)Math.Pow(2, MatchingNumbers.Length - 1);

        public IEnumerable<int> GetNextWinningCardIds() => MatchingNumbers.Select((_, i) => CardId + i + 1);
    }

    [GeneratedRegex(@"^Card +(?<cardId>\d+):(?<winningNumbers>[^|]+)\|(?<ourNumbers>[^|]+)$", RegexOptions.Multiline)]
    private static partial Regex ParseInputRegex();

    static Scratchcard[] ParseInput(string input) => ParseInputRegex().Matches(input)
        .Select(match => new Scratchcard(
            int.Parse(match.Groups["cardId"].Value),
            ParseNumbers(match.Groups["winningNumbers"].Value),
            ParseNumbers(match.Groups["ourNumbers"].Value)))
        .ToArray();

    static int[] ParseNumbers(string value) => value.Trim().Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
}

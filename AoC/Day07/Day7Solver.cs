namespace AoC.Day07;

public class Day7Solver : ISolver
{
    const char Joker = 'X';

    public string DayName => "Camel Cards";

    public long? SolvePart1(string input) => CalcTotalWinnings(input);

    public long? SolvePart2(string input) => CalcTotalWinnings(input.Replace('J', Joker)); // Replace 'J' with 'X' (where 'X' is our Joker card indicator)

    static long CalcTotalWinnings(string input) => ParseInput(input)
        .OrderBy(x => x.Hand.OrdinalScore)
        .Select((x, i) => new { x.Hand, x.Bid, Rank = i + 1 })
        .Select(x => x.Bid * x.Rank)
        .Sum();

    record Hand(string Cards, HandType Type, string OrdinalScore)
    {
        public static Hand Create(string cards)
        {
            var type = DetermineType(cards);
            return new Hand(cards, type, $"{type:D}{string.Concat(cards.Select(CardStrengthOrdinal))}");
        }

        static HandType DetermineType(string cards)
        {
            var counts = UpgradeHand(cards).GroupBy(c => c).Select(g => g.Count()).OrderByDescending(x => x).ToArray();
            return counts switch
            {
                [5] => HandType.FiveOfAKind,
                [4, 1] => HandType.FourOfAKind,
                [3, 2] => HandType.FullHouse,
                [3, ..] => HandType.ThreeOfAKind,
                [2, 2, ..] => HandType.TwoPair,
                [2, ..] => HandType.OnePair,
                _ => HandType.HighCard
            };
        }

        static string UpgradeHand(string cards)
        {
            var commonCard = cards.Where(c => c != Joker)
                .GroupBy(c => c)
                .OrderByDescending(g => g.Count())
                .FirstOrDefault()?.Key ?? 'A';
            return cards.Replace(Joker, commonCard);
        }

        static char CardStrengthOrdinal(char Card) => Card switch
        {
            'A' => 'E',
            'K' => 'D',
            'Q' => 'C',
            'J' => 'B',
            'T' => 'A',
            Joker => '1',
            _ => Card
        };
    }

    enum HandType
    {
        HighCard,
        OnePair,
        TwoPair,
        ThreeOfAKind,
        FullHouse,
        FourOfAKind,
        FiveOfAKind
    }

    static (Hand Hand, long Bid)[] ParseInput(string input) => input.ReadLines()
        .Select(line => line.Split(" "))
        .Select(parts => (Hand.Create(parts[0]), long.Parse(parts[1])))
        .ToArray();
}

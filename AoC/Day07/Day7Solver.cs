namespace AoC.Day07;

public class Day7Solver : ISolver
{
    const char Joker = 'X';

    public string DayName => "Camel Cards";

    public long? SolvePart1(string input) => CalcTotalWinnings(input);

    public long? SolvePart2(string input) => CalcTotalWinnings(input.Replace('J', Joker)); // Replace 'J' with 'X' (where 'X' is our Joker card indicator)

    /// <summary>
    /// Parse, then rank, then calculate total winnings.
    /// </summary>
    static long CalcTotalWinnings(string input) => ParseInput(input)
        .OrderBy(x => x.Hand, new HandComparer())
        .Select((x, i) => new { x.Hand, x.Bid, Rank = i + 1 })
        .Select(x => x.Bid * x.Rank)
        .Sum();

    record Hand(string Cards)
    {
        public HandType Type { get; } = DetermineType(UpgradeHand(Cards));

        static HandType DetermineType(string cards)
        {
            var groups = cards.GroupBy(c => c).OrderByDescending(g => g.Count()).ToArray();
            return groups.Length switch
            {
                1 => HandType.FiveOfAKind,
                2 => groups[0].Count() == 4 ? HandType.FourOfAKind : HandType.FullHouse,
                3 => groups[0].Count() == 3 ? HandType.ThreeOfAKind : HandType.TwoPair,
                4 => HandType.OnePair,
                _ => HandType.HighCard
            };
        }

        public static string UpgradeHand(string cards)
        {
            var commonCard = cards.Where(c => c != Joker)
                .GroupBy(c => c)
                .OrderByDescending(g => g.Count())
                .FirstOrDefault()?.Key ?? 'A';
            return cards.Replace(Joker, commonCard);
        }
    }

    class HandComparer : IComparer<Hand>
    {
        public int Compare(Hand? x, Hand? y)
        {
            if (x == null || y == null) throw new NotSupportedException();

            // Compare cards if hand type is the same; Otherwise compare Type
            if (x.Type == y.Type)
                foreach (var (cX, cY) in x.Cards.Select(CardStrength).Zip(y.Cards.Select(CardStrength)))
                    if (cX != cY)
                        return cX - cY;

            return x.Type - y.Type;
        }
    }

    static int CardStrength(char Card) => Card switch
    {
        'A' => 14,
        'K' => 13,
        'Q' => 12,
        'J' => 11,
        'T' => 10,
        Joker => 1,
        _ => int.Parse($"{Card}")
    };

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
        .Select(parts => (new Hand(parts[0]), long.Parse(parts[1])))
        .ToArray();
}

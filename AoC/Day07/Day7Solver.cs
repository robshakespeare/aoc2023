namespace AoC.Day07;

public class Day7Solver : ISolver
{
    public string DayName => "Camel Cards";

    public long? SolvePart1(string input)
    {
        var handAndBids = ParseInput(input);

        var ordered = handAndBids.OrderByDescending(x => x.Hand, new HandComparer()).ToArray();

        Console.WriteLine(
            handAndBids
                .OrderBy(x => x.Hand, new HandComparer())
                .Select((x, i) => new { hand = x.Hand, bid = x.Bid, rank = i + 1 })
                .Dump());

        return handAndBids
            .OrderBy(x => x.Hand, new HandComparer())
            .Select((x, i) => x.Bid * (i + 1))
            .Sum();
    }

    public long? SolvePart2(string input)
    {
        return null;
    }

    public record Hand(string Cards)
    {
        public HandType Type { get; } = DetermineType(Cards);

        public static HandType DetermineType(string cards)
        {
            var groups = cards.GroupBy(c => c).OrderByDescending(g => g.Count()).ToArray();
            return groups.Length switch
            {
                1 => HandType.FiveOfAKind,
                2 => groups[0].Count() == 4 ? HandType.FourOfAKind : HandType.FullHouse,
                3 => groups[0].Count() == 3 ? HandType.ThreeOfAKind : HandType.TwoPair,
                4 => HandType.OnePair,
                5 => HandType.HighCard,
                _ => throw new Exception("Invalid hand: " + cards)
            };
        }
    }

    class HandComparer : IComparer<Hand>
    {
        public int Compare(Hand? x, Hand? y)
        {
            /*
                x is less than y => Less than zero
                x equals y => Zero
                x is greater than y => Greater than zero
             */

            if (x == null || y == null)
            {
                throw new NotSupportedException();
            }

            // Compare cards if hand type is the same
            if (x.Type == y.Type)
            {
                foreach (var (cX, cY) in x.Cards.Select(CardStrength).Zip(y.Cards.Select(CardStrength)))
                {
                    if (cX == cY)
                    {
                        continue;
                    }
                    return cX - cY;
                }
            }

            // Otherwise compare Type
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
        '9' => 9,
        '8' => 8,
        '7' => 7,
        '6' => 6,
        '5' => 5,
        '4' => 4,
        '3' => 3,
        '2' => 2,
        //var x when int.TryParse(x, out var r) => r,
        _ => throw new Exception("Invalid card: " + Card)
    };

    public enum HandType
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

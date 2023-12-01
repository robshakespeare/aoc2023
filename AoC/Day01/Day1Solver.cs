namespace AoC.Day01;

public class Day1Solver : ISolver
{
    public string DayName => "Trebuchet?!";

    //[GeneratedRegex(@"one|two|three|four|five|six|seven|eight|nine")]
    //private static partial Regex NumbersAsWordsRegex(RegexOptions.);

    //static string Normalize(string line) => NumbersAsWordsRegex().Replace(
    //    line,
    //    match => match.Value switch
    //    {
    //        "one" => "1",
    //        "two" => "2",
    //        "three" => "3",
    //        "four" => "4",
    //        "five" => "5",
    //        "six" => "6",
    //        "seven" => "7",
    //        "eight" => "8",
    //        "nine" => "9",
    //        var x => throw new InvalidOperationException("Unexpected token: " + x)
    //    });

    public long? SolvePart1(string input) =>
        input.ReadLines()
            .Select(line => long.Parse($"{GetDigit(line)}{GetDigit(line.Reverse())}"))
            .Sum();

    public long? SolvePart2(string input) => SolvePart1(Normalize(input));

    static long GetDigit(IEnumerable<char> line) => long.Parse($"{line.SkipWhile(c => !char.IsDigit(c)).First()}");

    static string Normalize(string input)
    {
        var numbersAsWords = "one|two|three|four|five|six|seven|eight|nine";
        var numbersAsWordsRegex = new Regex(numbersAsWords, RegexOptions.Compiled);
        var numbersAsWordsRevRegex = new Regex(Reverse(numbersAsWords), RegexOptions.Compiled);

        return string.Join(Environment.NewLine, input.ReadLines().Select(NormalizeLine));

        string NormalizeLine(string line)
        {
            line = numbersAsWordsRegex.Replace(line, match => Replacement(match.Value), 1);
            line = Reverse(numbersAsWordsRevRegex.Replace(Reverse(line), match => Replacement(Reverse(match.Value)), 1));
            return line;
        }

        static string Reverse(string value) => string.Concat(value.Reverse());

        static string Replacement(string value) => value switch
        {
            "one" => "1",
            "two" => "2",
            "three" => "3",
            "four" => "4",
            "five" => "5",
            "six" => "6",
            "seven" => "7",
            "eight" => "8",
            "nine" => "9",
            var x => throw new InvalidOperationException("Unexpected token: " + x)
        };
    }
}

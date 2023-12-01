using System.Text.RegularExpressions;

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

    public long? SolvePart1(string input)
    {
        return input.ReadLines()
            .Select(line => long.Parse($"{GetDigit(line)}{GetDigit(line.Reverse())}"))
            .Sum();

        static long GetDigit(IEnumerable<char> line) => long.Parse($"{line.SkipWhile(c => !char.IsDigit(c)).First()}");
    }

    public long? SolvePart2(string input)
    {
        var firstDigitRegex = new Regex("[0-9]|one|two|three|four|five|six|seven|eight|nine", RegexOptions.Compiled);
        var lastDigitRegex = new Regex("[0-9]|one|two|three|four|five|six|seven|eight|nine", RegexOptions.Compiled | RegexOptions.RightToLeft);

        return input.ReadLines()
            .Select(line => long.Parse($"{DigitMatchToString(firstDigitRegex.Match(line))}{DigitMatchToString(lastDigitRegex.Match(line))}"))
            .Sum();

        static string DigitMatchToString(Match match) => match.Value switch
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
            var digit => digit
        };
    }
}

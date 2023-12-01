namespace AoC.Day01;

public partial class Day1Solver : ISolver
{
    public string DayName => "Trebuchet?!";

    public long? SolvePart1(string input)
    {
        return input.ReadLines()
            .Select(line => long.Parse($"{GetDigit(line)}{GetDigit(line.Reverse())}"))
            .Sum();

        static string GetDigit(IEnumerable<char> line) => $"{line.SkipWhile(c => !char.IsDigit(c)).First()}";
    }

    public long? SolvePart2(string input)
    {
        return input.ReadLines()
            .Select(line => long.Parse($"{GetDigit(line, FirstDigitRegex())}{GetDigit(line, LastDigitRegex())}"))
            .Sum();

        static string GetDigit(string line, Regex digitRegex) => digitRegex.Match(line).Value switch
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

    const string DigitRegex = "[0-9]|one|two|three|four|five|six|seven|eight|nine";

    [GeneratedRegex(DigitRegex, RegexOptions.Compiled)]
    private static partial Regex FirstDigitRegex();

    [GeneratedRegex(DigitRegex, RegexOptions.Compiled | RegexOptions.RightToLeft)]
    private static partial Regex LastDigitRegex();
}

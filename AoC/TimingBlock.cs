using System.Diagnostics;
using static Crayon.Output;

namespace AoC;

public sealed class TimingBlock(string name) : IDisposable
{
    private readonly Stopwatch _stopwatch = Stopwatch.StartNew();

    public TimeSpan Stop()
    {
        _stopwatch.Stop();
        return _stopwatch.Elapsed;
    }

    public void Dispose()
    {
        Stop();

        Console.WriteLine(Rgb(118, 118, 118).Text($"[{name}] time taken (seconds): {_stopwatch.Elapsed.TotalSeconds:0.000000}"));
        Console.WriteLine();
    }
}

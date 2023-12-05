namespace AoC.Day05;

public class Day5Solver : ISolver
{
    public string DayName => "If You Give A Seed A Fertilizer";

    public long? SolvePart1(string input)
    {
        var (seeds, pipeline) = ParseInput(input);
        return seeds.Select(pipeline.ConvertSeedToLocation).Min();
    }

    public long? SolvePart2(string input)
    {
        return null;
    }

    record MapperRange(long DestinationStart, long SourceStart, long RangeLength)
    {
        public long Offset { get; } = DestinationStart - SourceStart;

        public long SourceEnd { get; } = SourceStart + RangeLength - 1;

        public long? Map(long source)
        {
            if (source >= SourceStart && source <= SourceEnd)
            {
                return source + Offset;
            }

            return null;
        }
    }

    record Mapper(string Name, MapperRange[] Ranges)
    {
        public long Map(long source)
        {
            foreach (var range in Ranges)
            {
                var destination = range.Map(source);
                if (destination != null)
                {
                    return destination.Value;
                }
            }

            return source;
        }
    }

    record Pipeline(Mapper[] Mappers)
    {
        public long ConvertSeedToLocation(long seed) => Mappers.Aggregate(seed, (acc, mapper) => mapper.Map(acc));
    }

    static (long[] Seeds, Pipeline) ParseInput(string input)
    {
        var sections = input.Split(Environment.NewLine + Environment.NewLine);

        var seeds = sections.First().Split(" ").Skip(1).Select(long.Parse).ToArray();

        var mappers = sections.Skip(1).Select(section =>
        {
            var lines = section.ReadLines().ToArray();
            var name = lines.First().Split(" ").First();
            return new Mapper(
                name,
                lines.Skip(1)
                    .Select(line => line.Split(" ").Select(long.Parse).ToArray())
                    .Select(values => new MapperRange(values[0], values[1], values[2]))
                    .ToArray()
            );
        }).ToArray();

        return (seeds, new Pipeline(mappers));
    }
}

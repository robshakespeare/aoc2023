namespace AoC.Day05;

public class Day5Solver : ISolver
{
    public string DayName => "If You Give A Seed A Fertilizer";

    public long? SolvePart1(string input)
    {
        var (seeds, _, pipeline) = ParseInput(input);
        return seeds.Select(pipeline.ConvertSeedToLocation).Min();
    }

    public long? SolvePart2(string input)
    {
        var (_, seedRanges, pipeline) = ParseInput(input);

        // For each 'seed range', get location of start and end, then mid point. Then use a kind-of Binary Search combined with a kind-of Dijkstra Search.
        // but optimise:
        //  - when a range's start and end and mid are the same result location, then assume it is a whole block, so record that location and drop out.
        //  - also exclude sub ranges where the minimum possible location in that range is greater than the currently known lowest location.

        var currentMinLocation = long.MaxValue;

        var explore = new PriorityQueue<SeedRange, long>(seedRanges.Select(seedRange => (seedRange, seedRange.MinLocation)));
        var rangesSeen = new HashSet<SeedRange>();

        while (explore.Count > 0)
        {
            var range = explore.Dequeue();

            if (!rangesSeen.Contains(range))
            {
                if (range.LocationStart == range.LocationMid && range.LocationStart == range.LocationEnd)
                {
                    currentMinLocation = Math.Min(currentMinLocation, range.MinLocation);
                }
                else
                {
                    foreach (var child in range.Split().Where(child => child.MinLocation < currentMinLocation))
                    {
                        explore.Enqueue(child, child.MinLocation);
                    }
                }

                rangesSeen.Add(range);
            }
        }

        return currentMinLocation;
    }

    record MapperRange(long DestinationStart, long SourceStart, long RangeLength)
    {
        public long Offset { get; } = DestinationStart - SourceStart;

        /// <summary>
        /// End of the source range, inclusive.
        /// </summary>
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

    record SeedRange
    {
        public SeedRange(long start, long length, Pipeline pipeline)
        {
            SeedStart = start;
            SeedEnd = start + length - 1;
            SeedMid = (start + SeedEnd) / 2;
            SeedLength = length;

            LocationStart = pipeline.ConvertSeedToLocation(SeedStart);
            LocationMid = pipeline.ConvertSeedToLocation(SeedMid);
            LocationEnd = pipeline.ConvertSeedToLocation(SeedEnd);
            MinLocation = Math.Min(Math.Min(LocationStart, LocationMid), LocationEnd);

            Pipeline = pipeline;
        }

        public long SeedStart { get; }

        /// <summary>
        /// End of the range, inclusive.
        /// </summary>
        public long SeedEnd { get; }

        public long SeedMid { get; }

        public long SeedLength { get; }

        public long LocationStart { get; }

        public long LocationMid { get; }

        public long LocationEnd { get; }

        public long MinLocation { get; }

        public Pipeline Pipeline { get; }

        public SeedRange[] Split() => [
            new(SeedStart, SeedMid - SeedStart + 1, Pipeline),
            new(SeedMid, SeedEnd - SeedMid + 1, Pipeline)
        ];
    }

    static (long[] Seeds, SeedRange[] SeedRanges, Pipeline) ParseInput(string input)
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

        var pipeline = new Pipeline(mappers);

        var seedRanges = seeds
            .Select((value, index) => new { value, pairId = index / 2 })
            .GroupBy(x => x.pairId)
            .Select(grp => grp.Select(x => x.value).ToArray())
            .Select(pair => new SeedRange(pair.ElementAt(0), pair.ElementAt(1), pipeline))
            .ToArray();

        return new(seeds, seedRanges, pipeline);
    }
}

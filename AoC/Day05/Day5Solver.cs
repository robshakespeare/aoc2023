using System.Diagnostics;

namespace AoC.Day05;

public class Day5Solver : ISolver
{
    public string DayName => "If You Give A Seed A Fertilizer";

    public Action<string>? Logger { get; set; }

    public long? SolvePart1(string input)
    {
        var (seeds, _, pipeline) = ParseInput(input);

        //Console.WriteLine(pipeline.Dump());

        //seeds = [.. seeds, 97, 98, 99, 100, 101, 102, 103];

        var results = seeds.Select(seed => pipeline.ConvertSeedToLocationWithTrace(seed, $"{seed}")).ToArray();

        Console.WriteLine(string.Join(Environment.NewLine + Environment.NewLine, results.Select(res => res.ToDebugInfo())));

        var maxSourceEnd = pipeline.Mappers.SelectMany(mapper => mapper.Ranges).Max(x => x.SourceEnd);

        Console.WriteLine(new { maxSourceEnd }.ToString());

        return results.MinBy(x => x.Location)?.Location;
    }

    public long? SolvePart2(string input)
    {
        var (_, seedRanges, pipeline) = ParseInput(input);

        if (false)
        {
            var overlaps = seedRanges.Count(x => seedRanges.Any(other => x != other && x.Intersects(other)));

            Console.WriteLine("count of overlaps is: " + overlaps);

            if (seedRanges.Length > 2)
            {
                //var results2 = seedRanges
                //    .SelectMany(seedRange => seedRange.GetSeeds().Select(seed => new { seedRange, seed }))
                //    .Select(x => pipeline.ConvertSeedToLocationWithTrace(x.seed, x.seedRange.ToString()))
                //    .Take(1000000)
                //    .ToArray();

                //Console.WriteLine(string.Join(Environment.NewLine + Environment.NewLine, results2.Select(res => res.ToDebugInfo())));

                //return null;
                throw new NotImplementedException("Part 2 actual result breakout!!");
            }

            //Console.WriteLine(seedRanges.Dump());

            var results = seedRanges
                .SelectMany(seedRange => seedRange.GetSeeds().Select(seed => new { seedRange, seed }))
                .Select(x => pipeline.ConvertSeedToLocationWithTrace(x.seed, x.seedRange.ToString())).ToArray();

            Console.WriteLine(string.Join(Environment.NewLine + Environment.NewLine, results.Select(res => res.ToDebugInfo())));
        }

        // Thinking, for each 'seed range', get location of start and end, then mid point. Then use a kind of binary search
        // but optimise, e.g. when a range's start and end and mid are the same result location, then assume it is a whole block, so record that location and drop out
        // Might also be able to exclude things when the start location is greater than the currently known lowest

        var currentMinLocation = long.MaxValue;

        var explore = new PriorityQueue<SeedRange, long>(seedRanges.Select(seedRange => (seedRange, seedRange.MinLocation)));
        var seen = new HashSet<SeedRange>();

        var stopwatch = Stopwatch.StartNew();

        while (explore.Count > 0)
        {
            var node = explore.Dequeue(); // this takes out the top priority node
            //var node = path.CurrentNode;

            //// if node is the goal return the path
            //if (isGoal(node))
            //{
            //    return path;
            //}

            // if we've not already seen the node
            if (!seen.Contains(node))
            {
                if ((node.LocationStart == node.LocationMid && node.LocationStart == node.LocationEnd) || node.Length <= 3) 
                {
                    currentMinLocation = Math.Min(currentMinLocation, node.MinLocation);
                }
                else
                {
                    foreach (var child in node.Split())
                    {
                        if (child.MinLocation < currentMinLocation)
                        {
                            Console.WriteLine($"Adding to explore {child}");

                            explore.Enqueue(child, child.MinLocation);
                        }
                    }
                }

                //foreach (var child in _getSuccessors(node))
                //{
                //    var childPath = path.Append(child);
                //    explore.Enqueue(childPath, childPath.TotalCost + _getHeuristic(child)); // the heuristic is added here as a part of the priority
                //}

                seen.Add(node);
            }

            if (stopwatch.Elapsed > TimeSpan.FromSeconds(0.2))
            {
                break;
            }
        }

        //throw new InvalidOperationException("No paths found");

        //Console.WriteLine(seedRanges.Where(x => x.LocationMid < x.LocationStart || x.LocationEnd < x.LocationStart).Dump());

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

    record MapResult(long Source, long Destination, MapperRange? MatchedRange);

    record Mapper(string Name, MapperRange[] Ranges)
    {
        public MapResult Map(long source)
        {
            foreach (var range in Ranges)
            {
                var destination = range.Map(source);
                if (destination != null)
                {
                    return new(source, destination.Value, range);
                }
            }

            return new(source, source, null);
        }
    }

    record Pipeline(Mapper[] Mappers)
    {
        public long ConvertSeedToLocation(long seed) => Mappers.Aggregate(seed, (acc, mapper) => mapper.Map(acc).Destination);

        public PipelineResult ConvertSeedToLocationWithTrace(long seed, string context)
        {
            var path = Mappers.Aggregate(new MapResult[] { new(seed, seed, null) }, (currentPath, mapper) => [.. currentPath, mapper.Map(currentPath.Last().Destination)]);

            return new(seed, path.Last().Destination, path, context);
        }
    }

    record PipelineResult(long Seed, long Location, MapResult[] Path, string Context)
    {
        public string ToDebugInfo(bool detailed = false) =>
            $"{Context} // Seed: {Seed}, Location: {Location}, Offset Sum: {Path.Sum(p => p.MatchedRange?.Offset ?? 0)}" +
            (detailed ? $", {Environment.NewLine}{string.Join(Environment.NewLine, Path.Select(p => p.ToString()))}" : "");
    }

    record SeedRange //(long Start, long Length)
    {
        public SeedRange(long start, long length, Pipeline pipeline)
        {
            Start = start;
            Length = length;
            Pipeline = pipeline;
            End = start + length - 1;
            Mid = (start + End) / 2;

            LocationStart = pipeline.ConvertSeedToLocation(Start);
            LocationMid = pipeline.ConvertSeedToLocation(Mid);
            LocationEnd = pipeline.ConvertSeedToLocation(End);

            MinLocation = Math.Min(Math.Min(LocationStart, LocationMid), LocationEnd);
        }

        public long Start { get; }

        public long Length { get; }

        public Pipeline Pipeline { get; }

        /// <summary>
        /// End of the range, inclusive.
        /// </summary>
        public long End { get; }

        public long Mid { get; }

        public long LocationStart { get; }

        public long LocationMid { get; }

        public long LocationEnd { get; }

        public long MinLocation { get; }

        public SeedRange[] Split() => [
            new(Start, (Mid - Start) + 1, Pipeline),
            new(Mid, (End - Mid) + 1, Pipeline)
        ];

        public IEnumerable<long> GetSeeds() => Enumerable.Range((int)Start, (int)Length).Select(n => (long)n);

        public bool Intersects(SeedRange other) =>
            (Start >= other.Start && Start <= other.End) ||
            (End >= other.Start && End <= other.End);
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

        return new(seeds, seedRanges, new Pipeline(mappers));
    }
}

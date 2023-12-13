//using static System.Environment;

//namespace AoC.Day13;

//public class Day13Solver : ISolver
//{
//    public string DayName => "Point of Incidence";

//    public long? SolvePart1(string input)
//    {
//        return Grid.ParseInput(input).Sum(grid => grid.GetMirrorLine());

//        //foreach (var grid in input.Split(NewLine + NewLine))
//        //{
//        //    var rowMatch = grid.ReadLines().Select((line, index) => (line, rowNum: index + 1))
//        //        .GroupBy(x => x.line)
//        //        .SingleOrDefault(x => x.Count() == 2 && Math.Abs(x.ElementAt(0).rowNum - x.ElementAt(1).rowNum) == 1);

//        //    if (rowMatch != null)
//        //    {
//        //        Console.WriteLine($"rowMatch: {rowMatch.Key} -- {string.Join(", ", rowMatch)}");
//        //    }
//        //    else
//        //    {
//        //        Console.WriteLine($"No rowMatch");
//        //    }
//        //}
//    }

//    public long? SolvePart2(string input)
//    {
//        return null;
//    }

//    public record Grid(string GridFull, string[] Rows)
//    {
//        public string[] Columns { get; } = GetColumns(Rows);

//        public long GetMirrorLine()
//        {
//            var result = GetMirrorLine(Rows, isHorizontal: true) ?? GetMirrorLine(Columns, isHorizontal: false);

//            if (result == null)
//            {
//                throw new Exception("Couldn't find mirror line");
//            }

//            return result.Value;

//            //return GetMirrorLine(Rows, isHorizontal: true) ?? GetMirrorLine(Columns, isHorizontal: false) ?? throw new Exception("Couldn't find mirror line");
//        }

//        static long? GetMirrorLine(string[] lines, bool isHorizontal)
//        {
//            for (var candidate = 0; candidate < lines.Length - 1; candidate++)
//            {
//                var endReached = false;

//                for (int i = 0; i <= candidate; i++)
//                {
//                    var idx = candidate - i;
//                    var inverseIdx = candidate + 1 + i;

//                    if (inverseIdx >= lines.Length)
//                    {
//                        endReached = true;
//                        break;
//                    }

//                    if (lines[idx] == lines[inverseIdx])
//                    {
//                        //numMatches++;
//                        endReached = idx == 0;
//                    }
//                    else
//                    {
//                        break;
//                    }
//                }

//                if (endReached) // && numMatches > 0)
//                {
//                    return (candidate + 1) * (isHorizontal ? 100L : 1L);
//                }
//            }

//            //foreach (var candidate in lines
//            //    .Select((line, index) => (line, index, num: index + 1))
//            //    .ContiguousGroupBy(x => x.line)
//            //    .Where(grp => grp.Count() == 2))
//            //{
//            //    // Go back, getting matches. Until forward is out of range
//            //    // Must have at least one match

//            //    //var numMatches = 0;
//            //    var endReached = candidate.ElementAt(0).index == 0;

//            //    for (int i = 1; i <= candidate.ElementAt(0).index; i++)
//            //    {
//            //        var idx = candidate.ElementAt(0).index - i;
//            //        var inverseIdx = candidate.ElementAt(1).index + i;

//            //        if (inverseIdx >= lines.Length)
//            //        {
//            //            endReached = true;
//            //            break;
//            //        }

//            //        if (lines[idx] == lines[inverseIdx])
//            //        {
//            //            //numMatches++;
//            //            endReached = idx == 0;
//            //        }
//            //        else
//            //        {
//            //            break;
//            //        }
//            //    }

//            //    if (endReached) // && numMatches > 0)
//            //    {
//            //        return candidate.ElementAt(0).num * (isHorizontal ? 100L : 1L);
//            //    }
//            //}

//            return null;
//        }

//        public static Grid Parse(string grid) => new(grid, grid.ReadLines().ToArray());

//        public static Grid[] ParseInput(string input) => input.Split(NewLine + NewLine).Select(Parse).ToArray();

//        static string[] GetColumns(string[] Rows)
//        {
//            string GetColumn(int x) => string.Concat(Rows.Select(line => line[x]));
//            return Enumerable.Range(0, Rows[0].Length).Select(GetColumn).ToArray();
//        }
//    }
//}

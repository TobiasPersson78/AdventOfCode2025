using System.Text.RegularExpressions;

bool useExampleInput = false;

string inputFilename = useExampleInput
    ? "exampleInput.txt"
    : "input.txt";

List<long> ids = File
    .ReadAllText(inputFilename)
    .Split(',')
    .Select(range => range.Split('-'))
    .SelectMany(range => LongRange(long.Parse(range[0]), long.Parse(range[1])))
    .ToList();
List<long> results = new[] { @"^(\d+)\1$", @"^(\d+)\1+$" }
    .Select(pattern => ids
        .AsParallel()
        .Where(id => Regex.IsMatch(id.ToString(), pattern))
        .Sum())
    .ToList();

Console.WriteLine("Day 2A");
Console.WriteLine($"Sum of invalid ids: {results[0]}"); // 1227775554, 29818212493

Console.WriteLine("Day 2B");
Console.WriteLine($"Sum of invalid ids: {results[1]}"); // 4174379265, 37432260594

IEnumerable<long> LongRange(long fromInclusive, long toInclusive)
{
    for (long index = fromInclusive; index <= toInclusive; index++)
        yield return index;
}

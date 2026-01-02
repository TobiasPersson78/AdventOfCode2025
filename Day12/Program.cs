using System.Collections.Immutable;

bool useExampleInput = false;

string inputFilename = useExampleInput
    ? "exampleInput.txt"
    : "input.txt";

(ImmutableList<Present> presents, ImmutableList<Region> regions) = File
    .ReadAllText(inputFilename)
    .Replace("\r\n", "\n")
    .Split("\n\n")
    .Aggregate(
        (Presents: ImmutableList<Present>.Empty, Regions: ImmutableList<Region>.Empty),
        (acc, curr) =>
        {
            string[] linesInBlock = curr.Trim().Split('\n');

            return linesInBlock[0].EndsWith(':')
                ? (acc.Presents.Add(new Present(linesInBlock[1..].ToArray())), acc.Regions)
                : (acc.Presents,
                    linesInBlock
                        .Select(line => line.Split([':', ' '], StringSplitOptions.RemoveEmptyEntries))
                        .Select(parts =>
                            (WidthAndHeight: parts[0].Split('x').Select(int.Parse).ToArray(),
                                Quantities: parts[1..].Select(int.Parse).ToArray()))
                        .Select(item => new Region(item.WidthAndHeight[0], item.WidthAndHeight[1], item.Quantities))
                        .ToImmutableList());
        }
    );
List<SolutionType> solutionTypes = [..regions.Select(GetSolutionType)];
int triviallySolvableCount = solutionTypes.Count(solutionType => solutionType == SolutionType.TriviallySolvable);
bool hasUnknownSolutions = solutionTypes.Any(solutionType => solutionType == SolutionType.Unknown);

Console.WriteLine("Day 12A");
Console.WriteLine($"Regions that can fit all of the presents: {(hasUnknownSolutions ? "Unknown" : triviallySolvableCount)} "); // 2 (but will print "Unknown"), 517

SolutionType GetSolutionType(Region region) =>
    region switch
    {
        _ when (region.Width / 3) * (region.Height / 3) >= region.Quantities.Sum() => SolutionType.TriviallySolvable,
        _ when region.Width * region.Height
            < region.Quantities.Index().Sum(tuple => tuple.Item * presents[tuple.Index].SpaceCount)
            => SolutionType.Unsolvable,
        _ => SolutionType.Unknown
    };

readonly record struct Present(string[] Shape)
{
    public int SpaceCount { get; init; } = Shape.Sum(row => row.Count(column => column == '#'));
}

readonly record struct Region(int Width, int Height, int[] Quantities);

enum SolutionType
{
    TriviallySolvable,
    Unsolvable,
    Unknown
}

bool useExampleInput = false;

string inputFilename = useExampleInput
    ? "exampleInput.txt"
    : "input.txt";

string[] matrix = File.ReadAllLines(inputFilename);
Dictionary<Position, long> timelineCountForPosition = Enumerable
    .Range(0, matrix[0].Length)
    .ToDictionary(column => new Position(matrix.Length - 1, column), _ => 1L);
HashSet<Position> visitedSplits = new();
long timelineCount = GetTimelineCountForPosition(new Position(0, matrix[0].IndexOf('S')));

Console.WriteLine("Day 7A");
Console.WriteLine($"Number of splits: {visitedSplits.Count}"); // 21, 1581
Console.WriteLine("Day 7B");
Console.WriteLine($"Number of different timelines: {timelineCount}"); // 40, 73007003089792

long GetTimelineCountForPosition(Position position)
{
    if (timelineCountForPosition.TryGetValue(position, out long memoizedCount))
        return memoizedCount;

    if (matrix[position.Row + 1][position.Column] == '^')
        visitedSplits.Add(position with { Row = position.Row + 1 });

    return timelineCountForPosition[position] = matrix[position.Row + 1][position.Column] != '^'
        ? GetTimelineCountForPosition(position with { Row = position.Row + 1 })
        : GetTimelineCountForPosition(new Position(position.Row + 1, position.Column - 1)) +
            GetTimelineCountForPosition(new Position(position.Row + 1, position.Column + 1));
};

readonly record struct Position(int Row, int Column);

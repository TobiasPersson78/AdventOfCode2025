using System.Collections.Immutable;

bool useExampleInput = false;

(string inputFilenamePartA, string inputFilenamePartB) = useExampleInput
    ? ("exampleInputPartA.txt", "exampleInputPartB.txt")
    : ("input.txt", "input.txt");

long resultPartA = CountPaths(ReadDeviceConnections(inputFilenamePartA), "you", "out");
Dictionary<string, HashSet<string>> deviceConnectionsPartB = ReadDeviceConnections(inputFilenamePartB);
long resultPartB =
    CountPaths(deviceConnectionsPartB, "svr", "dac") *
    CountPaths(deviceConnectionsPartB, "dac", "fft") *
    CountPaths(deviceConnectionsPartB, "fft", "out")
    +
    CountPaths(deviceConnectionsPartB, "svr", "fft") *
    CountPaths(deviceConnectionsPartB, "fft", "dac") *
    CountPaths(deviceConnectionsPartB, "dac", "out");

Console.WriteLine("Day 11A");
Console.WriteLine($"Number of paths to out: {resultPartA}"); // 5, 511
Console.WriteLine("Day 11B");
Console.WriteLine($"Number of paths from svr by fft and dac to out: {resultPartB}"); // 2, 458618114529380

Dictionary<string, HashSet<string>> ReadDeviceConnections(string filename) => File
    .ReadAllLines(filename)
    .Select(line => line.Split([':', ' '], StringSplitOptions.RemoveEmptyEntries))
    .ToDictionary(item => item[0], item => item[1..].ToHashSet());

long CountPaths(Dictionary<string, HashSet<string>> deviceConnections, string startNode, string endNode)
{
    Dictionary<string, long> branchCountForNode = new(){{endNode, 1}};

    long PathsToEndNode(string currentNode, ImmutableHashSet<string> visited)
    {
        if (branchCountForNode.TryGetValue(currentNode, out long count))
            return count;

        count = deviceConnections.TryGetValue(currentNode, out HashSet<string>? connections)
            ? connections
                .Where(item => !visited.Contains(item))
                .Sum(connection => PathsToEndNode(connection, visited.Add(currentNode)))
            : 0L;

        branchCountForNode[currentNode] = count;
        return count;
    }

    return PathsToEndNode(startNode, ImmutableHashSet<string>.Empty);
}

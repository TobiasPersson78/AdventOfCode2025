bool useExampleInput = false;

(string inputFilename, int maxConnections) = useExampleInput
    ? ("exampleInput.txt", 10)
    : ("input.txt", 1_000);

List<Position3d> positions = File
    .ReadAllLines(inputFilename)
    .Select(line => line.Split(','))
    .Select(array => new Position3d(long.Parse(array[0]), long.Parse(array[1]), long.Parse(array[2])))
    .ToList();
List<PositionsDistance> distancesBetweenPositions =
    (from i in Enumerable.Range(0, positions.Count - 1)
     from j in Enumerable.Range(i + 1, positions.Count - (i + 1))
     select new PositionsDistance(positions[i], positions[j], SquaredDistance(positions[i], positions[j])))
    .OrderBy(distance => distance.SquaredDistance)
    .ToList();
Dictionary<Position3d, HashSet<Position3d>> circuitsForPositions = positions
    .ToDictionary(position => position, position => new HashSet<Position3d>([position]));
foreach (PositionsDistance positionsDistance in distancesBetweenPositions.Take(maxConnections))
    _ = JoinNotConnectedCircuits(positionsDistance, circuitsForPositions);
long resultPartA = circuitsForPositions
    .Values
    .Distinct()
    .Select(set => set.Count)
    .OrderByDescending(count => count)
    .Take(3)
    .Aggregate(1L, (acc, curr) => acc * curr);
long resultPartB = 0;
foreach (PositionsDistance positionsDistance in distancesBetweenPositions.Skip(maxConnections))
{
    if (JoinNotConnectedCircuits(positionsDistance, circuitsForPositions).Count == positions.Count)
    {
        resultPartB = positionsDistance.A.X * positionsDistance.B.X;
        break;
    }
}

Console.WriteLine("Day 8A");
Console.WriteLine($"Product of the largest circuits: {resultPartA}"); // 40, 29406
Console.WriteLine("Day 8B");
Console.WriteLine($"Product of the last X coordinate to join: {resultPartB}"); // 25272, 7499461416

long SquaredDistance(Position3d a, Position3d b) =>
    (a.X - b.X) * (a.X - b.X) +
    (a.Y - b.Y) * (a.Y - b.Y) +
    (a.Z - b.Z) * (a.Z - b.Z);

HashSet<Position3d> JoinNotConnectedCircuits(PositionsDistance positionsDistance1, Dictionary<Position3d, HashSet<Position3d>> dictionary)
{
    (Position3d a, Position3d b, _) = positionsDistance1;
    HashSet<Position3d> circuitA = dictionary[a];
    if (!circuitA.Contains(b))
    {
        HashSet<Position3d> circuitB = dictionary[b];
        foreach (Position3d positionInCircuit in circuitB)
            dictionary[positionInCircuit] = circuitA;
        circuitA.UnionWith(circuitB);
    }

    return circuitA;
}

readonly record struct Position3d(long X, long Y, long Z);
readonly record struct PositionsDistance(Position3d A, Position3d B, long SquaredDistance);

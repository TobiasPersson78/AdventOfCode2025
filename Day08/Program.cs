bool useExampleInput = true;

string inputFilename = useExampleInput
    ? "exampleInput.txt"
    : "input.txt";

List<Position3d> positions = File
    .ReadAllLines(inputFilename)
    .Select(line => line.Split(','))
    .Select(array => new Position3d(long.Parse(array[0]), long.Parse(array[1]), long.Parse(array[2])))
    .ToList();
List<PositionsDistance> distances =
    (from i in Enumerable.Range(0, positions.Count - 1)
     from j in Enumerable.Range(i + 1, positions.Count - (i + 1))
     select new PositionsDistance(positions[i], positions[j], SquaredDistance(positions[i], positions[j])))
    .OrderBy(distance => distance.SquaredDistance)
    .ToList();
Dictionary<Position3d, HashSet<Position3d>> circuitsForPositions = positions
    .ToDictionary(position => position, position => new HashSet<Position3d>([position]));
int maxConnections = useExampleInput
    ? 10
    : 1_000;
foreach (PositionsDistance distance in distances.Take(maxConnections))
{
    (Position3d a, Position3d b, _) = distance;
    HashSet<Position3d> circuitA = circuitsForPositions[a];
    if (!circuitA.Contains(b))
    {
        circuitA.UnionWith(circuitsForPositions[b]);
        foreach (Position3d positionInCircuit in circuitA)
            circuitsForPositions[positionInCircuit] = circuitA;
    }
}
long resultPartA = circuitsForPositions
    .Values
    .Distinct()
    .Select(set => set.Count)
    .OrderByDescending(count => count)
    .Take(3)
    .Aggregate(1L, (acc, curr) => acc * curr);

Console.WriteLine("Day 8A");
Console.WriteLine($"Product of the largest circuits: {resultPartA}"); // 40, 29406

long SquaredDistance(Position3d a, Position3d b) =>
    (a.X - b.X) * (a.X - b.X) +
    (a.Y - b.Y) * (a.Y - b.Y) +
    (a.Z - b.Z) * (a.Z - b.Z);

readonly record struct Position3d(long X, long Y, long Z);
readonly record struct PositionsDistance(Position3d A, Position3d B, long SquaredDistance);

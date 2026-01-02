bool useExampleInput = false;

string inputFilename = useExampleInput
    ? "exampleInput.txt"
    : "input.txt";

List<string> grid = [.. File.ReadAllLines(inputFilename)];
Dictionary<Position, int> neighborRollsForPosition =
    (from row in Enumerable.Range(0, grid.Count)
     from column in Enumerable.Range(0, grid[0].Length)
     where grid[row][column] == '@'
     select new Position(row, column))
    .ToDictionary(
        position => position,
        position => GetNeighbors(grid, position).Count(neighbor => grid[neighbor.Row][neighbor.Column] == '@'));

List<int> rollsRemovedCount = new();
do
{
    List<Position> positionsToRemove = [..neighborRollsForPosition
        .Where(item => item.Value < 4)
        .Select(item => item.Key)];
    rollsRemovedCount.Add(positionsToRemove.Count);

    foreach (Position position in positionsToRemove)
    {
        neighborRollsForPosition.Remove(position);
        foreach (Position neighbor
            in GetNeighbors(grid, position)
                .Where(neighbor => neighborRollsForPosition.ContainsKey(neighbor)))
        {
            neighborRollsForPosition[neighbor]--;
        }
    }
}
while (rollsRemovedCount.Last() > 0);

Console.WriteLine("Day 4A");
Console.WriteLine($"Rolls of paper immediately accessible by forklift: {rollsRemovedCount.First()}"); // 13, 1489
Console.WriteLine("Day 4B");
Console.WriteLine($"Rolls of paper accessible by forklift after removals: {rollsRemovedCount.Sum()}"); // 43, 8890

IEnumerable<Position> GetNeighbors(List<string> grid, Position position) =>
    from rowOffset in Enumerable.Range(-1, 3)
    from columnOffset in Enumerable.Range(-1, 3)
    let neighbor = new Position(position.Row + rowOffset, position.Column + columnOffset)
    where neighbor.Row >= 0 && neighbor.Row < grid.Count
    where neighbor.Column >= 0 && neighbor.Column < grid[0].Length
    where neighbor != position
    select neighbor;

record struct Position(int Row, int Column);

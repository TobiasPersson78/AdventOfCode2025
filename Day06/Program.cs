bool useExampleInput = true;

string inputFilename = useExampleInput
    ? "exampleInput.txt"
    : "input.txt";

string[] lines = File.ReadAllLines(inputFilename);
string[] operandLines = lines[..^1].ToArray();
string[] operations = lines[^1].Split(' ', StringSplitOptions.RemoveEmptyEntries);
List<List<long>> operandsPartA = operandLines
    .Select(line => line
        .Split(' ', StringSplitOptions.RemoveEmptyEntries)
        .Select(long.Parse)
        .ToList())
    .ToList();
List<List<long>> operandsPartB =
    (from columnIndex in Enumerable.Range(0, operandLines[0].Length)
     select (from line in operandLines
             select line[columnIndex]))
    .Select(column => string.Join(string.Empty, column).Trim())
    .Aggregate(
        new List<List<long>> { new () },
        (acc, curr) =>
        {
            if (curr == string.Empty)
                acc.Add(new List<long>());
            else
                acc[^1].Add(long.Parse(curr));
            return acc;
        });
Func<int, IEnumerable<long>> getOperandsForProblemPartA = (int index) => operandsPartA.Select(row => row[index]);
Func<int, IEnumerable<long>> getOperandsForProblemPartB = (int index) => operandsPartB[index];
List<long> results = new[] { getOperandsForProblemPartA, getOperandsForProblemPartB }
    .Select(getOperands => operations
        .Index()
        .Sum(operationAndIndex => operationAndIndex.Item == "*"
            ? getOperands(operationAndIndex.Index).Aggregate(1L, (acc, curr) => acc * curr)
            : getOperands(operationAndIndex.Index).Sum()))
    .ToList();

Console.WriteLine("Day 6A");
Console.WriteLine($"Grand total: {results[0]}"); // 4277556, 4722948564882
Console.WriteLine("Day 6A");
Console.WriteLine($"Grand total: {results[1]}"); // 3263827, 9581313737063

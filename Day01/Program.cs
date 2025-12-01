bool useExampleInput = false;

string inputFilename = useExampleInput
    ? "exampleInput.txt"
    : "input.txt";

(int Position, int ZeroCount, int ZeroPasses) result = File
    .ReadAllLines(inputFilename)
    .Select(line => (line[0] == 'R' ? 1 : -1) * int.Parse(line[1..]))
    .Aggregate(
        (Position: 50, ZeroCount: 0, ZeroPasses: 0),
        (acc, curr) =>
        {
            acc.ZeroPasses += Math.Abs(curr) / 100;
            curr %= 100;
            int newPosition = acc.Position + curr;
            if (newPosition <= 0 && acc.Position != 0)
                acc.ZeroPasses++;
            if (newPosition < 0)
                newPosition += 100;
            if (newPosition >= 100)
            {
                acc.ZeroPasses++;
                newPosition -= 100;
            }
            acc.Position = newPosition;
            acc.ZeroCount += (acc.Position == 0 ? 1 : 0);
                
            return acc;
        });

Console.WriteLine("Day 1A");
Console.WriteLine($"Password: {result.ZeroCount}"); // 3, 1074

Console.WriteLine("Day 1B");
Console.WriteLine($"Password: {result.ZeroPasses}"); // 6, 6254

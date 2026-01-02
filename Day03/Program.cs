bool useExampleInput = false;

string inputFilename = useExampleInput
    ? "exampleInput.txt"
    : "input.txt";

List<char[]> batteryBanks = [..File
    .ReadAllLines(inputFilename)
    .Select(line => line.ToCharArray())];
List<long> results = [..new[] { 2, 12 }
    .Select(numberOfSwitchesPerBank =>
        batteryBanks
            .Sum(bank => Enumerable
                .Range(0, numberOfSwitchesPerBank)
                .Reverse()
                .Aggregate(
                    (PreviousIndex: -1, Total: 0L),
                    (acc, curr) =>
                    {
                        (int remainingMax, int index) = bank[(acc.PreviousIndex + 1)..^curr]
                            .Select((character, index) => (Value: character - '0', Index: index))
                            .MaxBy(item => item.Value);
                        return (acc.PreviousIndex + 1 + index, acc.Total * 10 + remainingMax);
                    })
                .Total))];

Console.WriteLine("Day 3A");
Console.WriteLine($"Total output voltage: {results[0]}"); // 357, 17343
Console.WriteLine("Day 3B");
Console.WriteLine($"Total output voltage: {results[1]}"); // 3121910778619, 172664333119298

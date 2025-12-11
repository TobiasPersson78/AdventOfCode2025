using System.Collections.Immutable;
using System.Text.RegularExpressions;

bool useExampleInput = false;

string inputFilename = useExampleInput
    ? "exampleInput.txt"
    : "input.txt";

List<Machine> lines = File
    .ReadAllLines(inputFilename)
    .Select(line => Regex.Match(line, @"\[(?<lights>[\.#]+)\] (?<buttons>.+) \{(?<joltage>(\d+,)+\d+)\}"))
    .Select(match => new Machine(
        match
            .Groups["lights"]
            .Value
            .Reverse()
            .Aggregate(
                0,
                (acc, curr) => (acc << 1) + (curr == '#' ? 1 : 0)),
        Regex
            .Matches(
                match.Groups["buttons"].Value,
                @"\(([\d,]+)\)")
            .Select(match => match.Groups[1].Value)
            .Select(button => button
                .Split(',')
                .Select(int.Parse)
                .Aggregate(
                    0,
                    (acc, curr) => acc + (1 << curr)))
            .ToList(),
        Regex
            .Matches(
                match.Groups["joltage"].Value,
                 @"\d+")
            .Select(match => match.Groups[0].Value)
            .Select(int.Parse)
            .ToList()))
    .ToList();
long resultPartA = lines.AsParallel().Sum(machine => MinimumPressesToReachTarget(machine));

Console.WriteLine("Day 9A");
Console.WriteLine($"Fewest total presses: {resultPartA}"); // 7, 578

int MinimumPressesToReachTarget(in Machine machine)
{
    Queue<(int NumberOfPresses, int currentState)> queue = new([(0, 0)]);
    while (true)
    {
        (int numberOfPresses, int currentState) = queue.Dequeue();

        ++numberOfPresses;
        foreach (int button in machine.Buttons)
        {
            if (machine.Target == (currentState ^ button))
                return numberOfPresses;

            queue.Enqueue((numberOfPresses, currentState ^ button));
        }
    }
}

readonly record struct Machine(int Target, List<int> Buttons, List<int> Joltage);

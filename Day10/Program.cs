using System.Collections.Concurrent;
using System.Collections.Immutable;

bool useExampleInput = false;

string inputFilename = useExampleInput
    ? "exampleInput.txt"
    : "input.txt";

(int resultPartA, int resultPartB) = File
    .ReadAllLines(inputFilename)
    .Select(line => line.Split(' '))
    .Select(lineParts => new Machine(
        [..lineParts[0][1..^1].Select(item => item == '#')],
        [..lineParts[1..^1]
            .Select(buttonPart => buttonPart[1..^1]
                .Split(',')
                .Select(int.Parse)
                .Aggregate(
                    0,
                    (acc, curr) => acc + (1 << curr)))],
        [..lineParts[^1][1..^1]
            .Split(',')
            .Select(int.Parse)]))
    .Select(machine => (Machine: machine, SinglePressesCombinations: GetSimpleButtonCombinations(machine)))
    .Aggregate(
        (ResultPartA: 0, ResultPartB: 0),
        (acc, curr) =>
            (acc.ResultPartA + MinimumPressesPartA(curr.Machine.LightIsOnStatuses, curr.SinglePressesCombinations),
                acc.ResultPartB + MinimumPressesPartB(
                    curr.Machine.Joltages,
                    curr.SinglePressesCombinations,
                    new ConcurrentDictionary<string, int>())));

ImmutableList<SimpleButtonCombination> GetSimpleButtonCombinations(Machine machine) => [..Enumerable
    .Range(0, 1 << machine.Buttons.Count)
    .Select(buttonMask => new SimpleButtonCombination(
        buttonMask,
        [..machine
            .Buttons
            .Where((item, index) => (buttonMask & 1 << index) != 0)
            .SelectMany(button => Enumerable
                .Range(0, machine.Joltages.Count)
                .Where(index => (button & 1 << index) != 0))
            .Aggregate(
                new int[machine.Joltages.Count].ToImmutableArray(),
                (acc, curr) => acc.SetItem(curr, acc[curr]+1))]))];

int MinimumPressesPartA(
    ImmutableList<bool> lightIsOnStatuses,
    ImmutableList<SimpleButtonCombination> singlePressesCombinations) =>
    singlePressesCombinations
        .Where(item => Enumerable
            .SequenceEqual(
                lightIsOnStatuses,
                item
                    .JoltageChange
                    .Select(item => (item & 1) != 0))) // An odd joltage change means the light is on.
        .Min(item => int.PopCount(item.Buttons));

int MinimumPressesPartB(
    ImmutableList<int> joltages,
    ImmutableList<SimpleButtonCombination> singlePressesCombinations,
    ConcurrentDictionary<string, int> memoizedCount) =>
    joltages.All(item => item == 0)
        ? 0
        : memoizedCount.GetOrAdd(
            string.Join(',', joltages),
            _ => singlePressesCombinations
                .Select(combination =>
                    (ButtonCount: int.PopCount(combination.Buttons),
                        UpdatedJoltages: joltages
                            .Zip(combination.JoltageChange)
                            .Select(pair => pair.First - pair.Second)
                            .ToList()))
                .Where(buttonCountAndUpdatedJoltages => buttonCountAndUpdatedJoltages
                    .UpdatedJoltages
                    .All(item => item >= 0 && (item & 1) == 0))
                .Select(buttonCountAndUpdatedJoltages =>
                    buttonCountAndUpdatedJoltages.ButtonCount +
                    2 * MinimumPressesPartB(
                        [..buttonCountAndUpdatedJoltages
                            .UpdatedJoltages
                            .Select(item => item / 2)],
                        singlePressesCombinations,
                        memoizedCount))
                .Append(314_159_265) // Implicit step count if there's no solution for the current branch. Must be larger than any solution value.
                .Min());

Console.WriteLine("Day 10A");
Console.WriteLine($"Fewest total presses: {resultPartA}"); // 7, 578
Console.WriteLine("Day 10B");
Console.WriteLine($"Fewest total presses: {resultPartB}"); // 33, 20709

readonly record struct SimpleButtonCombination(int Buttons, ImmutableList<int> JoltageChange);
readonly record struct Machine(ImmutableList<bool> LightIsOnStatuses, ImmutableList<int> Buttons, ImmutableList<int> Joltages);

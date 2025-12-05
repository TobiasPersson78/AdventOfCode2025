using System.Text.RegularExpressions;

bool useExampleInput = false;

string inputFilename = useExampleInput
    ? "exampleInput.txt"
    : "input.txt";

(List<LongRange> ranges, List<long> ids) = File
    .ReadAllLines(inputFilename)
    .Aggregate(
        (Ranges: new List<LongRange>(), Ids: new List<long>()),
        (acc, curr) =>
        {
            MatchCollection matches = Regex.Matches(curr, @"\d+");

            if (matches.Count == 2)
                acc.Ranges.Add(new LongRange(long.Parse(matches[0].Value), long.Parse(matches[1].Value)));
            if (matches.Count == 1)
                acc.Ids.Add(long.Parse(matches[0].Value));
            return acc;
        });
for (int aIndex = 0; aIndex < ranges.Count; aIndex++)
{
    for (int bIndex = ranges.Count - 1; bIndex > aIndex; bIndex--)
    {
        (LongRange a, LongRange b) = (ranges[aIndex], ranges[bIndex]);

        if ((b.From <= a.To && b.To >= a.From) || // Overlap
            (b.From >= a.From && b.To <= a.To) || // b inside a
            (a.From >= b.From && a.To <= b.To)) // a inside b
        {
            ranges[aIndex] = new LongRange(
                Math.Min(a.From, b.From),
                Math.Max(a.To, b.To));
            ranges.RemoveAt(bIndex);
            bIndex = ranges.Count; // Restart inner loop
        }
    }
}

int resultPartA = ids.Count(id => ranges.Any(range => range.From <= id && range.To >= id));
long resultPartB = ranges.Sum(range => range.To - range.From + 1);

Console.WriteLine("Day 5A");
Console.WriteLine($"Number of fresh ingredients: {resultPartA}"); // 3, 577
Console.WriteLine("Day 5B");
Console.WriteLine($"Number of fresh ingredient ids: {resultPartB}"); // 14, 350513176552950

record LongRange(long From, long To);

bool useExampleInput = false;

string inputFilename = useExampleInput
    ? "exampleInput.txt"
    : "input.txt";

List<Point> points = [..File
    .ReadAllLines(inputFilename)
    .Select(line => line.Split(','))
    .Select(array => new Point(long.Parse(array[0]), long.Parse(array[1])))];
List<(Rectangle Rectangle, long Area)> rectanglesAndAreas =
    [..(from i in Enumerable.Range(0, points.Count - 1)
        from j in Enumerable.Range(i + 1, points.Count - (i + 1))
        let rectangle = new Rectangle(
            new Point(Math.Min(points[i].X, points[j].X), Math.Min(points[i].Y, points[j].Y)),
            new Point(Math.Max(points[i].X, points[j].X), Math.Max(points[i].Y, points[j].Y)))
        select (Rectangle: rectangle, Area: RectangleArea(rectangle)))
        .OrderByDescending(rectangleAndArea => rectangleAndArea.Area)];
long resultPartA = rectanglesAndAreas.First().Area;
List<Line> perimeter = [..points
    .Zip([..points[1..], points[0]], (a, b) => new Line(a, b))];
long resultPartB = rectanglesAndAreas // Not a general solution, only works for this specific problem input
    .First(item => !perimeter.Any(line => RectangleCrossesLine(item.Rectangle, line)))
    .Area;

Console.WriteLine("Day 9A");
Console.WriteLine($"Largest rectangle area: {resultPartA}"); // 50, 4782896435
Console.WriteLine("Day 9B");
Console.WriteLine($"Largest rectangle area: {resultPartB}"); // 24, 1540060480

long RectangleArea(Rectangle rectangle) =>
    (rectangle.LowerRight.X - rectangle.UpperLeft.X + 1) * (rectangle.LowerRight.Y - rectangle.UpperLeft.Y + 1);

bool RectangleCrossesLine(Rectangle rectangle, Line line) =>
    rectangle.UpperLeft.X < Math.Max(line.PointA.X, line.PointB.X) &&
    rectangle.UpperLeft.Y < Math.Max(line.PointA.Y, line.PointB.Y) &&
    rectangle.LowerRight.X > Math.Min(line.PointA.X, line.PointB.X) &&
    rectangle.LowerRight.Y > Math.Min(line.PointA.Y, line.PointB.Y);

readonly record struct Point(long X, long Y);
readonly record struct Line(Point PointA, Point PointB);
readonly record struct Rectangle(Point UpperLeft, Point LowerRight);

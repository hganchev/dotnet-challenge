using SOLID.DependencyInversion;
using SOLID.InterfaceSegregation;
using SOLID.LiskovSubstitution;
using SOLID.OpenClosed;
using SOLID.SingleResponsibility;

// -----------------------------------------------------------------------
// SOLID Principles Demo
// -----------------------------------------------------------------------

Console.WriteLine("=== SRP — Single Responsibility Principle ===");
var badInvoice = new InvoiceBad("Alice", [49.99m, 15.00m]);
Console.WriteLine($"  [Bad]  Invoice + Print + Save all in one class: {badInvoice.Print()}");

var invoice = new Invoice("Alice", [49.99m, 15.00m]);
var printer = new InvoicePrinter();
var rendered = printer.Print(invoice);
Console.WriteLine($"  [Good] Invoice/Printer/Repository split: {rendered}");

Console.WriteLine("\n=== OCP — Open/Closed Principle ===");
var badCalculator = new AreaCalculatorBad();
Console.WriteLine($"  [Bad]  Rectangle area via switch-based calculator: {badCalculator.CalculateArea(new AreaCalculatorBad.Rectangle { Width = 4, Height = 5 }):F2}");

var calculator = new AreaCalculator();
IShape[] shapes = [new SOLID.OpenClosed.Rectangle(4, 5), new Circle(3), new Triangle(6, 4)];
Console.WriteLine($"  [Good] Total area across Rectangle/Circle/Triangle (new type, no edits): {calculator.CalculateTotalArea(shapes):F2}");

Console.WriteLine("\n=== LSP — Liskov Substitution Principle ===");
SOLID.LiskovSubstitution.Rectangle badRectangle = new Square { Width = 5 };
badRectangle.Height = 10; // Silently also changes Width because Square enforces width == height.
Console.WriteLine($"  [Bad]  Square-as-Rectangle: set Height=10 after Width=5 → Width is now {badRectangle.Width} (expected 5, area is wrong: {badRectangle.CalculateArea()})");

IQuadrilateral[] quadrilaterals = [new FixedRectangle(5, 10), new FixedSquare(5)];
foreach (var q in quadrilaterals)
    Console.WriteLine($"  [Good] {q.GetType().Name} area: {q.CalculateArea()}");

Console.WriteLine("\n=== ISP — Interface Segregation Principle ===");
var badPrinter = new OldFashionedPrinterBad();
badPrinter.Print("Report.pdf");
try
{
    badPrinter.Scan("Report.pdf");
}
catch (NotSupportedException ex)
{
    Console.WriteLine($"  [Bad]  Forced to implement Scan() just to throw: {ex.Message}");
}

IPrinter simplePrinter = new OldFashionedPrinter();
simplePrinter.Print("Report.pdf");
var allInOne = new AllInOnePrinter();
Console.WriteLine("  [Good] OldFashionedPrinter only implements IPrinter; AllInOnePrinter opts into IPrinter+IScanner+IFax.");

Console.WriteLine("\n=== DIP — Dependency Inversion Principle ===");
var badService = new OrderServiceBad();
badService.PlaceOrder("Order-Bad-1");
Console.WriteLine("  [Bad]  OrderServiceBad new()'s a concrete SqlOrderRepository — cannot swap or unit test.");

var goodService = new OrderService(new InMemoryOrderRepository());
goodService.PlaceOrder("Order-Good-1");
Console.WriteLine("  [Good] OrderService depends on IOrderRepository, injected via constructor.");

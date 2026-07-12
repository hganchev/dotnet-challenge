namespace SOLID.InterfaceSegregation;

/// <summary>Violates ISP: a fat interface forces every implementer to provide members it may
/// not need. <see cref="OldFashionedPrinter"/> is forced to implement Scan/Fax with exceptions.</summary>
public interface IMultiFunctionDeviceBad
{
    void Print(string document);
    void Scan(string document);
    void Fax(string document);
}

public sealed class OldFashionedPrinterBad : IMultiFunctionDeviceBad
{
    public void Print(string document) => Console.WriteLine($"  Printing: {document}");

    // Forced to implement members this device cannot support.
    public void Scan(string document) => throw new NotSupportedException("This printer cannot scan.");
    public void Fax(string document) => throw new NotSupportedException("This printer cannot fax.");
}

/// <summary>Fix: split the fat interface into small, focused ones. Consumers depend only on
/// the capability they actually need.</summary>
public interface IPrinter
{
    void Print(string document);
}

public interface IScanner
{
    void Scan(string document);
}

public interface IFax
{
    void Fax(string document);
}

public sealed class OldFashionedPrinter : IPrinter
{
    public void Print(string document) => Console.WriteLine($"  Printing: {document}");
}

public sealed class AllInOnePrinter : IPrinter, IScanner, IFax
{
    public void Print(string document) => Console.WriteLine($"  Printing: {document}");
    public void Scan(string document) => Console.WriteLine($"  Scanning: {document}");
    public void Fax(string document) => Console.WriteLine($"  Faxing: {document}");
}

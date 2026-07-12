namespace SOLID.SingleResponsibility;

/// <summary>
/// Violates SRP: <see cref="InvoiceBad"/> owns calculation, formatting, AND persistence —
/// three separate reasons to change (pricing rules, print layout, storage technology).
/// </summary>
public sealed class InvoiceBad
{
    public string CustomerName { get; }
    public IReadOnlyList<decimal> LineItems { get; }

    public InvoiceBad(string customerName, IReadOnlyList<decimal> lineItems)
    {
        CustomerName = customerName;
        LineItems = lineItems;
    }

    public decimal CalculateTotal() => LineItems.Sum();

    public string Print() => $"Invoice for {CustomerName}: {CalculateTotal():C}";

    public void SaveToDisk(string path) => File.WriteAllText(path, Print());
}

/// <summary>Domain entity — only responsible for holding invoice data and calculating totals.</summary>
public sealed class Invoice
{
    public string CustomerName { get; }
    public IReadOnlyList<decimal> LineItems { get; }

    public Invoice(string customerName, IReadOnlyList<decimal> lineItems)
    {
        CustomerName = customerName;
        LineItems = lineItems;
    }

    public decimal CalculateTotal() => LineItems.Sum();
}

/// <summary>Single responsibility: render an invoice as text.</summary>
public sealed class InvoicePrinter
{
    public string Print(Invoice invoice) => $"Invoice for {invoice.CustomerName}: {invoice.CalculateTotal():C}";
}

/// <summary>Single responsibility: persist an invoice's rendered text.</summary>
public sealed class InvoiceRepository
{
    public void Save(string renderedInvoice, string path) => File.WriteAllText(path, renderedInvoice);
}

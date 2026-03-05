namespace OOP.Interfaces;

// Demonstrates IEquatable<T> and IComparable<T> — the two interfaces most
// commonly required for value-equality and natural ordering.
public class Employee : IEquatable<Employee>, IComparable<Employee>
{
    public int     Id     { get; }
    public string  Name   { get; }
    public decimal Salary { get; }

    public Employee(int id, string name, decimal salary)
    {
        Id     = id;
        Name   = name;
        Salary = salary;
    }

    // IEquatable<T>: type-safe equality, avoids boxing compared to Equals(object)
    public bool Equals(Employee? other) => other is not null && Id == other.Id;

    // Override object.Equals for consistency
    public override bool Equals(object? obj) => Equals(obj as Employee);

    // GetHashCode must be consistent with Equals: same Id → same hash
    public override int GetHashCode() => Id.GetHashCode();

    // Operator overloads to complete the equality contract
    public static bool operator ==(Employee? a, Employee? b) =>
        a is null ? b is null : a.Equals(b);
    public static bool operator !=(Employee? a, Employee? b) => !(a == b);

    // IComparable<T>: defines natural ordering (by Salary ascending)
    public int CompareTo(Employee? other)
    {
        if (other is null) return 1;
        return Salary.CompareTo(other.Salary);
    }

    public override string ToString() => $"Employee #{Id}: {Name} (${Salary:N0})";
}

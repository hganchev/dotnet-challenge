namespace ValueVsReference.Records;

// record struct: value type with compiler-generated value-based equality and 'with'.
public record struct Temperature(double Celsius)
{
    public double Fahrenheit => Celsius * 9.0 / 5.0 + 32;
}

namespace OOP.Polymorphism;

// override: replaces the base implementation in the vtable — truly polymorphic
public class ElectricCar : Vehicle
{
    public int RangeKm { get; }

    public ElectricCar(string make, string model, int rangeKm) : base(make, model)
    {
        RangeKm = rangeKm;
    }

    public override string GetInfo()     => $"{base.GetInfo()} (EV, {RangeKm} km range)";
    public override void   StartEngine() => Console.WriteLine($"{Make} {Model}: silent hum");
}

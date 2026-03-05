namespace OOP.Polymorphism;

// Base class with virtual members — runtime dispatch via vtable.
public class Vehicle
{
    public string Make  { get; }
    public string Model { get; }

    public Vehicle(string make, string model)
    {
        Make  = make;
        Model = model;
    }

    public virtual string GetInfo()      => $"{Make} {Model}";
    public virtual void   StartEngine()  => Console.WriteLine($"{Make} {Model}: vroom");
}


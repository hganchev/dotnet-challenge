namespace OOP.Polymorphism;

// 'new' hides the base member rather than overriding it.
// When called through a Vehicle reference the BASE version runs — NOT polymorphic.
public class SportsCar : Vehicle
{
    public int Horsepower { get; }

    public SportsCar(string make, string model, int horsepower) : base(make, model)
    {
        Horsepower = horsepower;
    }

    // 'new' — deliberately shadows, not overrides
    public new string GetInfo() => $"{Make} {Model} ({Horsepower} HP) [hidden method]";

    // override — this IS polymorphic
    public override void StartEngine() => Console.WriteLine($"{Make} {Model}: ROAR ({Horsepower} HP)");
}

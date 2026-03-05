using OOP.Polymorphism;

namespace OOP.Examples;

public static class PolymorphismDemo
{
    public static void Run()
    {
        Console.WriteLine("\n=== Polymorphism: virtual/override, new hiding, is/as ===");

        Vehicle    generic = new("Toyota",  "Corolla");
        ElectricCar tesla  = new("Tesla",   "Model 3", 560);
        SportsCar  ferrari = new("Ferrari", "F40",     478);

        // --- virtual / override: runtime dispatch ---
        Console.WriteLine("--- StartEngine (virtual/override — always calls the most-derived version) ---");
        Vehicle[] fleet = [generic, tesla, ferrari];
        foreach (var v in fleet)
            v.StartEngine();

        // --- method hiding with 'new' vs override ---
        Console.WriteLine("\n--- GetInfo: new (hiding) vs override ---");
        foreach (var v in fleet)
        {
            // Vehicle reference → calls Vehicle.GetInfo() even for ferrari (hiding is NOT polymorphic)
            Console.WriteLine($"  Vehicle ref:   {v.GetInfo()}");
        }
        // SportsCar reference → calls the hidden GetInfo()
        Console.WriteLine($"  SportsCar ref: {ferrari.GetInfo()}");

        // --- is / as / pattern matching ---
        Console.WriteLine("\n--- is / as pattern matching ---");
        foreach (var v in fleet)
        {
            if (v is ElectricCar ev)
                Console.WriteLine($"  EV: {ev.Make} {ev.Model}, range {ev.RangeKm} km");
            else if (v is SportsCar sc)
                Console.WriteLine($"  Sports: {sc.Make} {sc.Model}, {sc.Horsepower} HP");
            else
                Console.WriteLine($"  Regular: {v.Make} {v.Model}");
        }
    }
}

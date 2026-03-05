namespace OOP.Interfaces;

// IShape: demonstrates a default interface method (C# 8+).
// Implementors get Describe() for free unless they override it.
public interface IShape
{
    double Area();
    double Perimeter();

    // Default interface method — no need for an abstract base class just for shared logic
    string Describe() => $"{GetType().Name}: area={Area():F2}, perimeter={Perimeter():F2}";
}


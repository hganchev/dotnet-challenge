namespace OOP.Inheritance;

// sealed: no further subclassing allowed; compiler can devirtualise calls
public sealed class GoldenRetriever : Dog
{
    public GoldenRetriever(string name, int age) : base(name, age, "Golden Retriever") { }

    public override string Speak() => "Woof woof! (enthusiastic tail wag)";
}

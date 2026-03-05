namespace OOP.Inheritance;

public class Dog : Animal
{
    public string Breed { get; }

    public Dog(string name, int age, string breed) : base(name, age)
    {
        Breed = breed;
    }

    public override string Speak() => "Woof!";

    // Extends the base description rather than replacing it
    public override string Describe() => $"{base.Describe()}, breed: {Breed}";
}

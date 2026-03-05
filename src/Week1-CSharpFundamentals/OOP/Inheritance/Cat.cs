namespace OOP.Inheritance;

public class Cat : Animal
{
    public bool IsIndoor { get; }

    public Cat(string name, int age, bool isIndoor = true) : base(name, age)
    {
        IsIndoor = isIndoor;
    }

    public override string Speak() => "Meow!";

    public override string Describe() => $"{base.Describe()}, indoor: {IsIndoor}";
}

namespace OOP.Inheritance;

// Abstract base class: defines a contract (Speak) and a virtual default (Describe).
// Cannot be instantiated directly — only concrete subclasses can be.
public abstract class Animal
{
    public string Name { get; }
    public int    Age  { get; }

    // Constructor chaining: subclasses call base(name, age)
    protected Animal(string name, int age)
    {
        Name = name;
        Age  = age;
    }

    // abstract: no implementation; every concrete subclass MUST override
    public abstract string Speak();

    // virtual: provides a default implementation that subclasses CAN override
    public virtual string Describe() => $"{GetType().Name} named {Name}, age {Age}";

    public override string ToString() => Describe();
}



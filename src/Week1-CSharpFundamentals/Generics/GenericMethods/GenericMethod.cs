namespace Generics.GenericMethods;

public class GenericMethod
{
    // A generic method that takes a parameter of any type and returns it
    public T Identity<T>(T value)
    {
        return value;
    }

    // A generic method that takes two parameters of the same type and returns the first one
    public T First<T>(T first, T second)
    {
        return first;
    }

    // A generic method that takes an array of any type and returns the first element
    public T FirstElement<T>(T[] array)
    {
        if (array == null || array.Length == 0)
        {
            throw new ArgumentException("Array cannot be null or empty");
        }
        return array[0];
    }
}
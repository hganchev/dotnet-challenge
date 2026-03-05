namespace Generics.GenericMethods;

public static class UtilityMethods
{
    // Swap two values by reference - requires ref parameters
    public static void Swap<T>(ref T a, ref T b)
    {
        T temp = a;
        a = b;
        b = temp;
    }

    // Return the minimum of two comparable values
    // Type inference: compiler infers T from arguments - no need to write Min<int>(3, 5)
    public static T Min<T>(T a, T b) where T : IComparable<T>
        => a.CompareTo(b) <= 0 ? a : b;

    // Return the maximum of two comparable values
    public static T Max<T>(T a, T b) where T : IComparable<T>
        => a.CompareTo(b) >= 0 ? a : b;

    // Clamp a value between min and max
    public static T Clamp<T>(T value, T min, T max) where T : IComparable<T>
    {
        if (value.CompareTo(min) < 0) return min;
        if (value.CompareTo(max) > 0) return max;
        return value;
    }

    // Repeat an action N times - demonstrates Action<> as generic delegate parameter
    public static void Repeat<T>(T value, int times, Action<T> action)
    {
        for (int i = 0; i < times; i++)
            action(value);
    }

    // Project each element - demonstrates Func<> as generic delegate parameter
    public static TResult[] Map<TSource, TResult>(TSource[] source, Func<TSource, TResult> selector)
    {
        var result = new TResult[source.Length];
        for (int i = 0; i < source.Length; i++)
            result[i] = selector(source[i]);
        return result;
    }
}

namespace ClrInternals.Concepts;

/// <summary>
/// A simple generic container used to illustrate how the JIT specialises
/// generic code for value types vs reference types.
///
/// For value-type <c>T</c> (e.g., <c>int</c>, <c>double</c>):
///   the JIT emits a separate native method body per type argument.
///
/// For reference-type <c>T</c> (e.g., <c>string</c>, <c>object</c>):
///   the JIT emits ONE native method body shared across all reference types.
/// </summary>
public sealed class GenericContainer<T>
{
    private T _value;

    public GenericContainer(T value) => _value = value;

    public T Value
    {
        get => _value;
        set => _value = value;
    }

    /// <summary>True when T is a reference type.</summary>
    public bool IsReferenceType => !typeof(T).IsValueType;

    /// <summary>Size of T as seen by the runtime (pointer size for reference types).</summary>
    public int SizeOfT => System.Runtime.CompilerServices.Unsafe.SizeOf<T>();

    public override string ToString() =>
        $"GenericContainer<{typeof(T).Name}>(Value={_value}, IsRef={IsReferenceType}, SizeOf={SizeOfT})";
}

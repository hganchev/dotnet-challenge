namespace ClrInternals.Concepts;

/// <summary>
/// A struct with 128 bytes of data (16 × 8-byte longs).
///
/// The typical guideline: structs larger than ~16 bytes should be passed
/// by <c>in</c> or <c>ref</c> to avoid the copy overhead.
/// Copying this struct on every method call copies 128 bytes.
/// </summary>
public readonly struct LargeStruct
{
    public readonly long A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P;

    public LargeStruct(long seed)
    {
        A = seed;     B = seed + 1;  C = seed + 2;  D = seed + 3;
        E = seed + 4; F = seed + 5;  G = seed + 6;  H = seed + 7;
        I = seed + 8; J = seed + 9;  K = seed + 10; L = seed + 11;
        M = seed + 12; N = seed + 13; O = seed + 14; P = seed + 15;
    }

    public long Sum => A + B + C + D + E + F + G + H + I + J + K + L + M + N + O + P;

    public override string ToString() => $"LargeStruct(Sum={Sum}, {System.Runtime.CompilerServices.Unsafe.SizeOf<LargeStruct>()} bytes)";
}

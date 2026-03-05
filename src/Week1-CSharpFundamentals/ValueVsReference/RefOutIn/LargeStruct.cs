namespace ValueVsReference.RefOutIn;

// A larger struct where 'in' avoids a 16-byte copy on every call.
public readonly struct LargeStruct
{
    public readonly int A, B, C, D;
    public LargeStruct(int a, int b, int c, int d) { A = a; B = b; C = c; D = d; }
}

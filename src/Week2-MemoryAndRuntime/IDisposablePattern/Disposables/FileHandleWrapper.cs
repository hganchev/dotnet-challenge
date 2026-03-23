using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace IDisposablePattern.Disposables;

/// <summary>
/// A <see cref="SafeHandle"/> subclass that wraps a simulated native file handle.
///
/// SafeHandle is preferred over raw <see cref="nint"/> because:
/// - The CLR guarantees ReleaseHandle() is called even in low-resource conditions.
/// - It prevents handle recycling attacks (the handle stays valid until released).
/// - It integrates with P/Invoke marshalling automatically.
///
/// <see cref="SafeHandleZeroOrMinusOneIsInvalid"/> treats handle values of 0 and -1
/// as invalid — matching most Win32 file/event/mutex handles.
/// </summary>
public sealed class FileHandleWrapper : SafeHandleZeroOrMinusOneIsInvalid
{
    /// <summary>Creates and "opens" a simulated native handle.</summary>
    public FileHandleWrapper() : base(ownsHandle: true)
    {
        // Simulate receiving a handle from a native call (e.g., CreateFile)
        nint simulatedHandle = (nint)Random.Shared.Next(0x1000, 0xFFFF);
        SetHandle(simulatedHandle);
        Console.WriteLine($"  [FileHandleWrapper] Acquired handle: 0x{handle:X}");
    }

    /// <summary>
    /// Called by the CLR to release the handle. Must not throw.
    /// In real code this would call CloseHandle(handle) via P/Invoke.
    /// </summary>
    protected override bool ReleaseHandle()
    {
        Console.WriteLine($"  [FileHandleWrapper] Released handle: 0x{handle:X}");
        // return true = release succeeded; false would trigger an escalation policy
        return true;
    }

    /// <summary>Exposes the raw value for demo output.</summary>
    public nint RawValue => handle;
}

using System;

public interface IBytesSource 
{
    public ReadOnlySpan<byte> AsBytesReadOnlySpan { get; }
}

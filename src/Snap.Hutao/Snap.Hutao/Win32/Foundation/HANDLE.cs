// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Win32.Foundation;

[SuppressMessage("", "SA1310")]
internal readonly struct HANDLE
{
    public readonly nint Value;

    public static unsafe implicit operator HANDLE(nint value)
    {
        return *(HANDLE*)&value;
    }

    public static unsafe implicit operator nint(HANDLE handle)
    {
        return *(nint*)&handle;
    }

    public static unsafe implicit operator HANDLE(BOOL value)
    {
        return *(int*)&value;
    }
}
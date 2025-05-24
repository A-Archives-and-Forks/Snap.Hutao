// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Win32.Foundation;

// ReSharper disable InconsistentNaming
internal readonly struct PWSTR
{
#pragma warning disable CS0649
    public readonly unsafe char* Value;
#pragma warning restore CS0649

    public static unsafe implicit operator PWSTR(char* value)
    {
        return *(PWSTR*)&value;
    }

    public static unsafe implicit operator char*(PWSTR value)
    {
        return *(char**)&value;
    }
}
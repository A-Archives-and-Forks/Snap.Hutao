// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Snap.Hutao.Win32.Graphics.Direct3D11;

[SupportedOSPlatform("windows6.1")]
internal static unsafe class ID3D11Resource
{
    internal static ref readonly Guid IID
    {
        get => ref MemoryMarshal.AsRef<Guid>([0xF3, 0x63, 0x8E, 0xDC, 0x2B, 0xD1, 0x52, 0x49, 0xB4, 0x7B, 0x5E, 0x45, 0x02, 0x6A, 0x86, 0x2D]);
    }

    internal readonly struct Vftbl
    {
        internal readonly ID3D11DeviceChild.Vftbl ID3D11DeviceChildVftbl;
        internal new readonly delegate* unmanaged[Stdcall]<nint, D3D11_RESOURCE_DIMENSION*, void> GetType;
        internal readonly delegate* unmanaged[Stdcall]<nint, uint, void> SetEvictionPriority;
        internal readonly delegate* unmanaged[Stdcall]<nint, uint> GetEvictionPriority;
    }
}
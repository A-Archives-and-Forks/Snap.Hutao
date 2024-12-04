// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Snap.Hutao.Win32.Graphics.Direct3D11;

[SupportedOSPlatform("windows6.1")]
internal static unsafe class ID3D11RasterizerState
{
    internal static ref readonly Guid IID
    {
        get => ref MemoryMarshal.AsRef<Guid>([0x81, 0xAB, 0xB4, 0x9B, 0x1A, 0xAB, 0x8F, 0x4D, 0xB5, 0x06, 0xFC, 0x04, 0x20, 0x0B, 0x6E, 0xE7]);
    }

    internal readonly struct Vftbl
    {
        internal readonly ID3D11DeviceChild.Vftbl ID3D11DeviceChildVftbl;
        internal readonly delegate* unmanaged[Stdcall]<nint, D3D11_RASTERIZER_DESC*, void> GetDesc;
    }
}
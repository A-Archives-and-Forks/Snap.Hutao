// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Snap.Hutao.Win32.Graphics.Direct3D11;

[SupportedOSPlatform("windows6.1")]
internal static unsafe class ID3D11Texture1D
{
    internal static ref readonly Guid IID
    {
        get => ref MemoryMarshal.AsRef<Guid>([0x27, 0x5C, 0xFB, 0xF8, 0xB3, 0xC6, 0x75, 0x4F, 0xA4, 0xC8, 0x43, 0x9A, 0xF2, 0xEF, 0x56, 0x4C]);
    }

    internal readonly struct Vftbl
    {
        internal readonly ID3D11Resource.Vftbl ID3D11ResourceVftbl;
        internal readonly delegate* unmanaged[Stdcall]<nint, D3D11_TEXTURE1D_DESC*, void> GetDesc;
    }
}
﻿// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.LifeCycle.InterProcess;

internal enum PipePacketCommand : byte
{
    None = 0,

    RedirectActivation = 10,
}
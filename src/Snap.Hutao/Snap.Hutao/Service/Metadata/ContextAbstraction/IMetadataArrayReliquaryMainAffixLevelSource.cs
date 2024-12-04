// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Metadata.Reliquary;
using System.Collections.Immutable;

namespace Snap.Hutao.Service.Metadata.ContextAbstraction;

internal interface IMetadataArrayReliquaryMainAffixLevelSource
{
    ImmutableArray<ReliquaryMainAffixLevel> ReliquaryMainAffixLevels { get; set; }
}
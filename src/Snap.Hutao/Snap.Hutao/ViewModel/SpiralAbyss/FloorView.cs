// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Metadata.Tower;
using Snap.Hutao.UI.Xaml.Data;
using System.Collections.Immutable;

namespace Snap.Hutao.ViewModel.SpiralAbyss;

internal sealed partial class FloorView : IAdvancedCollectionViewItem
{
    public FloorView(TowerFloor floor, SpiralAbyssMetadataContext context)
    {
        Index = SH.FormatModelBindingHutaoComplexRankFloor(floor.Index);
        IndexValue = floor.Index;
        Disorders = floor.Descriptions;
        UpDisorders = floor.FirstDescriptions;
        DownDisorders = floor.SecondDescriptions;

        Levels = context.IdArrayTowerLevelMap[floor.LevelGroupId].OrderBy(l => l.Index).Select(l => LevelView.From(l, context)).ToList();
    }

    public bool Engaged { get; private set; }

    public string Index { get; }

    public string? SettleTime { get; private set; }

    public int Star { get; private set; }

    public ImmutableArray<string> Disorders { get; }

    public ImmutableArray<string>? UpDisorders { get; }

    public ImmutableArray<string>? DownDisorders { get; }

    public List<LevelView> Levels { get; }

    internal uint IndexValue { get; }

    public static FloorView From(TowerFloor floor, SpiralAbyssMetadataContext context)
    {
        return new(floor, context);
    }

    public void WithSpiralAbyssFloor(Web.Hoyolab.Takumi.GameRecord.SpiralAbyss.SpiralAbyssFloor floor, SpiralAbyssMetadataContext context)
    {
        SettleTime = $"{DateTimeOffset.FromUnixTimeSeconds(floor.SettleTime).ToLocalTime():yyyy.MM.dd HH:mm:ss}";
        Star = floor.Star;
        Engaged = true;

        foreach (LevelView levelView in Levels)
        {
            if (floor.Levels.SingleOrDefault(l => l.Index == levelView.IndexValue) is { } level)
            {
                levelView.WithSpiralAbyssLevel(level, context);
            }
        }
    }
}
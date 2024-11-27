﻿// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Snap.Hutao.Model;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.Abstraction;
using Snap.Hutao.Service.BackgroundImage;
using Snap.Hutao.Service.Game.Package;
using Snap.Hutao.UI.Xaml.Media.Backdrop;
using Snap.Hutao.Web.Bridge;
using Snap.Hutao.Web.Hoyolab;

namespace Snap.Hutao.Service;

[ConstructorGenerated(CallBaseConstructor = true)]
[Injection(InjectAs.Singleton)]
internal sealed partial class AppOptions : DbStoreOptions
{
    private bool? isNotifyIconEnabled;
    private bool? isEmptyHistoryWishVisible;
    private bool? isUnobtainedWishItemVisible;
    private BackdropType? backdropType;
    private ElementTheme? elementTheme;
    private BackgroundImageType? backgroundImageType;
    private Region? region;
    private int? downloadSpeedLimitPerSecondInKiloByte;
    private PackageConverterType? packageConverterType;
    private BridgeShareSaveType? bridgeShareSaveType;

    public bool IsNotifyIconEnabled
    {
        get => GetOption(ref isNotifyIconEnabled, SettingEntry.IsNotifyIconEnabled, true);
        set => SetOption(ref isNotifyIconEnabled, SettingEntry.IsNotifyIconEnabled, value);
    }

    public bool IsEmptyHistoryWishVisible
    {
        get => GetOption(ref isEmptyHistoryWishVisible, SettingEntry.IsEmptyHistoryWishVisible, false);
        set => SetOption(ref isEmptyHistoryWishVisible, SettingEntry.IsEmptyHistoryWishVisible, value);
    }

    public bool IsUnobtainedWishItemVisible
    {
        get => GetOption(ref isUnobtainedWishItemVisible, SettingEntry.IsUnobtainedWishItemVisible, false);
        set => SetOption(ref isUnobtainedWishItemVisible, SettingEntry.IsUnobtainedWishItemVisible, value);
    }

    public List<NameValue<BackdropType>> BackdropTypes { get; } = CollectionsNameValue.FromEnum<BackdropType>(type => type >= 0);

    public BackdropType BackdropType
    {
        get => GetOption(ref backdropType, SettingEntry.SystemBackdropType, Enum.Parse<BackdropType>, BackdropType.Mica);
        set => SetOption(ref backdropType, SettingEntry.SystemBackdropType, value, EnumToStringOrEmpty);
    }

    public Lazy<List<NameValue<ElementTheme>>> LazyElementThemes { get; } = new(() =>
    [
        new(SH.CoreWindowThemeLight, ElementTheme.Light),
        new(SH.CoreWindowThemeDark, ElementTheme.Dark),
        new(SH.CoreWindowThemeSystem, ElementTheme.Default),
    ]);

    public ElementTheme ElementTheme
    {
        get => GetOption(ref elementTheme, SettingEntry.ElementTheme, Enum.Parse<ElementTheme>, ElementTheme.Default);
        set => SetOption(ref elementTheme, SettingEntry.ElementTheme, value, EnumToStringOrEmpty);
    }

    public List<NameValue<BackgroundImageType>> BackgroundImageTypes { get; } = CollectionsNameValue.FromEnum<BackgroundImageType>(type => type.GetLocalizedDescription());

    public BackgroundImageType BackgroundImageType
    {
        get => GetOption(ref backgroundImageType, SettingEntry.BackgroundImageType, Enum.Parse<BackgroundImageType>, BackgroundImageType.None);
        set => SetOption(ref backgroundImageType, SettingEntry.BackgroundImageType, value, EnumToStringOrEmpty);
    }

    public Lazy<List<NameValue<Region>>> LazyRegions { get; } = new(KnownRegions.Get);

    public Region Region
    {
        get => GetOption(ref region, SettingEntry.AnnouncementRegion, v => Region.FromRegionString(v), Region.CNGF01).Value;
        set => SetOption(ref region, SettingEntry.AnnouncementRegion, value, v => v.ToStringOrEmpty());
    }

    [field: AllowNull]
    public string GeetestCustomCompositeUrl
    {
        get => GetOption(ref field, SettingEntry.GeetestCustomCompositeUrl);
        set => SetOption(ref field, SettingEntry.GeetestCustomCompositeUrl, value);
    }

    public int DownloadSpeedLimitPerSecondInKiloByte
    {
        get => GetOption(ref downloadSpeedLimitPerSecondInKiloByte, SettingEntry.DownloadSpeedLimitPerSecondInKiloByte, 0);
        set => SetOption(ref downloadSpeedLimitPerSecondInKiloByte, SettingEntry.DownloadSpeedLimitPerSecondInKiloByte, value);
    }

    public List<NameValue<PackageConverterType>> PackageConverterTypes { get; } = CollectionsNameValue.FromEnum<PackageConverterType>();

    public PackageConverterType PackageConverterType
    {
        get => GetOption(ref packageConverterType, SettingEntry.PackageConverterType, Enum.Parse<PackageConverterType>, PackageConverterType.ScatteredFiles);
        set => SetOption(ref packageConverterType, SettingEntry.PackageConverterType, value, EnumToStringOrEmpty);
    }

    public List<NameValue<BridgeShareSaveType>> BridgeShareSaveTypes { get; } = CollectionsNameValue.FromEnum<BridgeShareSaveType>(type => type.GetLocalizedDescription());

    public BridgeShareSaveType BridgeShareSaveType
    {
        get => GetOption(ref bridgeShareSaveType, SettingEntry.BridgeShareSaveType, Enum.Parse<BridgeShareSaveType>, BridgeShareSaveType.CopyToClipboard);
        set => SetOption(ref bridgeShareSaveType, SettingEntry.BridgeShareSaveType, value, EnumToStringOrEmpty);
    }
}
// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Quartz;
using Snap.Hutao.Model;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.Abstraction;
using Snap.Hutao.Service.Job;
using System.Collections.Immutable;

namespace Snap.Hutao.Service.DailyNote;

[ConstructorGenerated(CallBaseConstructor = true)]
[Injection(InjectAs.Singleton)]
internal sealed partial class DailyNoteOptions : DbStoreOptions
{
    private const int OneMinute = 60;

    private readonly IQuartzService quartzService;

    private bool? isAutoRefreshEnabled;
    private bool? isReminderNotification;
    private bool? isSilentWhenPlayingGame;

    public ImmutableArray<NameValue<int>> RefreshTimes { get; } =
    [
        new(SH.ViewModelDailyNoteRefreshTime4, OneMinute * 4),
        new(SH.ViewModelDailyNoteRefreshTime8, OneMinute * 8),
        new(SH.ViewModelDailyNoteRefreshTime30, OneMinute * 30),
        new(SH.ViewModelDailyNoteRefreshTime40, OneMinute * 40),
        new(SH.ViewModelDailyNoteRefreshTime60, OneMinute * 60),
    ];

    public bool IsAutoRefreshEnabled
    {
        get => GetOption(ref isAutoRefreshEnabled, SettingEntry.DailyNoteIsAutoRefreshEnabled, false);
        set
        {
            if (SetOption(ref isAutoRefreshEnabled, SettingEntry.DailyNoteIsAutoRefreshEnabled, value))
            {
                if (value)
                {
                    if (SelectedRefreshTime is not null)
                    {
                        int refreshTime = SelectedRefreshTime.Value;
                        quartzService.UpdateJobAsync(JobIdentity.DailyNoteGroupName, JobIdentity.DailyNoteRefreshTriggerName, builder =>
                        {
                            return builder.WithSimpleSchedule(sb => sb.WithIntervalInSeconds(refreshTime).RepeatForever());
                        }).GetAwaiter().GetResult();
                    }
                }
                else
                {
                    quartzService.StopJobAsync(JobIdentity.DailyNoteGroupName, JobIdentity.DailyNoteRefreshTriggerName).GetAwaiter().GetResult();
                }
            }
        }
    }

    public NameValue<int>? SelectedRefreshTime
    {
        get => GetOption(ref field, SettingEntry.DailyNoteRefreshSeconds, RefreshTimes, static v => $"{v}", RefreshTimes[1]);
        set
        {
            if (value is not null)
            {
                SetOption(ref field, SettingEntry.DailyNoteRefreshSeconds, value, static v => $"{v.Value}");
                quartzService.UpdateJobAsync(JobIdentity.DailyNoteGroupName, JobIdentity.DailyNoteRefreshTriggerName, builder =>
                {
                    return builder.WithSimpleSchedule(sb => sb.WithIntervalInSeconds(value.Value).RepeatForever());
                }).GetAwaiter().GetResult();
            }
        }
    }

    public bool IsReminderNotification
    {
        get => GetOption(ref isReminderNotification, SettingEntry.DailyNoteReminderNotify);
        set => SetOption(ref isReminderNotification, SettingEntry.DailyNoteReminderNotify, value);
    }

    public bool IsSilentWhenPlayingGame
    {
        get => GetOption(ref isSilentWhenPlayingGame, SettingEntry.DailyNoteSilentWhenPlayingGame);
        set => SetOption(ref isSilentWhenPlayingGame, SettingEntry.DailyNoteSilentWhenPlayingGame, value);
    }

    [SuppressMessage("", "CA1822")]
    public string? WebhookUrl
    {
        get => GetOption(ref field, SettingEntry.DailyNoteWebhookUrl);
        set => SetOption(ref field, SettingEntry.DailyNoteWebhookUrl, value);
    }
}
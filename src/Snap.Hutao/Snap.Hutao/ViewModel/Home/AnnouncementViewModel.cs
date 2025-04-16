// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Setting;
using Snap.Hutao.Service;
using Snap.Hutao.Service.Announcement;
using Snap.Hutao.Service.Hutao;
using Snap.Hutao.UI.Xaml.Control.Card;
using Snap.Hutao.UI.Xaml.View.Card;
using Snap.Hutao.Web.Hoyolab.Hk4e.Common.Announcement;
using System.Collections.ObjectModel;

namespace Snap.Hutao.ViewModel.Home;

[ConstructorGenerated]
[Injection(InjectAs.Scoped)]
internal sealed partial class AnnouncementViewModel : Abstraction.ViewModel
{
    private readonly IAnnouncementService announcementService;
    private readonly IServiceProvider serviceProvider;
    private readonly IHutaoAsAService hutaoAsAService;
    private readonly CultureOptions cultureOptions;
    private readonly ITaskContext taskContext;
    private readonly AppOptions appOptions;

    public AnnouncementWrapper? Announcement { get; set => SetProperty(ref field, value); }

    public ObservableCollection<Web.Hutao.HutaoAsAService.Announcement>? HutaoAnnouncements { get; set => SetProperty(ref field, value); }

    public string GreetingText { get; set => SetProperty(ref field, value); } = SH.ViewPageHomeGreetingTextDefault;

    public List<CardReference>? Cards { get; set => SetProperty(ref field, value); }

    protected override ValueTask<bool> LoadOverrideAsync()
    {
        InitializeDashboard();
        InitializeInGameAnnouncementAsync().SafeForget();
        InitializeHutaoAnnouncementAsync().SafeForget();
        UpdateGreetingText();
        return ValueTask.FromResult(true);
    }

    [SuppressMessage("", "SH003")]
    private async Task InitializeInGameAnnouncementAsync()
    {
        try
        {
            AnnouncementWrapper? announcementWrapper = await announcementService.GetAnnouncementWrapperAsync(cultureOptions.LanguageCode, appOptions.Region, CancellationToken).ConfigureAwait(false);
            await taskContext.SwitchToMainThreadAsync();
            Announcement = announcementWrapper;
            DeferContentLoader?.Load("GameAnnouncementPivot");
        }
        catch (OperationCanceledException)
        {
        }
    }

    [SuppressMessage("", "SH003")]
    private async Task InitializeHutaoAnnouncementAsync()
    {
        try
        {
            ObservableCollection<Web.Hutao.HutaoAsAService.Announcement> hutaoAnnouncements = await hutaoAsAService.GetHutaoAnnouncementCollectionAsync(CancellationToken).ConfigureAwait(false);
            await taskContext.SwitchToMainThreadAsync();
            HutaoAnnouncements = hutaoAnnouncements;
            DeferContentLoader?.Load("HutaoAnnouncementControl");
        }
        catch (OperationCanceledException)
        {
        }
    }

    private void UpdateGreetingText()
    {
        // TODO avatar birthday override.
        int rand = Random.Shared.Next(0, 1000);

        if (rand is >= 0 and < 6)
        {
            GreetingText = SH.ViewPageHomeGreetingTextEasterEgg;
        }
        else if (rand is >= 6 and < 57)
        {
            // TODO: retrieve days
            // GreetingText = string.Format(SH.ViewPageHomeGreetingTextEpic1, 0);
        }
        else if (rand is >= 57 and < 1000)
        {
            rand = Random.Shared.Next(0, 2);
            if (rand is 0)
            {
                // TODO: impl game launch times
                // GreetingText = string.Format(SH.ViewPageHomeGreetingTextCommon1, 0);
            }
            else if (rand is 1)
            {
                GreetingText = SH.FormatViewPageHomeGreetingTextCommon2(LocalSetting.Get(SettingKeys.LaunchTimes, 0));
            }
        }
    }

    private void InitializeDashboard()
    {
        List<CardReference> result = [];

        if (LocalSetting.Get(SettingKeys.IsHomeCardLaunchGamePresented, true))
        {
            result.Add(new() { Card = new LaunchGameCard(serviceProvider) });
        }

        if (LocalSetting.Get(SettingKeys.IsHomeCardGachaStatisticsPresented, true))
        {
            result.Add(new() { Card = new GachaStatisticsCard(serviceProvider) });
        }

        if (LocalSetting.Get(SettingKeys.IsHomeCardAchievementPresented, true))
        {
            result.Add(new() { Card = new AchievementCard(serviceProvider) });
        }

        if (LocalSetting.Get(SettingKeys.IsHomeCardDailyNotePresented, true))
        {
            result.Add(new() { Card = new DailyNoteCard(serviceProvider) });
        }

        if (LocalSetting.Get(SettingKeys.IsHomeCardCalendarPresented, true))
        {
            result.Add(new() { Card = new CalendarCard(serviceProvider) });
        }

        if (LocalSetting.Get(SettingKeys.IsHomeCardSignInPresented, true))
        {
            result.Add(new() { Card = new SignInCard(serviceProvider) });
        }

        Cards = result;
    }
}
// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Database;
using Snap.Hutao.Core.Diagnostics;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.GachaLog.Factory;
using Snap.Hutao.Service.GachaLog.QueryProvider;
using Snap.Hutao.Service.Metadata;
using Snap.Hutao.Service.Metadata.ContextAbstraction;
using Snap.Hutao.ViewModel.GachaLog;
using Snap.Hutao.Web.Hoyolab.Hk4e.Event.GachaInfo;
using Snap.Hutao.Web.Response;
using System.Collections.Immutable;

namespace Snap.Hutao.Service.GachaLog;

[ConstructorGenerated]
[Injection(InjectAs.Scoped, typeof(IGachaLogService))]
internal sealed partial class GachaLogService : IGachaLogService
{
    private readonly IGachaStatisticsSlimFactory gachaStatisticsSlimFactory;
    private readonly IGachaStatisticsFactory gachaStatisticsFactory;
    private readonly IGachaLogRepository gachaLogRepository;
    private readonly IServiceProvider serviceProvider;
    private readonly IMetadataService metadataService;
    private readonly ILogger<GachaLogService> logger;
    private readonly ITaskContext taskContext;

    private GachaLogServiceMetadataContext context;

    public AdvancedDbCollectionView<GachaArchive>? Archives { get; private set; }

    public async ValueTask<bool> InitializeAsync(CancellationToken token = default)
    {
        if (context is { IsInitialized: true })
        {
            return true;
        }

        if (await metadataService.InitializeAsync().ConfigureAwait(false))
        {
            context = await metadataService.GetContextAsync<GachaLogServiceMetadataContext>(token).ConfigureAwait(false);
            Archives = new(gachaLogRepository.GetGachaArchiveCollection(), serviceProvider);
            return true;
        }

        return false;
    }

    public async ValueTask<GachaStatistics> GetStatisticsAsync(GachaArchive archive)
    {
        using (ValueStopwatch.MeasureExecution(logger))
        {
            List<GachaItem> items = gachaLogRepository.GetGachaItemListByArchiveId(archive.InnerId);
            return await gachaStatisticsFactory.CreateAsync(context, items).ConfigureAwait(false);
        }
    }

    public async ValueTask<List<GachaStatisticsSlim>> GetStatisticsSlimListAsync(CancellationToken token = default)
    {
        await InitializeAsync(token).ConfigureAwait(false);
        ArgumentNullException.ThrowIfNull(Archives);

        List<GachaStatisticsSlim> statistics = [];
        foreach (GachaArchive archive in Archives)
        {
            List<GachaItem> items = gachaLogRepository.GetGachaItemListByArchiveId(archive.InnerId);
            GachaStatisticsSlim slim = await gachaStatisticsSlimFactory.CreateAsync(context, items, archive.Uid).ConfigureAwait(false);
            statistics.Add(slim);
        }

        return statistics;
    }

    public async ValueTask<bool> RefreshGachaLogAsync(GachaLogQuery query, RefreshStrategyKind kind, IProgress<GachaLogFetchStatus> progress, CancellationToken token)
    {
        bool isLazy = kind switch
        {
            RefreshStrategyKind.AggressiveMerge => false,
            RefreshStrategyKind.LazyMerge => true,
            _ => throw HutaoException.NotSupported(),
        };

        (bool authkeyValid, GachaArchive? target) = await FetchGachaLogsAsync(query, isLazy, progress, token).ConfigureAwait(false);

        if (target is not null && Archives is not null)
        {
            await taskContext.SwitchToMainThreadAsync();
            Archives.CurrentItem = target;
        }

        return authkeyValid;
    }

    public async ValueTask RemoveArchiveAsync(GachaArchive archive)
    {
        ArgumentNullException.ThrowIfNull(Archives);

        // Sync database
        await taskContext.SwitchToBackgroundAsync();
        gachaLogRepository.RemoveGachaArchiveById(archive.InnerId);

        // Sync cache
        await taskContext.SwitchToMainThreadAsync();
        Archives.Remove(archive);
    }

    public async ValueTask<GachaArchive> EnsureArchiveInCollectionAsync(Guid archiveId, CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(Archives);

        if (Archives.SourceCollection.SingleOrDefault(a => a.InnerId == archiveId) is { } archive)
        {
            return archive;
        }

        GachaArchive? newArchive = gachaLogRepository.GetGachaArchiveById(archiveId);
        ArgumentNullException.ThrowIfNull(newArchive);

        await taskContext.SwitchToMainThreadAsync();
        Archives.Add(newArchive);
        return newArchive;
    }

    private async ValueTask<ValueResult<bool, GachaArchive?>> FetchGachaLogsAsync(GachaLogQuery query, bool isLazy, IProgress<GachaLogFetchStatus> progress, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(Archives);
        GachaLogFetchContext fetchContext = new(gachaLogRepository, context, isLazy);

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            GachaInfoClient gachaInfoClient = scope.ServiceProvider.GetRequiredService<GachaInfoClient>();

            foreach (GachaType configType in GachaLog.QueryTypes)
            {
                fetchContext.ResetType(configType, query);

                do
                {
                    Response<GachaLogPage> response = await gachaInfoClient
                        .GetGachaLogPageAsync(fetchContext.TypedQueryOptions, token)
                        .ConfigureAwait(false);

                    // Fast break fetching if authkey timeout
                    if (!ResponseValidator.TryValidateWithoutUINotification(response, out GachaLogPage? page))
                    {
                        fetchContext.Report(progress, isAuthKeyTimeout: true);
                        break;
                    }

                    fetchContext.ResetCurrentPage();
                    ImmutableArray<GachaLogItem> items = page.List;

                    foreach (GachaLogItem item in items)
                    {
                        fetchContext.EnsureArchiveAndEndId(item, Archives, gachaLogRepository);

                        if (fetchContext.ShouldAddItem(item))
                        {
                            fetchContext.AddItem(item);
                        }
                        else
                        {
                            fetchContext.CompleteCurrentTypeAdding();
                            break;
                        }
                    }

                    fetchContext.Report(progress);
                    await Task.Delay(Random.Shared.Next(1000, 2000), token).ConfigureAwait(false);

                    if (fetchContext.HasReachCurrentTypeEnd(items))
                    {
                        // Exit current type fetch loop
                        break;
                    }
                }
                while (true);

                // Fast break query type loop if authkey timeout, skip saving items
                if (fetchContext.FetchStatus.AuthKeyTimeout)
                {
                    break;
                }

                // Save items for each queryType
                token.ThrowIfCancellationRequested();
                fetchContext.SaveItems();

                // Delay between query types
                await Task.Delay(Random.Shared.Next(1000, 2000), token).ConfigureAwait(false);
            }
        }

        return new(!fetchContext.FetchStatus.AuthKeyTimeout, fetchContext.TargetArchive);
    }
}
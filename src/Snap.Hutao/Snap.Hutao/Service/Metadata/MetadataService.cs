﻿// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Extensions.Caching.Memory;
using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Core.Diagnostics;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Core.IO;
using Snap.Hutao.Core.IO.Hashing;
using Snap.Hutao.Core.Setting;
using Snap.Hutao.Service.Notification;
using System.Collections.Frozen;
using System.Collections.Immutable;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;

namespace Snap.Hutao.Service.Metadata;

[ConstructorGenerated]
[Injection(InjectAs.Singleton, typeof(IMetadataService))]
[HttpClient(HttpClientConfiguration.Default)]
internal sealed partial class MetadataService : IMetadataService, IMetadataServiceInitialization
{
    private const string MetaFileName = "Meta.json";

    private readonly TaskCompletionSource initializeCompletionSource = new();

    private readonly IServiceScopeFactory serviceScopeFactory;
    private readonly ILogger<MetadataService> logger;
    private readonly MetadataOptions metadataOptions;
    private readonly IInfoBarService infoBarService;
    private readonly JsonSerializerOptions options;

    private FrozenSet<string>? fileNames;
    private bool isInitialized;

    public partial IMemoryCache MemoryCache { get; }

    public async ValueTask<bool> InitializeAsync()
    {
        await initializeCompletionSource.Task.ConfigureAwait(false);
        return isInitialized;
    }

    public async ValueTask InitializeInternalAsync(CancellationToken token = default)
    {
        if (isInitialized)
        {
            return;
        }

        using (ValueStopwatch.MeasureExecution(logger))
        {
            isInitialized = await DownloadMetadataDescriptionFileAndCheckAsync(token).ConfigureAwait(false);
            initializeCompletionSource.TrySetResult();
        }
    }

    public async ValueTask<ImmutableArray<T>> FromCacheOrFileAsync<T>(MetadataFileStrategy strategy, CancellationToken token)
        where T : class
    {
        Verify.Operation(isInitialized, SH.ServiceMetadataNotInitialized);
        string cacheKey = $"{nameof(MetadataService)}.Cache.{strategy.Name}";

        if (MemoryCache.TryGetValue(cacheKey, out object? value))
        {
            ArgumentNullException.ThrowIfNull(value);
            return (ImmutableArray<T>)value;
        }

        return strategy.IsScattered
            ? await FromCacheOrScatteredFile<T>(strategy, cacheKey, token).ConfigureAwait(false)
            : await FromCacheOrSingleFile<T>(strategy, cacheKey, token).ConfigureAwait(false);
    }

    private async ValueTask<ImmutableArray<T>> FromCacheOrSingleFile<T>(MetadataFileStrategy strategy, string cacheKey, CancellationToken token)
        where T : class
    {
        string path = metadataOptions.GetLocalizedLocalPath($"{strategy.Name}.json");
        if (!File.Exists(path))
        {
            FileNotFoundException exception = new(SH.ServiceMetadataFileNotFound, strategy.Name);
            throw HutaoException.Throw(SH.ServiceMetadataFileNotFound, exception);
        }

        using (Stream fileStream = File.OpenRead(path))
        {
            try
            {
                ImmutableArray<T> result = await JsonSerializer.DeserializeAsync<ImmutableArray<T>>(fileStream, options, token).ConfigureAwait(false);
                return MemoryCache.Set(cacheKey, result);
            }
            catch (Exception ex)
            {
                ex.Data.Add("FileName", strategy.Name);
                throw;
            }
        }
    }

    private async ValueTask<ImmutableArray<T>> FromCacheOrScatteredFile<T>(MetadataFileStrategy strategy, string cacheKey, CancellationToken token)
        where T : class
    {
        string path = metadataOptions.GetLocalizedLocalPath(strategy.Name);
        if (!Directory.Exists(path))
        {
            DirectoryNotFoundException exception = new(SH.ServiceMetadataFileNotFound);
            throw HutaoException.Throw(SH.ServiceMetadataFileNotFound, exception);
        }

        ImmutableArray<T>.Builder results = ImmutableArray.CreateBuilder<T>();
        foreach (string file in Directory.GetFiles(path, "*.json"))
        {
            string fileName = $"{strategy.Name}/{Path.GetFileNameWithoutExtension(file)}";
            if (fileNames is not null && !fileNames.Contains(fileName))
            {
                continue;
            }

            using (Stream fileStream = File.OpenRead(file))
            {
                try
                {
                    T? result = await JsonSerializer.DeserializeAsync<T>(fileStream, options, token).ConfigureAwait(false);
                    ArgumentNullException.ThrowIfNull(result);
                    results.Add(result);
                }
                catch (Exception ex)
                {
                    ex.Data.Add("FileName", fileName);
                    throw;
                }
            }
        }

        return MemoryCache.Set(cacheKey, results.ToImmutable());
    }

    private async ValueTask<bool> DownloadMetadataDescriptionFileAndCheckAsync(CancellationToken token)
    {
        if (LocalSetting.Get(SettingKeys.SuppressMetadataInitialization, false))
        {
            return true;
        }

        if (await DownloadMetadataDescriptionFileAsync(token).ConfigureAwait(false) is not { } metadataFileHashes)
        {
            return false;
        }

        await CheckMetadataSourceFilesAsync(metadataFileHashes, token).ConfigureAwait(false);

        // Save metadataFile
        using (FileStream metaFileStream = File.Create(metadataOptions.GetLocalizedLocalPath(MetaFileName)))
        {
            await JsonSerializer
                .SerializeAsync(metaFileStream, metadataFileHashes, options, token)
                .ConfigureAwait(false);
        }

        fileNames = [.. metadataFileHashes.Select(entry => entry.Key)];
        return true;
    }

    private async ValueTask<ImmutableDictionary<string, string>?> DownloadMetadataDescriptionFileAsync(CancellationToken token)
    {
        try
        {
            ImmutableDictionary<string, string>? metadataFileHashes;
            using (IServiceScope scope = serviceScopeFactory.CreateScope())
            {
                IHttpClientFactory httpClientFactory = scope.ServiceProvider.GetRequiredService<IHttpClientFactory>();
                using (HttpClient httpClient = httpClientFactory.CreateClient(nameof(MetadataService)))
                {
                    // Download meta check file
                    metadataFileHashes = await httpClient
                        .GetFromJsonAsync<ImmutableDictionary<string, string>>(metadataOptions.GetLocalizedRemoteFile(MetaFileName), options, token)
                        .ConfigureAwait(false);
                }
            }

            if (metadataFileHashes is null)
            {
                infoBarService.Error(SH.ServiceMetadataParseFailed);
                return default;
            }

            return metadataFileHashes;
        }
        catch (JsonException ex)
        {
            infoBarService.Error(ex, SH.ServiceMetadataRequestFailed);
            return default;
        }
        catch (HttpRequestException ex)
        {
            if (ex.StatusCode is (HttpStatusCode)418)
            {
                infoBarService.Error(SH.ServiceMetadataVersionNotSupported);
            }
            else
            {
                infoBarService.Error(ex, SH.FormatServiceMetadataHttpRequestFailed(ex.StatusCode, ex.HttpRequestError));
            }

            return default;
        }
    }

    [SuppressMessage("", "SH003")]
    private Task CheckMetadataSourceFilesAsync(ImmutableDictionary<string, string> metaHashMap, CancellationToken token)
    {
        return Parallel.ForEachAsync(metaHashMap, token, async (pair, token) =>
        {
            (string fileName, string hash) = pair;
            string fileFullName = $"{fileName}.json";
            string fileFullPath = metadataOptions.GetLocalizedLocalPath(fileFullName);
            if (Path.GetDirectoryName(fileFullPath) is { } directory && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            bool skip = false;
            if (File.Exists(fileFullPath))
            {
                skip = hash == await XXH64.HashFileAsync(fileFullPath, token).ConfigureAwait(true);
            }

            if (!skip)
            {
                logger.LogInformation("{Hash} of {File} not matched, begin downloading", nameof(XXH64), fileFullName);
                await DownloadMetadataSourceFilesAsync(fileFullName, token).ConfigureAwait(true);
            }
        });
    }

    private async ValueTask DownloadMetadataSourceFilesAsync(string fileFullName, CancellationToken token)
    {
        using (IServiceScope scope = serviceScopeFactory.CreateScope())
        {
            IHttpClientFactory httpClientFactory = scope.ServiceProvider.GetRequiredService<IHttpClientFactory>();
            using (HttpClient httpClient = httpClientFactory.CreateClient(nameof(MetadataService)))
            {
                using (HttpRequestMessage message = new(HttpMethod.Get, metadataOptions.GetLocalizedRemoteFile(fileFullName)))
                {
                    // We have too much line endings now, should cache the response.
                    using (HttpResponseMessage responseMessage = await httpClient.SendAsync(message, HttpCompletionOption.ResponseContentRead, token).ConfigureAwait(false))
                    {
                        Stream sourceStream = await responseMessage.Content.ReadAsStreamAsync(token).ConfigureAwait(false);

                        // Write stream while convert LF to CRLF
                        using (StreamReaderWriter readerWriter = new(new(sourceStream), File.CreateText(metadataOptions.GetLocalizedLocalPath(fileFullName))))
                        {
                            while (await readerWriter.ReadLineAsync(token).ConfigureAwait(false) is { } line)
                            {
                                await readerWriter.WriteAsync(line).ConfigureAwait(false);

                                if (!readerWriter.Reader.EndOfStream)
                                {
                                    await readerWriter.WriteAsync("\r\n").ConfigureAwait(false);
                                }
                            }
                        }
                    }
                }
            }
        }

        logger.LogInformation("Download {file} completed", fileFullName);
    }
}
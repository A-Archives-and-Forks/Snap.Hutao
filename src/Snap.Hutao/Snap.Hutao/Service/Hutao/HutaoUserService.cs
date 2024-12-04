// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Setting;
using Snap.Hutao.Web.Hutao;
using Snap.Hutao.Web.Response;

namespace Snap.Hutao.Service.Hutao;

/// <summary>
/// 胡桃用户服务
/// </summary>
[ConstructorGenerated]
[Injection(InjectAs.Singleton, typeof(IHutaoUserService))]
internal sealed partial class HutaoUserService : IHutaoUserService, IHutaoUserServiceInitialization
{
    private readonly TaskCompletionSource initializeCompletionSource = new();

    private readonly IServiceScopeFactory serviceScopeFactory;
    private readonly ITaskContext taskContext;
    private readonly HutaoUserOptions options;

    private bool isInitialized;

    /// <inheritdoc/>
    public async ValueTask<bool> InitializeAsync()
    {
        await initializeCompletionSource.Task.ConfigureAwait(false);
        return isInitialized;
    }

    /// <inheritdoc/>
    public async ValueTask InitializeInternalAsync(CancellationToken token = default)
    {
        string userName = LocalSetting.Get(SettingKeys.PassportUserName, string.Empty);
        string password = LocalSetting.Get(SettingKeys.PassportPassword, string.Empty);

        if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
        {
            options.PostLoginSkipped();
        }
        else
        {
            using (IServiceScope scope = serviceScopeFactory.CreateScope())
            {
                HutaoPassportClient hutaoPassportClient = scope.ServiceProvider.GetRequiredService<HutaoPassportClient>();
                Response<string> response = await hutaoPassportClient.LoginAsync(userName, password, token).ConfigureAwait(false);

                if (ResponseValidator.TryValidate(response, scope.ServiceProvider, out string? authToken))
                {
                    if (await options.PostLoginSucceedAsync(scope.ServiceProvider, userName, password, authToken).ConfigureAwait(false))
                    {
                        isInitialized = true;
                    }
                }
                else
                {
                    await taskContext.SwitchToMainThreadAsync();
                    options.PostLoginFailed();
                }
            }
        }

        initializeCompletionSource.TrySetResult();
    }
}
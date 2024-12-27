// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Hutao;
using Snap.Hutao.Web.Request.Builder;

namespace Snap.Hutao.Web.Hutao;

internal static class HutaoPassportHttpRequestMessageBuilderExtension
{
    public static async ValueTask TrySetTokenAsync(this HttpRequestMessageBuilder builder, HutaoUserOptions hutaoUserOptions)
    {
        string? token = await hutaoUserOptions.GetAuthTokenAsync().ConfigureAwait(false);
        builder.Headers.Authorization = string.IsNullOrEmpty(token) ? default : new("Bearer", token);
    }
}
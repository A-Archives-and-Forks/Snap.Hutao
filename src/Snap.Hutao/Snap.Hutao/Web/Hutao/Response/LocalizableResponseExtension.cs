// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Globalization;

namespace Snap.Hutao.Web.Hutao.Response;

internal static class LocalizableResponseExtension
{
    public static string? GetLocalizationMessageOrDefault(this ILocalizableResponse localizableResponse)
    {
        string? key = localizableResponse.LocalizationKey;
        return string.IsNullOrEmpty(key) ? default : SH.ResourceManager.GetString(key, CultureInfo.CurrentCulture);
    }

    public static string GetLocalizationMessageOrMessage<TResponse>(this TResponse localizableResponse)
        where TResponse : Web.Response.Response, ILocalizableResponse
    {
        return localizableResponse.GetLocalizationMessageOrDefault() ?? localizableResponse.Message;
    }

    public static string GetLocalizationMessage(this ILocalizableResponse localizableResponse)
    {
        string? key = localizableResponse.LocalizationKey;
        return string.IsNullOrEmpty(key)
            ? string.Empty
            : SH.ResourceManager.GetString(key, CultureInfo.CurrentCulture) ?? string.Empty;
    }
}
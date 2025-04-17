// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Notification;

namespace Snap.Hutao.Web.Response;

internal static class ResponseValidator
{
    public static bool TryValidate(Response response, IInfoBarService infoBarService)
    {
        return new DefaultResponseValidator(infoBarService).TryValidate(response);
    }

    public static bool TryValidate(Response response, IServiceProvider serviceProvider, IIsDisposed isDisposed)
    {
        isDisposed.TryThrow();
        return serviceProvider.GetRequiredService<ICommonResponseValidator<Response>>().TryValidate(response);
    }

    public static bool TryValidate<TData>(Response<TData> response, IInfoBarService infoBarService, [NotNullWhen(true)] out TData? data)
    {
        return new TypedResponseValidator<TData>(infoBarService).TryValidate(response, out data);
    }

    [Obsolete]
    public static bool TryValidate<TData>(Response<TData> response, IServiceProvider serviceProvider, [NotNullWhen(true)] out TData? data)
    {
        return serviceProvider.GetRequiredService<ITypedResponseValidator<TData>>().TryValidate(response, out data);
    }

    public static bool TryValidate<TData>(Response<TData> response, IServiceProvider serviceProvider, IIsDisposed isDisposed, [NotNullWhen(true)] out TData? data)
    {
        isDisposed.TryThrow();
        return serviceProvider.GetRequiredService<ITypedResponseValidator<TData>>().TryValidate(response, out data);
    }

    public static bool TryValidateWithoutUINotification(Response response)
    {
        return new DefaultResponseValidator(default!).TryValidateWithoutUINotification(response);
    }

    public static bool TryValidateWithoutUINotification(Response response, IServiceProvider serviceProvider, IIsDisposed isDisposed)
    {
        isDisposed.TryThrow();
        return serviceProvider.GetRequiredService<ICommonResponseValidator<Response>>().TryValidateWithoutUINotification(response);
    }

    public static bool TryValidateWithoutUINotification<TData>(Response<TData> response, [NotNullWhen(true)] out TData? data)
    {
        return new TypedResponseValidator<TData>(default!).TryValidateWithoutUINotification(response, out data);
    }

    public static bool TryValidateWithoutUINotification<TData>(Response<TData> response, IServiceProvider serviceProvider, IIsDisposed isDisposed, [NotNullWhen(true)] out TData? data)
    {
        isDisposed.TryThrow();
        return serviceProvider.GetRequiredService<ITypedResponseValidator<TData>>().TryValidateWithoutUINotification(response, out data);
    }
}
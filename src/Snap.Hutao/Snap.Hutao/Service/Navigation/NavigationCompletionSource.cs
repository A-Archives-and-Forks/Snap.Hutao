// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Navigation;

internal class NavigationCompletionSource : INavigationExtraData, INavigationCompletionSource
{
    private readonly TaskCompletionSource navigationCompletedTcs = new();

    public NavigationCompletionSource(object? data = null)
    {
        Data = data;
    }

    public object? Data { get; set; }

    [SuppressMessage("", "SH003")]
    public Task WaitForCompletionAsync()
    {
        return navigationCompletedTcs.Task;
    }

    public void NotifyNavigationCompleted()
    {
        navigationCompletedTcs.TrySetResult();
    }

    public void NotifyNavigationException(Exception exception)
    {
        navigationCompletedTcs.TrySetException(exception);
    }
}

[SuppressMessage("", "SA1402")]
internal class NavigationCompletionSource<T> : NavigationCompletionSource
    where T : class
{
    public NavigationCompletionSource(T data)
        : base(data)
    {
    }

    public T? TypedData { get => Data as T; set => Data = value; }
}
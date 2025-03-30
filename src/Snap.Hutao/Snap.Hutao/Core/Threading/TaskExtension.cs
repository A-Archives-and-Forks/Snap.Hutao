// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.Threading;

internal static class TaskExtension
{
    public static async void SafeForget(this Task task)
    {
        try
        {
            await task.ConfigureAwait(true);
        }
        catch (OperationCanceledException)
        {
            // Do nothing
        }
        catch (Exception ex)
        {
            ex.SetSentryMechanism("TaskExtension.SafeForget", handled: true);
            SentrySdk.CaptureException(ex);
        }
    }

    public static async void SafeForget(this ValueTask task)
    {
        try
        {
            await task.ConfigureAwait(true);
        }
        catch (OperationCanceledException)
        {
            // Do nothing
        }
        catch (Exception ex)
        {
            ex.SetSentryMechanism("TaskExtension.SafeForget", handled: true);
            SentrySdk.CaptureException(ex);
        }
    }

    [SuppressMessage("", "SH003")]
    public static async Task WhenAllOrAnyException(params Task[] tasks)
    {
        using (CancellationTokenSource taskFaultedCts = new())
        {
            List<Task> taskList = [.. tasks.Select(task => WrapTask(task, taskFaultedCts.Token))];

            Task firstCompletedTask = await Task.WhenAny(taskList).ConfigureAwait(true);

            if (firstCompletedTask.IsFaulted)
            {
#pragma warning disable CA1849
                // ReSharper disable once MethodHasAsyncOverload
                taskFaultedCts.Cancel();
#pragma warning restore CA1849
                await firstCompletedTask.ConfigureAwait(true);
            }

            await Task.WhenAll(taskList).ConfigureAwait(true);
        }

        static async Task WrapTask(Task task, CancellationToken token)
        {
            try
            {
                await task.ConfigureAwait(true);
            }
            catch (Exception) when (!token.IsCancellationRequested)
            {
                throw;
            }
        }
    }

    /// <summary>
    /// Immediately stop waiting the <paramref name="task"/> when the <paramref name="token"/> is triggered.
    /// </summary>
    /// <param name="task">The task to cancel waiting with</param>
    /// <param name="token">The cancellation token to trigger</param>
    /// <returns>A new task that will complete when <paramref name="task"/> is completed or <paramref name="token"/> is triggered</returns>
    /// <exception cref="OperationCanceledException">The <paramref name="token"/> is triggered</exception>
    [SuppressMessage("", "SH003")]
    [SuppressMessage("", "SH007")]
    public static async Task WithCancellation(this Task task, CancellationToken token)
    {
        TaskCompletionSource tcs = new();
        using (token.UnsafeRegister(s => ((TaskCompletionSource)s!).TrySetResult(), tcs))
        {
            if (task != await Task.WhenAny(task, tcs.Task).ConfigureAwait(true))
            {
                throw new OperationCanceledException(token);
            }
        }
    }

    /// <summary>
    /// Immediately stop waiting the <paramref name="task"/> when the <paramref name="token"/> is triggered.
    /// </summary>
    /// <typeparam name="T">Task return value's type</typeparam>
    /// <param name="task">The task to cancel waiting with</param>
    /// <param name="token">The cancellation token to trigger</param>
    /// <returns>A new task that will complete when <paramref name="task"/> is completed or <paramref name="token"/> is triggered</returns>
    /// <exception cref="OperationCanceledException">The <paramref name="token"/> is triggered</exception>
    [SuppressMessage("", "SH003")]
    [SuppressMessage("", "SH007")]
    public static async Task<T> WithCancellation<T>(this Task<T> task, CancellationToken token)
    {
        TaskCompletionSource tcs = new();
        using (token.UnsafeRegister(s => ((TaskCompletionSource)s!).TrySetResult(), tcs))
        {
            if (task != await Task.WhenAny(task, tcs.Task).ConfigureAwait(true))
            {
                throw new OperationCanceledException(token);
            }
        }

        return await task.ConfigureAwait(true);
    }
}
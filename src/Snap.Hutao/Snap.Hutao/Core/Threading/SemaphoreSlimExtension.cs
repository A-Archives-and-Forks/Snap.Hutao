// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.ExceptionService;

namespace Snap.Hutao.Core.Threading;

internal static class SemaphoreSlimExtension
{
    public static async ValueTask<SemaphoreSlimToken> EnterAsync(this SemaphoreSlim semaphoreSlim, CancellationToken token = default)
    {
        try
        {
            await semaphoreSlim.WaitAsync(token).ConfigureAwait(false);
        }
        catch (ObjectDisposedException ex)
        {
            HutaoException.OperationCanceled(SH.CoreThreadingSemaphoreSlimDisposed, ex);
        }

        return new(semaphoreSlim);
    }

    public static SemaphoreSlimToken Enter(this SemaphoreSlim semaphoreSlim)
    {
        try
        {
            semaphoreSlim.Wait();
        }
        catch (ObjectDisposedException ex)
        {
            HutaoException.OperationCanceled(SH.CoreThreadingSemaphoreSlimDisposed, ex);
        }

        return new(semaphoreSlim);
    }
}
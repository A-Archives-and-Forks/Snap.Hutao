// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.LifeCycle.InterProcess;

[Injection(InjectAs.Singleton)]
[ConstructorGenerated]
internal sealed partial class PrivateNamedPipeMessageDispatcher
{
    private readonly IServiceProvider serviceProvider;

    public void RedirectedActivation(HutaoActivationArguments? args)
    {
        if (args is null)
        {
            return;
        }

        serviceProvider.GetRequiredService<IAppActivation>().RedirectedActivate(args);
    }

    public void ExitApplication()
    {
        ITaskContext taskContext = serviceProvider.GetRequiredService<ITaskContext>();
        App app = serviceProvider.GetRequiredService<App>();
        taskContext.InvokeOnMainThread(app.Exit);
    }
}
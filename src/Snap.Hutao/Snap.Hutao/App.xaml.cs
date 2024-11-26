﻿// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.Windows.AppLifecycle;
using Microsoft.Windows.AppNotifications;
using Snap.Hutao.Core;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Core.LifeCycle;
using Snap.Hutao.Core.LifeCycle.InterProcess;
using Snap.Hutao.Core.Logging;
using Snap.Hutao.Win32.Foundation;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using WinRT;
using WinRT.Interop;

namespace Snap.Hutao;

[Injection(InjectAs.Singleton)]
[SuppressMessage("", "SH001", Justification = "The App must be public")]
public sealed partial class App : Application
{
    private const string ConsoleBanner = $"""
        ----------------------------------------------------------------
          _____                         _    _         _ 
         / ____|                       | |  | |       | |
        | (___   _ __    __ _  _ __    | |__| | _   _ | |_  __ _   ___
         \___ \ | '_ \  / _` || '_ \   |  __  || | | || __|/ _` | / _ \
         ____) || | | || (_| || |_) |_ | |  | || |_| || |_| (_| || (_) |
        |_____/ |_| |_| \__,_|| .__/(_)|_|  |_| \__,_| \__|\__,_| \___/
                              | |
                              |_|
        
        Snap.Hutao is a open source software developed by DGP Studio.
        Copyright (C) 2022 - 2024 DGP Studio, All Rights Reserved.
        ----------------------------------------------------------------
        """;

    private readonly IServiceProvider serviceProvider;
    private readonly IAppActivation activation;
    private readonly ILogger<App> logger;

    public App(IServiceProvider serviceProvider)
    {
        // Load app resource
        InitializeComponent();

        ExceptionHandlingSupport.Initialize(serviceProvider, this);

        activation = serviceProvider.GetRequiredService<IAppActivation>();
        logger = serviceProvider.GetRequiredService<ILogger<App>>();
        this.serviceProvider = serviceProvider;
    }

    private static ref readonly Guid IApplicationIID
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref MemoryMarshal.AsRef<Guid>([231, 244, 168, 6, 70, 17, 175, 85, 130, 13, 235, 213, 86, 67, 176, 33]);
    }

    public new void Exit()
    {
        XamlApplicationLifetime.Exiting = true;
        base.Exit();
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        try
        {
            // Important: You must call AppNotificationManager::Default().Register
            // before calling AppInstance.GetCurrent.GetActivatedEventArgs.
            AppNotificationManager.Default.NotificationInvoked += activation.NotificationInvoked;
            AppNotificationManager.Default.Register();
            AppActivationArguments activatedEventArgs = AppInstance.GetCurrent().GetActivatedEventArgs();

            if (serviceProvider.GetRequiredService<PrivateNamedPipeClient>().TryRedirectActivationTo(activatedEventArgs))
            {
                logger.LogDebug("Application exiting on RedirectActivationTo");
                Exit();
                return;
            }

            logger.LogColorizedInformation((ConsoleBanner, ConsoleColor.DarkYellow));
            LogDiagnosticInformation();

            // Manually invoke
            UnsafeAdjustRequestedThemeSettable();
            activation.ActivateAndInitialize(HutaoActivationArguments.FromAppActivationArguments(activatedEventArgs));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Application failed in App.OnLaunched");
            Process.GetCurrentProcess().Kill();
        }
    }

    private unsafe void UnsafeAdjustRequestedThemeSettable()
    {
        *(BOOLEAN*)(((IWinRTObject)this).NativeObject.As<IUnknownVftbl>(IApplicationIID).ThisPtr + 0x118U) = true;
    }

    private void LogDiagnosticInformation()
    {
        logger.LogColorizedInformation(("FamilyName: {Name}", ConsoleColor.Blue), (HutaoRuntime.FamilyName, ConsoleColor.Cyan));
        logger.LogColorizedInformation(("Version: {Version}", ConsoleColor.Blue), (HutaoRuntime.Version, ConsoleColor.Cyan));
        logger.LogColorizedInformation(("LocalCache: {Path}", ConsoleColor.Blue), (HutaoRuntime.LocalCache, ConsoleColor.Cyan));
    }
}
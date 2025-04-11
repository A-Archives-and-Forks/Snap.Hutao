// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Snap.Hutao.Core.Graphics;
using Snap.Hutao.UI.Xaml;
using Snap.Hutao.UI.Xaml.Media.Backdrop;
using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.UI.WindowsAndMessaging;
using Windows.Foundation;
using WinRT.Interop;
using static Snap.Hutao.Win32.Macros;
using static Snap.Hutao.Win32.User32;

namespace Snap.Hutao.UI.Shell;

internal sealed class NotifyIconXamlHostWindow : Window, IWindowNeedEraseBackground
{
    public NotifyIconXamlHostWindow(IServiceProvider serviceProvider)
    {
        Content = new Border();

        this.AddExStyleLayered();
        SetLayeredWindowAttributes(this.GetWindowHandle(), RGB(0, 0, 0), 0, LAYERED_WINDOW_ATTRIBUTES_FLAGS.LWA_COLORKEY | LAYERED_WINDOW_ATTRIBUTES_FLAGS.LWA_ALPHA);
        this.AddExStyleToolWindow();

        AppWindow.Title = "SnapHutaoNotifyIconXamlHost";
        AppWindow.IsShownInSwitchers = false;
        if (AppWindow.Presenter is OverlappedPresenter presenter)
        {
            presenter.IsMaximizable = false;
            presenter.IsMinimizable = false;
            presenter.IsResizable = false;
            presenter.IsAlwaysOnTop = true;
            presenter.SetBorderAndTitleBar(false, false);
        }

        Closed += OnWindowClosed;

        this.InitializeController(serviceProvider);
    }

    public void ShowFlyoutAt(FlyoutBase flyout, Point point, RECT icon)
    {
        icon.left -= 8;
        icon.top -= 8;
        icon.right += 8;
        icon.bottom += 8;

        HWND hwnd = WindowNative.GetWindowHandle(this);
        ShowWindow(hwnd, SHOW_WINDOW_CMD.SW_NORMAL);
        SetForegroundWindow(hwnd);

        if (AppWindow is null || Content?.XamlRoot is null /*ERROR_XAMLROOT_REQUIRED*/)
        {
            return;
        }

        MoveAndResize(icon);

        flyout.ShowAt(Content, new()
        {
            Placement = FlyoutPlacementMode.Auto,
            ShowMode = FlyoutShowMode.Standard,
        });
    }

    public void MoveAndResize(RECT icon)
    {
        AppWindow.MoveAndResize(RectInt32Convert.RectInt32(icon));
    }

    private static void OnWindowClosed(object sender, WindowEventArgs args)
    {
        // https://github.com/DGP-Studio/Snap.Hutao/issues/2532
        // Prevent the window closing when the application is not exiting.
        if (!XamlApplicationLifetime.Exiting)
        {
            args.Handled = true;
        }
    }
}
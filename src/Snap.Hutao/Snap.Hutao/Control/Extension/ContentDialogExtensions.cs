﻿// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Snap.Hutao.Control.Extension;

/// <summary>
/// 对话框扩展
/// </summary>
internal static class ContentDialogExtensions
{
    /// <summary>
    /// 针对窗口进行初始化
    /// </summary>
    /// <param name="contentDialog">对话框</param>
    /// <param name="window">窗口</param>
    /// <returns>初始化完成的对话框</returns>
    public static ContentDialog InitializeWithWindow(this ContentDialog contentDialog, Window window)
    {
        contentDialog.XamlRoot = window.Content.XamlRoot;

        return contentDialog;
    }

    /// <summary>
    /// 阻止用户交互
    /// </summary>
    /// <param name="contentDialog">对话框</param>
    /// <returns>用于恢复用户交互</returns>
    public static async ValueTask<IAsyncDisposable> BlockAsync(this ContentDialog contentDialog)
    {
        await ThreadHelper.SwitchToMainThreadAsync();
        contentDialog.ShowAsync().AsTask().SafeForget();

        // E_ASYNC_OPERATION_NOT_STARTED 0x80000019
        return new ContentDialogHider(contentDialog);
    }

    private readonly struct ContentDialogHider : IAsyncDisposable
    {
        private readonly ContentDialog contentDialog;

        public ContentDialogHider(ContentDialog contentDialog)
        {
            this.contentDialog = contentDialog;
        }

        public async ValueTask DisposeAsync()
        {
            await ThreadHelper.SwitchToMainThreadAsync();

            // Hide() must be called on main thread.
            contentDialog.Hide();
        }
    }
}
// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Model.Entity;

namespace Snap.Hutao.View.Dialog;

/// <summary>
/// ʵʱ���֪ͨ���öԻ���
/// </summary>
public sealed partial class DailyNoteNotificationDialog : ContentDialog
{
    /// <summary>
    /// ����һ���µ�ʵʱ���֪ͨ���öԻ���
    /// </summary>
    /// <param name="window">����</param>
    /// <param name="entry">ʵʱ���</param>
    public DailyNoteNotificationDialog(Window window, DailyNoteEntry entry)
    {
        InitializeComponent();
        XamlRoot = window.Content.XamlRoot;
        DataContext = entry;
    }
}

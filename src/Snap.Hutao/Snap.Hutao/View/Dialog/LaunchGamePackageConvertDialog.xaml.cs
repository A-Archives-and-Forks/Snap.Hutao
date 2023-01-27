// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Control;

namespace Snap.Hutao.View.Dialog;

/// <summary>
/// ������Ϸ�ͻ���ת���Ի���
/// </summary>
public sealed partial class LaunchGamePackageConvertDialog : ContentDialog
{
    private static readonly DependencyProperty DescriptionProperty = Property<LaunchGamePackageConvertDialog>.Depend(nameof(Description), "���Ժ�");

    /// <summary>
    /// ����һ���µ�������Ϸ�ͻ���ת���Ի���
    /// </summary>
    public LaunchGamePackageConvertDialog()
    {
        InitializeComponent();
        XamlRoot = Ioc.Default.GetRequiredService<MainWindow>().Content.XamlRoot;
        DataContext = this;
    }

    /// <summary>
    /// ����
    /// </summary>
    public string Description
    {
        get { return (string)GetValue(DescriptionProperty); }
        set { SetValue(DescriptionProperty, value); }
    }
}

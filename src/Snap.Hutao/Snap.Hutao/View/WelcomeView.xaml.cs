// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.ViewModel;

namespace Snap.Hutao.View;

/// <summary>
/// ��ӭ��ͼ
/// </summary>
public sealed partial class WelcomeView : UserControl
{
    /// <summary>
    /// ����һ���µĻ�ӭ��ͼ
    /// </summary>
    public WelcomeView()
    {
        InitializeComponent();
        DataContext = Ioc.Default.GetRequiredService<WelcomeViewModel>();
    }
}

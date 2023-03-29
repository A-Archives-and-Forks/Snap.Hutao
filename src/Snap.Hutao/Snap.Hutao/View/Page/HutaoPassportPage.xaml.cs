// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Control;
using Snap.Hutao.ViewModel;

namespace Snap.Hutao.View.Page;

/// <summary>
/// ����ͨ��֤ҳ��
/// </summary>
internal sealed partial class HutaoPassportPage : ScopedPage
{
    /// <summary>
    /// ����һ���µĺ���ͨ��֤ҳ��
    /// </summary>
    public HutaoPassportPage()
    {
        InitializeWith<HutaoPassportViewModel>();
        InitializeComponent();
    }
}

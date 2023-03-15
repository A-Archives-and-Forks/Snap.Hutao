// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Control;
using Snap.Hutao.ViewModel.Wiki;

namespace Snap.Hutao.View.Page;

/// <summary>
/// ����ҳ��
/// </summary>
[HighQuality]
internal sealed partial class WikiWeaponPage : ScopedPage
{
    /// <summary>
    /// ����һ���µ�����ҳ��
    /// </summary>
    public WikiWeaponPage()
    {
        InitializeWith<WikiWeaponViewModel>();
        InitializeComponent();
    }
}

// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.UI.Xaml.Control;
using Snap.Hutao.ViewModel.DailyNote;

namespace Snap.Hutao.UI.Xaml.View.Page;

internal sealed partial class DailyNotePage : ScopedPage
{
    public DailyNotePage()
    {
        InitializeComponent();
    }

    protected override void LoadingOverride()
    {
        InitializeWith<DailyNoteViewModel>();
    }
}
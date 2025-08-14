// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;

namespace Snap.Hutao.UI.Xaml.Control.AutoSuggestBox;

[DependencyProperty<string>("Text")]
internal sealed partial class PretokenStringContainer : DependencyObject, ITokenStringContainer
{
    public PretokenStringContainer(bool isLast = false)
    {
        IsLast = isLast;
    }

    public PretokenStringContainer(string text)
    {
        Text = text;
    }

    public bool IsLast { get; }

    public override string ToString()
    {
        return Text;
    }
}
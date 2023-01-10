// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Web.Hoyolab.Passport;

namespace Snap.Hutao.View.Dialog;

/// <summary>
/// ��¼��������Ի���
/// </summary>
public sealed partial class LoginMihoyoBBSDialog : ContentDialog
{
    /// <summary>
    /// ����һ���µĵ�¼��������Ի���
    /// </summary>
    /// <param name="window">����</param>
    public LoginMihoyoBBSDialog()
    {
        InitializeComponent();
        XamlRoot = Ioc.Default.GetRequiredService<MainWindow>().Content.XamlRoot;
    }

    /// <summary>
    /// �첽��ȡ�û�������˺�����
    /// </summary>
    /// <returns>�˺�����</returns>
    public async Task<ValueResult<bool, Dictionary<string, string>>> GetInputAccountPasswordAsync()
    {
        await ThreadHelper.SwitchToMainThreadAsync();
        bool result = await ShowAsync() == ContentDialogResult.Primary;

        Dictionary<string, string> data = new()
        {
            { "account", RSAEncryptedString.Encrypt(AccountTextBox.Text) },
            { "password", RSAEncryptedString.Encrypt(PasswordTextBox.Password) },
        };

        return new(result, data);
    }
}

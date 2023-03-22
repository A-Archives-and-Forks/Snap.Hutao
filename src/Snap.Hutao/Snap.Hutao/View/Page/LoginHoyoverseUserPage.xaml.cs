// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.Web.WebView2.Core;
using Snap.Hutao.Service.Abstraction;
using Snap.Hutao.Service.Navigation;
using Snap.Hutao.Service.User;
using Snap.Hutao.View.Dialog;
using Snap.Hutao.Web.Hoyolab;
using Snap.Hutao.Web.Hoyolab.Passport;
using Snap.Hutao.Web.Hoyolab.Takumi.Auth;
using Snap.Hutao.Web.Response;
using System.Diagnostics.Eventing.Reader;

namespace Snap.Hutao.View.Page;

/// <summary>
/// ��¼�׹���ͨ��֤ҳ��
/// </summary>
[HighQuality]
internal sealed partial class LoginHoyoverseUserPage : Microsoft.UI.Xaml.Controls.Page
{
    /// <summary>
    /// ����һ���µĵ�¼�׹���ͨ��֤ҳ��
    /// </summary>
    public LoginHoyoverseUserPage()
    {
        InitializeComponent();
    }

    [SuppressMessage("", "VSTHRD100")]
    private async void OnRootLoaded(object sender, RoutedEventArgs e)
    {
        try
        {
            await WebView.EnsureCoreWebView2Async();


            CoreWebView2CookieManager manager = WebView.CoreWebView2.CookieManager;
            IReadOnlyList<CoreWebView2Cookie> cookies = await manager.GetCookiesAsync("https://account.hoyolab.com");
            foreach (CoreWebView2Cookie item in cookies)
            {
                manager.DeleteCookie(item);
            }

            WebView.CoreWebView2.Navigate("https://account.hoyolab.com/#/login");
        }
        catch (Exception ex)
        {
            Ioc.Default.GetRequiredService<IInfoBarService>().Error(ex);
        }
    }

    private async Task HandleCurrentCookieAsync(CancellationToken token)
    {
        CoreWebView2CookieManager manager = WebView.CoreWebView2.CookieManager;
        IReadOnlyList<CoreWebView2Cookie> cookies = await manager.GetCookiesAsync("https://account.hoyolab.com");

        IInfoBarService infoBarService = Ioc.Default.GetRequiredService<IInfoBarService>();

        // Get user id from text input, login_uid is missed in cookie
        string uid = UidInput.Text;

        if (uid.Length != 9)
        {
            await ThreadHelper.SwitchToMainThreadAsync();
            infoBarService.Warning($"����ҳ�����Ϸ����������д���ͨ��֤ ID!");
            return;
        }

        Cookie loginTicketCookie = Cookie.FromCoreWebView2Cookies(cookies);
        loginTicketCookie["login_uid"] = uid;

        // ʹ�� loginTicket ��ȡ stoken
        Response<ListWrapper<NameToken>> multiTokenResponse = await Ioc.Default
            .GetRequiredService<AuthClientOs>()
            .GetMultiTokenByLoginTicketAsync(loginTicketCookie, token)
            .ConfigureAwait(false);

        if (!multiTokenResponse.IsOk())
        {
            return;
        }

        Dictionary<string, string> multiTokenMap = multiTokenResponse.Data.List.ToDictionary(n => n.Name, n => n.Token);
        Cookie hoyoLabCookie = Cookie.Parse($"stoken={multiTokenMap["stoken"]}; stuid={uid}");

        // ���� cookie ������û�
        (UserOptionResult result, string nickname) = await Ioc.Default
            .GetRequiredService<IUserService>()
            .ProcessInputOsCookieAsync(hoyoLabCookie)
            .ConfigureAwait(false);

        Ioc.Default.GetRequiredService<INavigationService>().GoBack();

        switch (result)
        {
            case UserOptionResult.Added:
                ViewModel.UserViewModel vm = Ioc.Default.GetRequiredService<ViewModel.UserViewModel>();
                if (vm.Users!.Count == 1)
                {
                    await ThreadHelper.SwitchToMainThreadAsync();
                    vm.SelectedUser = vm.Users.Single();
                }

                infoBarService.Success($"�û� [{nickname}] ��ӳɹ�");
                break;
            case UserOptionResult.Incomplete:
                infoBarService.Information($"�� Cookie ������������ʧ��");
                break;
            case UserOptionResult.Invalid:
                infoBarService.Information($"�� Cookie ��Ч������ʧ��");
                break;
            case UserOptionResult.Updated:
                infoBarService.Success($"�û� [{nickname}] ���³ɹ�");
                break;
            default:
                throw Must.NeverHappen();
        }
    }

    private void CookieButtonClick(object sender, RoutedEventArgs e)
    {
        HandleCurrentCookieAsync(CancellationToken.None).SafeForget();
    }
}

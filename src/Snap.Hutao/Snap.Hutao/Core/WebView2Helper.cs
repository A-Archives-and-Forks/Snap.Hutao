﻿// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Web.WebView2.Core;
using Snap.Hutao.Core.Logging;
using System.IO;

namespace Snap.Hutao.Core;

/// <summary>
/// 检测 WebView2运行时 是否存在
/// 不再使用注册表检查方式
/// 必须为抽象类才能使用泛型日志器
/// </summary>
internal abstract class WebView2Helper
{
    private static bool hasEverDetected;
    private static bool isSupported;
    private static string version = "未检测到 WebView2 运行时";

    /// <summary>
    /// 检测 WebView2 是否存在
    /// </summary>
    public static bool IsSupported
    {
        get
        {
            if (!hasEverDetected)
            {
                hasEverDetected = true;
                isSupported = true;
                try
                {
                    version = CoreWebView2Environment.GetAvailableBrowserVersionString();
                }
                catch (FileNotFoundException ex)
                {
                    ILogger<WebView2Helper> logger = Ioc.Default.GetRequiredService<ILogger<WebView2Helper>>();
                    logger.LogError(EventIds.WebView2EnvironmentException, ex, "WebView2 运行时未安装");
                    isSupported = false;
                }
            }

            return isSupported;
        }
    }

    /// <summary>
    /// WebView2的版本
    /// </summary>
    public static string Version
    {
        get => version;
    }
}
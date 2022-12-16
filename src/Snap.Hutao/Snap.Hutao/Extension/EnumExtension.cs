﻿// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Reflection;

namespace Snap.Hutao.Extension;

/// <summary>
/// 枚举拓展
/// </summary>
public static class EnumExtension
{
    /// <summary>
    /// 获取枚举的描述
    /// </summary>
    /// <typeparam name="TEnum">枚举的类型</typeparam>
    /// <param name="enum">枚举值</param>
    /// <returns>描述</returns>
    public static string GetDescription<TEnum>(this TEnum @enum)
        where TEnum : struct, Enum
    {
        string enumName = Enum.GetName(@enum)!;
        FieldInfo? field = @enum.GetType().GetField(enumName);
        DescriptionAttribute? attr = field?.GetCustomAttribute<DescriptionAttribute>();
        return attr?.Description ?? enumName;
    }

    /// <summary>
    /// 获取枚举的描述
    /// </summary>
    /// <typeparam name="TEnum">枚举的类型</typeparam>
    /// <param name="enum">枚举值</param>
    /// <returns>描述</returns>
    public static string? GetDescriptionOrNull<TEnum>(this TEnum @enum)
        where TEnum : struct, Enum
    {
        string enumName = Enum.GetName(@enum)!;
        FieldInfo? field = @enum.GetType().GetField(enumName);
        DescriptionAttribute? attr = field?.GetCustomAttribute<DescriptionAttribute>();
        return attr?.Description;
    }
}
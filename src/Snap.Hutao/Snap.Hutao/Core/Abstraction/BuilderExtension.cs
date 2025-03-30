// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Diagnostics;

namespace Snap.Hutao.Core.Abstraction;

internal static class BuilderExtension
{
    [DebuggerStepThrough]
    public static T Configure<T>(this T builder, Action<T> configure)
        where T : class, IBuilder
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(configure);

        configure(builder);
        return builder;
    }

    [DebuggerStepThrough]
    public static unsafe T Configure<T>(this T builder, delegate*<T, void> configure)
        where T : class, IBuilder
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(configure);

        configure(builder);
        return builder;
    }

    [DebuggerStepThrough]
    public static T If<T>(this T builder, bool condition, Action<T> action)
        where T : class, IBuilder
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(action);

        if (condition)
        {
            action(builder);
        }

        return builder;
    }

    [DebuggerStepThrough]
    public static unsafe T If<T>(this T builder, bool condition, delegate*<T, void> action)
        where T : class, IBuilder
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(action);

        if (condition)
        {
            action(builder);
        }

        return builder;
    }

    [DebuggerStepThrough]
    public static T IfNot<T>(this T builder, bool condition, Action<T> action)
        where T : class, IBuilder
    {
        return builder.If(!condition, action);
    }

    [DebuggerStepThrough]
    public static unsafe T IfNot<T>(this T builder, bool condition, delegate*<T, void> action)
        where T : class, IBuilder
    {
        return builder.If(!condition, action);
    }
}
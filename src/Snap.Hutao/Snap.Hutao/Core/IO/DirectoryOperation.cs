// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualBasic.FileIO;
using Snap.Hutao.Win32.System.Com;
using Snap.Hutao.Win32.UI.Shell;
using System.IO;
using WinRT;
using static Snap.Hutao.Win32.Macros;
using static Snap.Hutao.Win32.Ole32;
using static Snap.Hutao.Win32.Shell32;

namespace Snap.Hutao.Core.IO;

internal static class DirectoryOperation
{
    public static bool Copy(string sourceDirName, string destDirName)
    {
        if (!Directory.Exists(sourceDirName))
        {
            return false;
        }

        try
        {
            FileSystem.CopyDirectory(sourceDirName, destDirName, true);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static bool Move(string sourceDirName, string destDirName)
    {
        if (!Directory.Exists(sourceDirName))
        {
            return false;
        }

        try
        {
            FileSystem.MoveDirectory(sourceDirName, destDirName, true);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static bool UnsafeRename(string path, string name, FILEOPERATION_FLAGS flags = FILEOPERATION_FLAGS.FOF_ALLOWUNDO | FILEOPERATION_FLAGS.FOF_NOCONFIRMMKDIR)
    {
        if (!SUCCEEDED(CoCreateInstance(in Win32.UI.Shell.FileOperation.CLSID, default, CLSCTX.CLSCTX_INPROC_SERVER, in IFileOperation.IID, out ObjectReference<IFileOperation.Vftbl> fileOperation)))
        {
            return false;
        }

        using (fileOperation)
        {
            if (!SUCCEEDED(SHCreateItemFromParsingName(path, default, in IShellItem.IID, out ObjectReference<IShellItem.Vftbl> shellItem)))
            {
                return false;
            }

            using (shellItem)
            {
                fileOperation.SetOperationFlags(flags);
                fileOperation.RenameItem(shellItem, name, default);

                return SUCCEEDED(fileOperation.PerformOperations());
            }
        }
    }
}
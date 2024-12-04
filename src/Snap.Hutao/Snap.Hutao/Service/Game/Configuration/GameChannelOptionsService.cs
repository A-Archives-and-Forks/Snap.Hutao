// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.IO.Ini;
using System.IO;

namespace Snap.Hutao.Service.Game.Configuration;

[ConstructorGenerated]
[Injection(InjectAs.Singleton, typeof(IGameChannelOptionsService))]
internal sealed partial class GameChannelOptionsService : IGameChannelOptionsService
{
    private readonly IGameConfigurationFileService gameConfigurationFileService;
    private readonly LaunchOptions launchOptions;

    public ChannelOptions GetChannelOptions()
    {
        if (!launchOptions.TryGetGameFileSystem(out IGameFileSystem? gameFileSystem))
        {
            return ChannelOptions.GamePathNullOrEmpty();
        }

        using (gameFileSystem)
        {
            if (!File.Exists(gameFileSystem.GetGameConfigurationFilePath()))
            {
                // Try restore the configuration file if it does not exist
                // The configuration file may be deleted by an incompatible launcher
                gameConfigurationFileService.Restore(gameFileSystem.GetGameConfigurationFilePath());
            }

            if (!File.Exists(gameFileSystem.GetScriptVersionFilePath()))
            {
                // Try to fix ScriptVersion by reading game_version from the configuration file
                // Will check the configuration file first
                // If the configuration file and ScriptVersion file are both missing, the game content is corrupted
                if (!gameFileSystem.TryFixScriptVersion())
                {
                    return ChannelOptions.GameContentCorrupted(gameFileSystem.GetGameDirectory());
                }
            }

            if (!File.Exists(gameFileSystem.GetGameConfigurationFilePath()))
            {
                return ChannelOptions.ConfigurationFileNotFound(gameFileSystem.GetGameConfigurationFilePath());
            }

            string? channel = default;
            string? subChannel = default;
            foreach (ref readonly IniElement element in IniSerializer.DeserializeFromFile(gameFileSystem.GetGameConfigurationFilePath()).AsSpan())
            {
                if (element is not IniParameter parameter)
                {
                    continue;
                }

                switch (parameter.Key)
                {
                    case ChannelOptions.ChannelName:
                        channel = parameter.Value;
                        break;
                    case ChannelOptions.SubChannelName:
                        subChannel = parameter.Value;
                        break;
                }

                if (channel is not null && subChannel is not null)
                {
                    break;
                }
            }

            return new(channel, subChannel, gameFileSystem.IsOversea());
        }
    }
}
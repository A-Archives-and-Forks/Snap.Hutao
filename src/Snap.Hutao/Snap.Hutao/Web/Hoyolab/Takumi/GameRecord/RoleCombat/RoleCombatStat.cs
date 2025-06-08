// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;
using System.Collections.Immutable;

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.RoleCombat;

internal sealed class RoleCombatStat
{
    [JsonPropertyName("difficulty_id")]
    public RoleCombatDifficultyLevel DifficultyId { get; set; }

    [JsonPropertyName("max_round_id")]
    public uint MaxRoundId { get; set; }

    [JsonPropertyName("heraldry")]
    public RoleCombatDifficultyLevel Heraldry { get; set; }

    [JsonPropertyName("get_medal_round_list")]
    public ImmutableArray<int> GetMedalRoundList { get; set; }

    [JsonPropertyName("medal_num")]
    public int MedalNumber { get; set; }

    [JsonPropertyName("coin_num")]
    public int CoinNumber { get; set; }

    [JsonPropertyName("avatar_bonus_num")]
    public int AvatarBonusNumber { get; set; }

    [JsonPropertyName("rent_cnt")]
    public int RentCount { get; set; }
}
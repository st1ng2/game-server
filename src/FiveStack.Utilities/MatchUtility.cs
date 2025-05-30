using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using FiveStack.Entities;
using FiveStack.Enums;
using Microsoft.Extensions.FileProviders;

namespace FiveStack.Utilities
{
    public static class MatchUtility
    {
        public static string GetSafeMatchPrefix(MatchData matchData)
        {
            return $"{matchData.id}_{matchData.current_match_map_id}".Replace("-", "");
        }

        public static MatchMember? GetMemberFromLineup(
            MatchData matchData,
            string steamId,
            string playerName
        )
        {
            List<MatchMember> players = matchData
                .lineup_1.lineup_players.Concat(matchData.lineup_2.lineup_players)
                .ToList();

            return players.Find(member =>
            {
                if (member.steam_id == null)
                {
                    return member.placeholder_name.StartsWith(playerName);
                }

                return member.steam_id == steamId;
            });
        }

        public static Guid? GetPlayerLineup(MatchData matchData, CCSPlayerController player)
        {
            MatchMember? member = MatchUtility.GetMemberFromLineup(
                matchData,
                player.SteamID.ToString(),
                player.PlayerName
            );

            if (member == null)
            {
                return null;
            }

            return member.match_lineup_id;
        }

        public static eMapStatus MapStatusStringToEnum(string state)
        {
            switch (state)
            {
                case "Scheduled":
                    return eMapStatus.Scheduled;
                case "Finished":
                    return eMapStatus.Finished;
                case "Knife":
                    return eMapStatus.Knife;
                case "Live":
                    return eMapStatus.Live;
                case "Overtime":
                    return eMapStatus.Overtime;
                case "Paused":
                    return eMapStatus.Paused;
                case "Warmup":
                    return eMapStatus.Warmup;
                case "Unknown":
                    return eMapStatus.Unknown;
                default:
                    throw new ArgumentException($"Unsupported status string: {state}");
            }
        }

        public static CCSGameRules? Rules()
        {
            return CounterStrikeSharp
                .API.Utilities.FindAllEntitiesByDesignerName<CCSGameRulesProxy>("cs_gamerules")
                ?.First()
                ?.GameRules;
        }

        public static List<CCSPlayerController> Players()
        {
            var players = CounterStrikeSharp.API.Utilities.GetPlayers();
            var validPlayers = new List<CCSPlayerController>();

            foreach (var player in players)
            {
                if (
                    !player.IsBot
                    && player.IsValid
                    && player.UserId != null
                    && player.PlayerName != "SourceTV"
                )
                {
                    validPlayers.Add(player);
                }
            }

            return validPlayers;
        }

        public static IEnumerable<CCSTeam> Teams()
        {
            return CounterStrikeSharp.API.Utilities.FindAllEntitiesByDesignerName<CCSTeam>(
                "cs_team_manager"
            );
        }
    }
}

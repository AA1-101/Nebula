using AmongUs.Data;
using Discord;
using HarmonyLib;

namespace Nebula.Patches;

// Originally from "Town of Us Rewritten", by Det
[HarmonyPatch(typeof(ActivityManager), nameof(ActivityManager.UpdateActivity))]
public static class DiscordPatch
{
    private static string Lobbycode = "";
    private static string Region = "";
    [HarmonyPrefix]
    public static void Prefix([HarmonyArgument(0)] Activity activity)
    {
        Main.Logger.LogInfo("DiscordPatch called");
        if (activity == null) return;

        var details = $"Nebula v{Main.PluginDisplayVersion}";
        activity.Details = details;

        activity.Assets = new ActivityAssets
        {
            LargeImage = "https://i.ibb.co/Q3r7nKCV/file-00B0000005f9472079c2f406e2ae9a354.png"
        };

        try
        {
            if (activity.State != "In Menus")
            {
                if (!DataManager.Settings.Gameplay.StreamerMode)
                {
                    if (Modules.GameStates.IsInLobby)
                    {
                        Lobbycode = GameStartManager.Instance.GameRoomNameCode.text;
                        Region = Modules.Utils.GetRegionName();
                    }

                    if (Lobbycode != "" && Region != "") details = $"Nebula - {Lobbycode} ({Region})";
                }
                else
                    details = $"Nebula v{Main.PluginDisplayVersion}";

                activity.Details = details;
            }
        }
        catch (Exception ex)
        {
            Main.Logger.LogInfo("Error in updating discord rpc");            
            details = $"Nebula v{Main.PluginDisplayVersion}";
            activity.Details = details;
        }
    }
}
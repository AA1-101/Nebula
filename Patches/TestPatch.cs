using HarmonyLib;
using System.Collections;

namespace Nebula.Patches;

[HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnGameJoined))]
internal static class TestPatch
{
    public static void Postfix()
    {
        PlayerControl player = PlayerControl.LocalPlayer;

        Main.Logger.LogInfo("OnGameJoined called");

        Main.Instance.StartCoroutine(PlayerNullDelay());
            
        if (!AmongUsClient.Instance.AmHost)
            return;

        if (Modules.GameStates.GameStarted)
            return;

        string displayName = TagManager.BuildName(player) + $"<color=#a54aff>{player.Data.PlayerName}</color>";
        Main.Logger.LogInfo("Name Set");

        player.RpcSetName(displayName);                   
    }

    public static IEnumerator PlayerNullDelay()
    {
        while (PlayerControl.LocalPlayer == null)
        yield return null;
    }
}
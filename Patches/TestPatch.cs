using HarmonyLib;
using System.Collections;

namespace Nebula.Patches;

[HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnGameJoined))]
internal static class TestPatch
{
    public static void Postfix()
    {
        Main.Logger.LogInfo("OnGameJoined called");        
        Main.Instance.StartCoroutine(DisplayName());
    }

    public static IEnumerator DisplayName()
    {
        while (PlayerControl.LocalPlayer == null)
            yield return null;

        PlayerControl player = PlayerControl.LocalPlayer;

        while (player.Data == null)
            yield return null;

        if (!AmongUsClient.Instance.AmHost)
            yield break;

        if (Modules.GameStates.GameStarted)
            yield break;

        string displayName = TagManager.BuildName(player) + $"<color=#a54aff>{player.Data.PlayerName}</color>";
        Main.Logger.LogInfo("Name Set");

        player.RpcSetName(displayName);
    }
}
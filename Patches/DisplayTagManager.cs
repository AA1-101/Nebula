using HarmonyLib;
using System.Collections;

namespace Nebula.Patches;

[HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnGameJoined))]
internal static class DisplayTagmanager
{
    public static void Postfix()
    {
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

        string displayName = TagManager.BuildName(player);

        player.RpcSetName(displayName);
    }

    public static void RefreshName()
    {
        if (!AmongUsClient.Instance.AmHost)
            return;

        var player = PlayerControl.LocalPlayer;

        if (player?.Data == null)
            return;

        player.RpcSetName(TagManager.BuildName(player));
    }
}
using HarmonyLib;
using Nebula.Modules;
using System.Collections;

namespace Nebula.Patches;

[HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnPlayerJoined))]
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

        if (GameStates.GameStarted)
            yield break;

        foreach (PlayerControl pc in PlayerControl.AllPlayerControls)
        {
            if (pc.Data == null)
                continue;

            pc.RpcSetName(TagManager.BuildName(pc));
        }
    }

    public static void RefreshNames()
    {
        if (!AmongUsClient.Instance.AmHost)
            return;

        foreach (PlayerControl pc in PlayerControl.AllPlayerControls)
        {
            if (pc.Data == null)
                continue;

            pc.RpcSetName(TagManager.BuildName(pc));
        }
    }
}
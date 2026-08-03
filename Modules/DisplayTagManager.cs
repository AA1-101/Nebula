using HarmonyLib;
using System.Collections;

namespace Nebula.Modules;

[HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnPlayerJoined))]
public static class DisplayTagManager
{  
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
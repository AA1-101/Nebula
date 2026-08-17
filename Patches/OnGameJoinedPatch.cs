using HarmonyLib;
using Nebula.Modules;
using Nebula.Networking;

namespace Nebula.Patches;

[HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnGameJoined))]
public static class OnGameJoinedPatch
{
    public static void Postfix()
    {
        DisplayTagManager.OriginalNames.Clear();

        Main.Instance.StartCoroutine(DisplayTagManager.DisplayName());
    }
}



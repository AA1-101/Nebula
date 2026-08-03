using HarmonyLib;
using Nebula.Modules;

namespace Nebula.Patches;

[HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnGameJoined))]
public static class OnGameJoinedPatch
{
    public static void Postfix()
    {
        Main.Instance.StartCoroutine(DisplayTagManager.DisplayName());
    }
}



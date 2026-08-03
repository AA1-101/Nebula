using HarmonyLib;
using Nebula.Modules;

namespace Nebula.Patches;

[HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnPlayerJoined))]
public static class OnPlayerJoinedPatch
{
    public static void Postfix()
    {
        Main.Instance.StartCoroutine(DisplayTagManager.DisplayName());
    }
}


    
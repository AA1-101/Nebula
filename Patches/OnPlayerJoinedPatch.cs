using HarmonyLib;
using InnerNet;
using Nebula.Modules;
using Nebula.Networking;

namespace Nebula.Patches;

[HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnPlayerJoined))]
public static class OnPlayerJoinedPatch
{
    public static void Postfix(ClientData __instance)
    {
        Main.Instance.StartCoroutine(DisplayTagManager.DisplayName());

        RpcSender.SendHandshake(__instance.Character);
    }
}


    
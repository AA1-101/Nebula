using HarmonyLib;
using Nebula;
using Nebula.Modules;
using Nebula.Networking;

[HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnGameJoined))]
public static class OnGameJoinedPatch
{
    public static void Postfix()
    {     
        Main.Instance.StartCoroutine(DisplayTagManager.DisplayName());

        Main.Instance.StartCoroutine(RpcSender.SendHandshakeWhenReady());
    }    
}
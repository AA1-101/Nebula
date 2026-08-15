using AmongUs.Data;
using HarmonyLib;
using UnityEngine;
using Nebula.Modules;

namespace Nebula.Patches
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.RpcSendChat))]
    public static class ChatCommandPatch
    {
        public static bool Prefix(PlayerControl __instance, string chatText)
        {
            if (!AmongUsClient.Instance.AmHost)
                return true;

            if (!chatText.StartsWith("/cmd", StringComparison.OrdinalIgnoreCase))
                return true;

            Main.IsChatCommand = true;

            Main.Logger.LogInfo($"Chat intercepted: {chatText}");

            return CommandManager.OnReceiveChat(__instance, chatText);            
        }        
    }  
    
}


        
    


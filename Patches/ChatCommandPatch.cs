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

    [HarmonyPatch(typeof(ChatController), nameof(ChatController.AddChat))]

    public static class ChatControllerPatch
    {
        [HarmonyPrefix]
        public static bool Prefix(ChatController __instance,PlayerControl sourcePlayer, string chatText)
        {            
            if (!sourcePlayer || !PlayerControl.LocalPlayer)
            {
                return false;
            }
            NetworkedPlayerInfo data = PlayerControl.LocalPlayer.Data;
            NetworkedPlayerInfo data2 = sourcePlayer.Data;
            if (data2 == null || data == null || (data2.IsDead && !data.IsDead))
            {
                return false;
            }
            ChatBubble pooledBubble = __instance.GetPooledBubble();
            try
            {
                pooledBubble.transform.SetParent(__instance.scroller.Inner);
                pooledBubble.transform.localScale = Vector3.one;
                bool flag = sourcePlayer == PlayerControl.LocalPlayer;
                if (flag)
                {
                    pooledBubble.SetRight();
                }
                else
                {
                    pooledBubble.SetLeft();
                }
                bool didVote = MeetingHud.Instance && MeetingHud.Instance.DidVote(sourcePlayer.PlayerId);
                pooledBubble.SetCosmetics(data2);
                __instance.SetChatBubbleName(pooledBubble, data2, data2.IsDead, didVote, PlayerNameColor.Get(data2), null);
              
                pooledBubble.SetText(chatText);
                pooledBubble.AlignChildren();
                __instance.AlignAllBubbles();
                if (!__instance.IsOpenOrOpening && __instance.notificationRoutine == null)
                {
                    __instance.notificationRoutine = __instance.StartCoroutine(__instance.BounceDot());
                }
                if (!flag && !__instance.IsOpenOrOpening)
                {
                    SoundManager.Instance.PlaySound(__instance.messageSound, false, 1f, null).pitch = 0.5f + (float)sourcePlayer.PlayerId / 15f;
                    __instance.chatNotification.SetUp(sourcePlayer, chatText);
                }
            }
            catch 
            {
                __instance.chatBubblePool.Reclaim(pooledBubble);
            }

            return false;
        }
    }
}


        
    


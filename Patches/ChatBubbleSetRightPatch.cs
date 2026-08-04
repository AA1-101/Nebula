using HarmonyLib;

namespace Nebula.Patches
{
    [HarmonyPatch(typeof (ChatBubble), nameof(ChatBubble.SetRight))]
    public static class ChatBubbleSetRightPatch
    {
        public static void Postfix(ChatBubble __instance)
        {
            if (!Main.IsChatCommand)
                return;

            __instance.SetLeft();

            Main.IsChatCommand = false;
        }
    }
}

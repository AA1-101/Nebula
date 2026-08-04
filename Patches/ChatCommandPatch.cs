using HarmonyLib;
using Nebula.Modules;
using Nebula.Networking;

namespace Nebula.Patches
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.RpcSendChat))]
    public static class ChatCommandPatch
    {
        public static bool Prefix(PlayerControl __instance,string chatText)
        {
            if (!chatText.StartsWith("/cmd", StringComparison.OrdinalIgnoreCase))
                return true;

            Main.IsChatCommand = true;

            Main.Logger.LogInfo($"Chat intercepted: {chatText}");

            Utils.CheckServerCommand(ref chatText);

            chatText = chatText[1..];

            string[] split = chatText.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (split.Length == 0)
            {
                RpcSender.SendMessage(
                    __instance,
                    "Usage: /cmd <command>\nTry using /cmd help",
                    sendTo: __instance.OwnerId);

                return false;
            }           

            string command = split[0].ToLowerInvariant();
            string[] args = split.Skip(1).ToArray();

            CommandManager.HandleCommand(__instance, command, args);

            return false;
        }
    }
}

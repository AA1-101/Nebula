using AmongUs.GameOptions;
using AmongUs.QuickChat;
using HarmonyLib;
using Hazel;
using Nebula.Modules;
using System.Collections;
using UnityEngine;

namespace Nebula.Networking
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.HandleRpc))] //https://github.com/Gurge44/EndlessHostRoles/blob/main/Modules/RPC.cs
    internal static class RpcHandler
    {
        public static bool Prefix(PlayerControl __instance, [HarmonyArgument(0)] byte callId, [HarmonyArgument(1)] MessageReader reader)
        {
            var rpcType = (RpcCalls)callId;
            MessageReader subReader = MessageReader.Get(reader);

            try
            {
                if (__instance != null)
                {
                    switch (rpcType)
                    {
                        case RpcCalls.SetName:
                            subReader.ReadUInt32();
                            string name = subReader.ReadString();

                            if (subReader.BytesRemaining > 0 && subReader.ReadBoolean())
                            {
                                return false;
                            }
                            break;
                        case RpcCalls.SetRole:
                            var role = (RoleTypes)subReader.ReadUInt16();
                            bool canOverriddenRole = subReader.ReadBoolean();
                            break;
                        case RpcCalls.SendChat:
                            string chatText = subReader.ReadString();
                            if (!CommandManager.OnReceiveChat(__instance, chatText))
                            {
                                return false;
                            }
                            break;
                        case RpcCalls.SendQuickChat:
                            string quickText = QuickChatNetData.Deserialize(subReader).ToChatText();
                            CommandManager.OnReceiveChat(__instance, quickText);
                            break;
                        case RpcCalls.StartMeeting:
                            PlayerControl p = Utils.GetPlayerById(subReader.ReadByte());
                            break;
                        case RpcCalls.Pet:
                            break;
                    }
                }
            }
            finally
            {
                subReader.Recycle();
            }

            return true;
        }
        public static void Postfix(PlayerControl __instance, [HarmonyArgument(0)] byte callId, [HarmonyArgument(1)] MessageReader reader)
        {
            var rpcType = (CustomRpc)callId;

            switch (rpcType)
            {
                case CustomRpc.ModdedClientInfo:

                    if (!AmongUsClient.Instance.AmHost)
                        return;

                    string guid = reader.ReadString();
                    string version = reader.ReadString();

                    if (guid == Main.PluginGuid && version == Main.PluginVersion)
                    {
                        if (Main.ModdedClients.ContainsKey(__instance.PlayerId))
                            return;

                        Main.ModdedClients.Add(__instance.PlayerId, __instance);
                        Main.Logger.LogInfo($"{__instance.Data.PlayerName} recognized as Nebula Client");
                    }
                    else
                    {
                        Main.Instance.StartCoroutine(RPC.VersionMismatch(__instance));
                    }

                    break;
            }

        }
    }

    internal static class RPC
    {
        public static IEnumerator VersionMismatch(PlayerControl player)
        {
            RpcSender.SendMessage($"{player.Data.PlayerName}, you are running the wrong verion of Nebula!\n" +
                $"You will be kicked in 5 seconds. Please update.", sendTo: player.OwnerId);

            yield return new WaitForSeconds(3f);

            AmongUsClient.Instance.KickPlayer(player.PlayerId, false);

            PlayerControl host = Utils.GetHost();

            RpcSender.SendMessage($"{player.Data.PlayerName} was kicked for having the wrong version", sendTo: host.OwnerId);
        }
    }
}



using AmongUs.GameOptions;
using AmongUs.QuickChat;
using HarmonyLib;
using Hazel;
using Nebula.Modules;

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
                            string text = subReader.ReadString();
                            if (!CommandManager.OnReceiveChat(__instance, text))
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
    }
}

using Hazel;
using System.Collections;

namespace Nebula.Networking
{
    public static class RpcSender
    {
       public static void Send(uint netId,
           RpcCalls rpcCalls,
           Action<MessageWriter> write,
           int sendTo = -1)
        {
            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(netId, 
                (byte)rpcCalls,
                SendOption.Reliable,sendTo);

            write(writer);

            AmongUsClient.Instance.FinishRpcImmediately(writer);
        }
        public static void SendMessage(PlayerControl pc, string msg, string title = "<color=#a54aff>★[Nebula-System]★</color>", int sendTo = -1)
        {
            string originalName = pc.Data.PlayerName;

            if (sendTo == -1 && HudManager.InstanceExists)
            {
                pc.SetName(title);
                HudManager.Instance.Chat.AddChat(pc, msg);
                pc.SetName(originalName);
            }

            SetName(pc, title, sendTo);

            Send(pc.NetId, RpcCalls.SendChat, writer =>
            {
                writer.Write(msg);
            }, sendTo);

            Main.Instance.StartCoroutine(RestoreNameNextFrame(pc, originalName, sendTo));
        }

        public static void SetName(PlayerControl pc, string name, int sendTo = -1)
        {
            Send(pc.NetId, RpcCalls.SetName, writer =>
            {
                writer.Write(pc.Data.NetId);
                writer.Write(name);
            }, sendTo);
        }

        public static IEnumerator RestoreNameNextFrame(PlayerControl pc, string name, int sendTo = -1)
        {
            if (!AmongUsClient.Instance.AmHost)
                yield break;

            yield return null;

            SetName(pc, name, sendTo);
        }       
    }
}

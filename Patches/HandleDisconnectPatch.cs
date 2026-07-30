using HarmonyLib;
using InnerNet;
using Nebula.Modules;

namespace Nebula.Patches
{
    [HarmonyPatch(typeof(InnerNetClient), nameof(InnerNetClient.HandleDisconnect))]
    public class HandleDisconnectedPatch
    {
        public static void Postfix()
        {
            NebulaLogger.EndLog();
        }
    }
}

using HarmonyLib;
using Nebula.Modules;

namespace Nebula.Patches
{
    [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.CoStartGameHost))]
    public static class CoStartGameHostPatch
    {
        public static void Prefix()
        {
            if (!AmongUsClient.Instance.AmHost)
                return;

            DisplayTagManager.RefreshNames();
        }
    }
}
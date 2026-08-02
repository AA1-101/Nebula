using HarmonyLib;
using Nebula.Modules;

namespace EHR.Patches;

[HarmonyPatch(typeof(Constants), nameof(Constants.GetBroadcastVersion))] //https://github.com/Gurge44/EndlessHostRoles/blob/main/Patches/ServerVersionPatch.cs
internal static class ServerUpdatePatch
{
    public static void Postfix(ref int __result)
    {
        if (GameStates.IsOnlineGame)
        {
            // Changing server version for AU mods
            var revision = __result % 50;
            if (revision < 25)
            {
                __result += 25;
            }
        }
    }
}

[HarmonyPatch(typeof(Constants), nameof(Constants.IsVersionModded))]
public static class IsVersionModdedPatch
{
    public static bool Prefix(ref bool __result)
    {
        __result = true;
        return false;
    }
}
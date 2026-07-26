using HarmonyLib;

namespace Nebula.Patches
{
    [HarmonyPatch(typeof(Constants), nameof(Constants.GetBroadcastVersion))]
    static class BroadcastVersionPatch
    {
        static void Postfix(ref int __result)
        {
            var version = Constants.GetVersionComponents(__result);

            int year = version.Item1;
            int month = version.Item2;
            int day = version.Item3;

            __result = Constants.GetVersion(year, month, day, 25);
        }
    }
}

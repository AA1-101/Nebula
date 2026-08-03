using HarmonyLib;

namespace Nebula.Patches
{
    [HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.Start))]
    public class GameStartManagerPatch
    {
        public static void Postfix()
        {
            GameStartManager.Instance.MinPlayers = 1;
        }
    }
}

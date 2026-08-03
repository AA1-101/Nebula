using HarmonyLib;
using Nebula.Modules;
using System.Collections;

namespace Nebula.Patches
{
    [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.CoStartGame))]
    public static class CoStartGamePatch
    {
        public static void Postfix()
        {
            Main.Instance.StartCoroutine(RemoveTags());
        }

        private static IEnumerator RemoveTags()
        {
            while (!GameStates.GameStarted)
                yield return null;

            if (!AmongUsClient.Instance.AmHost)
                yield break;

            DisplayTagmanager.RefreshNames();
        }
    }

}

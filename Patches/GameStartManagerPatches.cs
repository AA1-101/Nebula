using HarmonyLib;
using UnityEngine;

namespace Nebula.Patches
{
    [HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.Start))]
    public class MinPlayerPatch
    {
        public static void Postfix()
        {
            GameStartManager.Instance.MinPlayers = 1;
        }
    }
    [HarmonyPatch(typeof(GameStartManager), nameof (GameStartManager.Update))]

    public class StartGameShortcut
    {
        public static void Postfix()
        {
            if (!AmongUsClient.Instance.AmHost)
                return;

            // compare the instance startState to the enum values
            if (GameStartManager.Instance.startState == GameStartManager.StartingStates.Starting
                || GameStartManager.Instance.startState == GameStartManager.StartingStates.Countdown)
                return;

            // ensure Alt + S is required (fix precedence)
            if ((Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))
                && Input.GetKeyDown(KeyCode.S))
            {
                GameStartManager.Instance.BeginGame();
            }
        }
    }
}

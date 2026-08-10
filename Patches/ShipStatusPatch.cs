using HarmonyLib;
using UnityEngine;
using Nebula.Modules;

namespace Nebula.Patches
{
    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.FixedUpdate))]
    public static class ShipStatusPatch
    {
        public static void Postfix()
        {
            if (!AmongUsClient.Instance.AmHost)
                return;

            if (GameStates.IsInLobby)
                return;

            if (GameStates.IsInMeeting)
                return;

            if (Input.GetKey(KeyCode.LeftControl)
                &&( Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                && Input.GetKeyDown(KeyCode.P))
            {
                PlayerControl.LocalPlayer.RpcStartMeeting(null);
            }
        }
    }
}

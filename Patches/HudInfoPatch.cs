using HarmonyLib;
using NebulaMod;
using NebulaMod.Modules;
using System.Text;
using TMPro;
using UnityEngine;

namespace Nebula.Patches;

[HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.Start))]
public static class ModStampPatch
{
    public static void Postfix()
    {
        ModManager.Instance?.ShowModStamp();

    }
}

[HarmonyPatch(typeof(PingTracker), nameof(PingTracker.Update))]
internal static class HudInfoPatch
{
    private static readonly StringBuilder Sb = new();

    private static readonly float[] FpsBuffer = new float[10];
    private static int FpsIndex;
    private static int FpsCount;

    [HarmonyPrefix]
    public static bool Prefix(PingTracker __instance)
    {
        FpsSampler.TickFrame();

        var client = AmongUsClient.Instance;
        if (client == null)
            return false;

        if (client.NetworkMode == NetworkModes.FreePlay)
        {
            __instance.gameObject.SetActive(false);
            return false;
        }

        __instance.gameObject.SetActive(true);

        bool inGame = client.IsGameStarted;

        __instance.aspectPosition.DistanceFromEdge =
            inGame ? __instance.gamePos : __instance.lobbyPos;

        __instance.transform.localPosition =
            inGame ? new Vector3(-0.12f, 0f, 0f) : Vector3.zero;

        Sb.Clear();

        Sb.Append(inGame ? "<size=1.6>" : "<size=2.2>");

        // Header
        Sb.Append("<color=#a54aff>Nebula</color> ")
          .Append("v")
          .Append(Main.PluginDisplayVersion)
          .Append(" <color=#FFFFFF>by</color> ")
          .Append("<color=#00FFFF>AA-101</color>");

        int ping = client.Ping;

        string pingColor =
            ping < 30 ? "#46FFD4" :
            ping < 100 ? "#8CFF8C" :
            ping < 200 ? "#FFB347" :
            "#FF4F6D";

        Sb.Append(inGame ? "    -    " : "\n")
          .Append("<color=")
          .Append(pingColor)
          .Append(">Ping: ")
          .Append(ping)
          .Append(" ms</color>");

        if (FpsCount > 0)
        {
            float total = 0f;

            for (int i = 0; i < FpsCount; i++)
                total += FpsBuffer[i];

            int fps = Mathf.RoundToInt(total / FpsCount);

            string fpsColor =
                fps >= 60 ? "#00FF32" :
                fps >= 40 ? "#FFF700" :
                fps >= 20 ? "#FFA100" :
                "#FF0000";

            Sb.Append(inGame ? "    -    " : "  -  ")
              .Append("<color=")
              .Append(fpsColor)
              .Append(">FPS: ")
              .Append(fps)
              .Append("</color>");
        }

        // Region
        string region = Utils.GetRegionName();

        if (!string.IsNullOrEmpty(region))
        {
            Sb.Append(inGame ? "    -    " : "  -  ")
              .Append("<color=#6963FF>")
              .Append(region)
              .Append("</color>");
        }

        Sb.Append("</size>");

        if (inGame)
            Sb.Append("\n.");

        __instance.text.alignment = TextAlignmentOptions.Center;
        __instance.text.text = Sb.ToString();

        return false;
    }

    private static class FpsSampler
    {
        private static int Frames;
        private static float Elapsed;
        private const float SampleInterval = 0.25f;

        public static void TickFrame()
        {
            Frames++;
            Elapsed += Time.unscaledDeltaTime;

            if (Elapsed < SampleInterval)
                return;

            FpsBuffer[FpsIndex] = Frames / Elapsed;

            if (FpsCount < FpsBuffer.Length)
                FpsCount++;

            FpsIndex = (FpsIndex + 1) % FpsBuffer.Length;

            Frames = 0;
            Elapsed = 0f;
        }
    }
}
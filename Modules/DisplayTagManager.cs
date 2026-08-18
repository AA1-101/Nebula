using System.Collections;

namespace Nebula.Modules;

public static class DisplayTagManager
{
    public static Dictionary<byte, string> OriginalNames = new();

    public static IEnumerator DisplayName()
    {
        while (PlayerControl.LocalPlayer == null)
            yield return null;

        PlayerControl player = PlayerControl.LocalPlayer;

        while (player.Data == null)
            yield return null;

        if (!AmongUsClient.Instance.AmHost)
            yield break;

        if (GameStates.GameStarted)
            yield break;

        foreach (PlayerControl pc in PlayerControl.AllPlayerControls)
        {
            if (pc.Data == null)
                yield return null;

            while (string.IsNullOrEmpty(pc.Data.PlayerName))
                yield return null;

            if (!OriginalNames.ContainsKey(pc.PlayerId))
                OriginalNames[pc.PlayerId] = pc.Data.PlayerName;

            string displayName = TagManager.BuildName(pc, OriginalNames[pc.PlayerId]);

            Main.Logger.LogInfo($"Current: {pc.Data.PlayerName}");
            Main.Logger.LogInfo($"Built: {displayName}");

            if (pc.Data.PlayerName == displayName)
                continue;

            pc.RpcSetName(displayName);
        }
    }

    public static IEnumerator RefreshNames()
    {
        if (!AmongUsClient.Instance.AmHost)
            yield break;

        foreach (PlayerControl pc in PlayerControl.AllPlayerControls)
        {
            if (pc.Data == null)
                yield return null;

            while (string.IsNullOrEmpty(pc.Data.PlayerName))
                yield return null;

            if (!OriginalNames.ContainsKey(pc.PlayerId))
                OriginalNames[pc.PlayerId] = pc.Data.PlayerName;

            string displayName = TagManager.BuildName(pc, OriginalNames[pc.PlayerId]);

            Main.Logger.LogInfo($"Current: {pc.Data.PlayerName}");
            Main.Logger.LogInfo($"Built: {displayName}");

            if (pc.Data.PlayerName == displayName)
                continue;

            pc.RpcSetName(displayName);
        }
    }
}
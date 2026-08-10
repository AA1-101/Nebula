using System.Collections;

namespace Nebula.Modules;
public static class DisplayTagManager
{  
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
                continue;

            string displayName = TagManager.BuildName(pc);

            Main.Logger.LogInfo($"Current: {pc.Data.PlayerName}");
            Main.Logger.LogInfo($"Built: {displayName}");

            if (pc.Data.PlayerName == displayName)
                continue;

            pc.RpcSetName(displayName);
        }
    }

    public static void RefreshNames()
    {
        if (!AmongUsClient.Instance.AmHost)
            return;

        foreach (PlayerControl pc in PlayerControl.AllPlayerControls)
        {
            if (pc.Data == null)
                continue;

            string displayName = TagManager.BuildName(pc);

            Main.Logger.LogInfo($"Current: {pc.Data.PlayerName}");
            Main.Logger.LogInfo($"Built: {displayName}");

            if (pc.Data.PlayerName == displayName)
                continue;           

            pc.RpcSetName(displayName);
        }
    }   
}
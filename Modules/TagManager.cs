using Nebula.Modules;

public static class TagManager
{
    const string DevFriendCode = "palaceglad#5449";
    public static string BuildName(PlayerControl player, string originalName)
    {
        if (GameStates.GameStarted)
            return originalName;

        bool isHost = AmongUsClient.Instance.HostId == player.OwnerId;
        bool isDev = player.Data.FriendCode == DevFriendCode;

        if (isHost && isDev)
            return $"<color=#a54aff>[Nebula-Host]</color> <color=#00EB66>{originalName} [Developer]</color>";

        if (isDev)
            return $"<color=#00EB66>{originalName} ★[Developer]★</color>";

        if (isHost)
            return $"<color=#a54aff>★[Nebula-Host]★ {originalName}</color>";

        return originalName;
    }
}

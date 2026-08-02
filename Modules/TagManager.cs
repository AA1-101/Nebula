public static class TagManager
{
    public static string BuildName(PlayerControl player)
    {
        bool isHost = AmongUsClient.Instance.HostId == player.OwnerId;

        if (isHost)
            return "<color=#a54aff>[Nebula-Host]</color> ";

        return "";
    }
}
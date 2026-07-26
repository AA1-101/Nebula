using InnerNet;

namespace Nebula.Modules
{
    public static class GameStates
    {
        public static bool IsInLobby => AmongUsClient.Instance.GameState == InnerNetClient.GameStates.Joined;
    }
}

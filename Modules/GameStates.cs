using InnerNet;

namespace Nebula.Modules
{
    public static class GameStates
    {
        public static bool IsInLobby => AmongUsClient.Instance.GameState == InnerNetClient.GameStates.Joined;
        public static bool GameStarted => AmongUsClient.Instance.GameState == InnerNetClient.GameStates.Started;
        public static bool IsOnlineGame => AmongUsClient.Instance.NetworkMode == NetworkModes.OnlineGame;
        public static bool IsLocalGame => AmongUsClient.Instance.NetworkMode == NetworkModes.LocalGame;
        public static bool IsFreePlay => AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay;
        public static bool IsStarting => GameStartManager.Instance != null && GameStartManager.Instance.startState == GameStartManager.StartingStates.Countdown;
        public static bool IsInMeeting => MeetingHud.Instance != null;

    }
}

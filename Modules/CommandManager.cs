using HarmonyLib;
using Nebula.Networking;
using Rewired.Utils.Classes.Data;
using System.Collections;
using UnityEngine;

namespace Nebula.Modules
{
    public class Command
    {
        public Command(string name, UsageLevels usageLevel, UsageTimes usageTime)
        {
            Name = name;
            UsageLevel = usageLevel;
            UsageTime = usageTime;
        }

        public string Name { get; }

        public UsageLevels UsageLevel { get; }

        public UsageTimes UsageTime { get; }
        public enum UsageLevels
        {
            Everyone,           
            Host            
        }
        public enum UsageTimes
        {
            Always,
            InLobby,
            InGame            
        }

        public static bool CanUseCommand(PlayerControl pc, Command cmd)
        {
            switch (cmd.UsageLevel)
            {
                case UsageLevels.Host when !pc.IsHost():
                    return false;
            }

            switch (cmd.UsageTime)
            {
                case UsageTimes.InLobby when GameStates.GameStarted || GameStates.IsStarting:
                    return false;
            }

            return true;
        }
    }

    public static class CommandManager
    {
        public static List<Command> AllCommands = new List<Command>();

        public static void HandleCommand(PlayerControl player, string command, string[] args)
        {
            Main.Logger.LogInfo($"Handling command: {command}");
            Main.Logger.LogInfo($"Registered commands: {AllCommands.Count}");

            Command cmd = AllCommands.FirstOrDefault(x =>
                x.Name.Equals(command, StringComparison.OrdinalIgnoreCase));

            if (cmd == null)
            {
                RpcSender.SendMessage(player, "Unknown command.\nTry using /cmd help");
                return;
            }

            if (!Command.CanUseCommand(player, cmd))
            {
                RpcSender.SendMessage(player, "You cannot use this command!");
                return;
            }

            switch (command.ToLowerInvariant())
            {
                case "help":
                    HelpCommand(player);
                    break;

                case "start":
                    StartCommand(player, args);
                    break;

                case "id":
                    IdCommand(player);
                    break;
                case "ban":
                    BanCommand(player,args);
                    break;
            }
        }
        public static void LoadCommands()
        {
            AllCommands.Add(new Command("help", Command.UsageLevels.Everyone, Command.UsageTimes.Always));
            AllCommands.Add(new Command("start", Command.UsageLevels.Host, Command.UsageTimes.InLobby));
            AllCommands.Add(new Command("id", Command.UsageLevels.Everyone, Command.UsageTimes.Always));
            AllCommands.Add(new Command("ban", Command.UsageLevels.Host, Command.UsageTimes.Always));
        }

        public static void HelpCommand(PlayerControl player)
        {
            string message = "Available Commands";

            foreach (var cmd in AllCommands)
            {
                Main.Logger.LogInfo($"Adding command: {cmd.Name}");

                if (!Command.CanUseCommand(player, cmd))
                    continue;

                message += $"\n<b>/cmd {cmd.Name}</b>";
            }

            Main.Logger.LogInfo($"Sending message: {message}");

            RpcSender.SendMessage(player, message, sendTo: player.OwnerId);
        }

        public static void StartCommand(PlayerControl player, string[] args)
        {
            int seconds = 5;

            if (args.Length > 0 && int.TryParse(args[0], out int value))
            {
                seconds = value;
            }

            seconds = Mathf.Clamp(seconds, 1, 60);

            if (GameStates.IsStarting)
            {
                RpcSender.SendMessage(player, "Game is already starting!", sendTo: player.OwnerId);
                return;
            }

            GameStartManager.Instance.startState = GameStartManager.StartingStates.Countdown;
            GameStartManager.Instance.countDownTimer = seconds;
            GameStartManager.Instance.StartButton.gameObject.SetActive(false);
        }

        public static void IdCommand(PlayerControl player)
        {
            string message = "Player Ids";

            foreach (PlayerControl pc in PlayerControl.AllPlayerControls)
            {
                message += $"\n<b>{pc.Data.PlayerName} - {pc.PlayerId}</b>";
            }

            RpcSender.SendMessage(player, message, sendTo: player.OwnerId);
        }

        public static void BanCommand(PlayerControl player, string[] args)
        {
            if (args.Length == 0 || !int.TryParse(args[0], out int id))
            {
                RpcSender.SendMessage(player, "Usage: /cmd ban <id>", sendTo: player.OwnerId);
                return;
            }            

            PlayerControl target = Utils.GetPlayerById(id);

            if (target == null)
            {
                RpcSender.SendMessage(player, "Player not found.", sendTo: player.OwnerId);
                return;
            }

            if (target == player)
            {
                RpcSender.SendMessage(player, "You cannot ban yourself!", sendTo: player.OwnerId);
                return;
            }

            AmongUsClient.Instance.KickPlayer(target.OwnerId, true);

            RpcSender.SendMessage(player, $"{target.Data.PlayerName} has been banned");
        }
    }
}

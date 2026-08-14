using InnerNet;
using JetBrains.Annotations;
using Nebula.Networking;
using UnityEngine;
using Random = System.Random;

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
            InGame,
            InMeeting
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
                case UsageTimes.InGame  when !GameStates.GameStarted || GameStates.IsStarting:
                case UsageTimes.InMeeting when !GameStates.IsInMeeting || GameStates.IsInLobby:
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
                RpcSender.SendMessage(
                    "Unknown command.\nTry using /cmd help",
                    sendTo: player.OwnerId);
                return;
            }

            if (!Command.CanUseCommand(player, cmd))
            {
                RpcSender.SendMessage(
                    "You cannot use this command!",
                    sendTo: player.OwnerId);
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
                case "tpout":
                    TPOutCommand(player);
                    break;
                case "tpin":
                    TPInCommand(player);
                    break;
                case "hwhisper":
                    HWhisper(player,args);
                    break;
                case "8ball":
                    EightBallCommand(player, args);
                    break;
                case "pi":
                    PlayerInformationCommand(player, args);
                    break;
            }
        }
        public static void LoadCommands()
        {
            AllCommands.Add(new Command("help", Command.UsageLevels.Everyone, Command.UsageTimes.Always));
            AllCommands.Add(new Command("start", Command.UsageLevels.Host, Command.UsageTimes.InLobby));
            AllCommands.Add(new Command("id", Command.UsageLevels.Everyone, Command.UsageTimes.Always));
            AllCommands.Add(new Command("ban", Command.UsageLevels.Host, Command.UsageTimes.Always));
            AllCommands.Add(new Command("tpout", Command.UsageLevels.Everyone, Command.UsageTimes.InLobby));
            AllCommands.Add(new Command("tpin", Command.UsageLevels.Everyone, Command.UsageTimes.InLobby));
            AllCommands.Add(new Command("hwhisper", Command.UsageLevels.Everyone, Command.UsageTimes.InLobby));
            AllCommands.Add(new Command("8ball", Command.UsageLevels.Everyone, Command.UsageTimes.InLobby));
            AllCommands.Add(new Command("pi", Command.UsageLevels.Everyone, Command.UsageTimes.Always));
        }

        public static bool OnReceiveChat(PlayerControl player, string text)
        {
            if (text.StartsWith("/cmd", StringComparison.OrdinalIgnoreCase))
            {
                Utils.CheckServerCommand(ref text);
                text = text[1..];

                string[] split = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                if (split.Length == 0)
                {
                    RpcSender.SendMessage("Usage: /cmd <command>\nTry using /cmd help",sendTo: player.OwnerId);

                    return false;
                }
                string command = split[0].ToLowerInvariant();
                string[] args = split.Skip(1).ToArray();

                HandleCommand(player, command, args);

                return false;
            }
            return true;
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

            RpcSender.SendMessage(message, sendTo: player.OwnerId);
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
                RpcSender.SendMessage("Game is already starting!", sendTo: player.OwnerId);
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
                message += $"\n<b>{pc.Data.PlayerName} - ID[{pc.PlayerId}]</b>";
            }

            RpcSender.SendMessage(message, sendTo: player.OwnerId);
        }

        public static void BanCommand(PlayerControl player, string[] args)
        {
            if (args.Length == 0 || !int.TryParse(args[0], out int id))
            {
                RpcSender.SendMessage("Usage: /cmd ban (id)", sendTo: player.OwnerId);
                return;
            }            

            PlayerControl target = Utils.GetPlayerById(id);

            if (target == null)
            {
                RpcSender.SendMessage("Player not found.", sendTo: player.OwnerId);
                return;
            }

            if (target == player)
            {
                RpcSender.SendMessage("You cannot ban yourself!", sendTo: player.OwnerId);
                return;
            }

            AmongUsClient.Instance.KickPlayer(target.OwnerId, true);

            RpcSender.SendMessage($"{target.Data.PlayerName} has been banned");
        }
        public static void TPOutCommand(PlayerControl player)
        {
            player.Teleport(new Vector2(0.1f, 3.8f));
        }

        public static void TPInCommand(PlayerControl player)
        {
            player.Teleport(new Vector2(-0.2f, 1.3f));
        }

        public static void HWhisper(PlayerControl sender,string[] args)
        {
            PlayerControl host = Utils.GetHost();

            if (args.Length == 0)
            {
                RpcSender.SendMessage("Invalid message.\n Please use /cmd hwhisper (sentence).",sendTo: sender.OwnerId);
                return;
            }

            if (sender == host)
            {
                RpcSender.SendMessage("You can't whisper to yourself!", sendTo: host.OwnerId);
                return;
            }           

            string message = string.Join(" ", args);

            if (message.Length > 100)
            {
                RpcSender.SendMessage("Message too long!", sendTo: sender.OwnerId);
                return;
            }

            RpcSender.SendMessage($"{sender.Data.PlayerName} is saying:\n {message}", sendTo: host.OwnerId);
        }

        public static void EightBallCommand(PlayerControl player, string[] args)
        {
            if (args.Length == 0)
            {
                RpcSender.SendMessage("Invalid message.\n Please use /cmd 8ball (sentence).", sendTo: player.OwnerId);
                return;
            }
            string message = string.Join(" ", args);

            if (message.Length > 100)
            {
                RpcSender.SendMessage("Message too long!", sendTo: player.OwnerId);
                return;
            }

            Random random = new();

            int result = random.Next(1, 6);       
            
            switch(result)
            {
                case 1:
                    RpcSender.SendMessage($"{player.Data.PlayerName} asked.....\"{message}\" \n" +
                        $"The answer is yes");
                    break;
                case 2:
                    RpcSender.SendMessage($"{player.Data.PlayerName} asked.....\"{message}\" \n" +
                      $"The answer is no");
                    break;
                case 3:
                    RpcSender.SendMessage($"{player.Data.PlayerName} asked.....\"{message}\" \n" +
                      $"The answer is maybe");
                    break;
                case 4:
                    RpcSender.SendMessage($"{player.Data.PlayerName} asked.....\"{message}\" \n" +
                      $"The answer is maybe not");
                    break;
                case 5:
                    RpcSender.SendMessage($"{player.Data.PlayerName} asked.....\"{message}\" \n" +
                      $"The answer is I have absolutely no clue");
                    break;

                default:
                    RpcSender.SendMessage($"{player.Data.PlayerName} asked.....\"{message}\" \n" +
                     $"The answer is I have absolutely no clue");
                    break;
            }        
                      
        }
        public static void PlayerInformationCommand(PlayerControl player, string[] args)
        {
            if (args.Length == 0 || !int.TryParse(args[0], out int id))
            {
                RpcSender.SendMessage("Usage: /cmd pi (id)", sendTo: player.OwnerId);
                return;
            }
            PlayerControl target = Utils.GetPlayerById(id);

            if (target == null)
            {
                RpcSender.SendMessage("Player not found.", sendTo: player.OwnerId);
                return;
            }

            ClientData clientData = target.GetClientData();

            RpcSender.SendMessage($"<size=120%><b>{target.Data.PlayerName} Info</b></size>\n" +
                $"<b>OS: {clientData.PlatformData.Platform}</b>\n" +
                $"<b>ID: {target.Data.PlayerId}</b>\n" +
                $"<b>Friend-Code: {target.Data.FriendCode}</b>\n" +
                $"<b>Level: {target.Data.PlayerLevel}</b>", sendTo: player.OwnerId);
        }
    }
}

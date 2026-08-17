using InnerNet;

namespace Nebula.Modules
{
    public static class Utils
    {
        public static string GetRegionName(IRegionInfo region = null, bool ignoreNetworkMode = false) //Taken from Ehr
        {
            try
            {
                region ??= ServerManager.Instance.CurrentRegion;

                string name = region.Name;

                if (!ignoreNetworkMode && AmongUsClient.Instance.NetworkMode != NetworkModes.OnlineGame)
                {
                    name = "Local Game";
                    return name;
                }

                if (region.PingServer.EndsWith("among.us", StringComparison.Ordinal))
                {
                    // Official server
                    name = name switch
                    {
                        "North America" => "NA",
                        "Europe" => "EU",
                        "Asia" => "AS",
                        _ => name
                    };

                    return name;
                }

                string ip = region.Servers.FirstOrDefault()?.Ip ?? string.Empty;

                if (ip.Contains("aumods.org", StringComparison.Ordinal) || ip.Contains("duikbo.at", StringComparison.Ordinal))
                {
                    // Official Modded Server
                    if (ip.Contains("au-eu"))
                        name = "MEU";
                    else if (ip.Contains("au-as"))
                        name = "MAS";
                    else
                        name = "MNA";

                    return name;
                }

                if (name.Contains("Niko", StringComparison.OrdinalIgnoreCase))
                    name = name.Replace("233(", "-").Replace("233 (", "-").TrimEnd(')');

                return name;
            }
            catch
            {
                try { return (region ?? ServerManager.Instance.CurrentRegion).Name; }
                catch { return string.Empty;}               
            }
        }

        public static PlayerControl GetPlayerById(int playerId)
        {
            try
            {
                if (PlayerControl.LocalPlayer.PlayerId == playerId)
                    return PlayerControl.LocalPlayer;

                byte id = (byte)playerId;

                foreach (var pc in PlayerControl.AllPlayerControls)
                {
                    if (pc.PlayerId == id)
                        return pc;
                }

                return null;
            }
            catch (Exception e)
            {
                Main.Logger.LogError(e);
                return null;
                 
            }
        }

        public static PlayerControl GetPlayerByClientId(int clientId)
        {
            foreach (PlayerControl pc in PlayerControl.AllPlayerControls)
            {
                if (pc.OwnerId == clientId)
                    return pc;
            }

            return null;
        }

        public static PlayerControl GetHost()
        {
            foreach (PlayerControl pc in PlayerControl.AllPlayerControls)
            {
                if (pc.OwnerId == AmongUsClient.Instance.HostId)
                {
                    return pc;
                }                
            }
            return null;
        }

        public static void CheckServerCommand(ref string text)
        {
            if (text.StartsWith("/cmd"))
            {
                text = "/" + text[4..].TrimStart();
            }
        }

        public static byte TextToColor(string text)
        {
            text = text.ToLowerInvariant();

            int color = text switch
            {
                "red" or "0" => 0,
                "blue" or "1" => 1,
                "green" or "2" => 2,
                "pink" or "3" => 3,
                "orange" or "4" => 4,
                "yellow" or "5" => 5,
                "black" or "6" => 6,
                "white" or "7" => 7,
                "purple" or "8" => 8,
                "brown" or "9" => 9,
                "cyan" or "10" => 10,
                "lime" or "11" => 11,
                "maroon" or "12" => 12,
                "rose" or "13" => 13,
                "banana" or "14" => 14,
                "gray" or "15" => 15,
                "tan" or "16" => 16,
                "coral" or "17" => 17,
                _ => -1
            };

            return color < 0 ? byte.MaxValue : (byte)color;
        }
    }
}

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
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nebula.Modules
{
    internal static class ExtendedPlayerControl
    {
        public static bool IsHost(this PlayerControl player)
        {
            return player != null &&
          AmongUsClient.Instance != null &&
          player.OwnerId == AmongUsClient.Instance.HostId;
        }
    }
}

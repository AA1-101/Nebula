using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nebula.Modules
{
    public static class ExtendedPlayerControl
    {
        extension(PlayerControl player)
        {
            public bool IsHost()
            {
                return player != null
                    && AmongUsClient.Instance != null
                    && player.OwnerId == AmongUsClient.Instance.HostId;
            }          


        }
    }
}

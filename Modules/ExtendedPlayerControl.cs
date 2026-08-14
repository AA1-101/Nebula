using UnityEngine;

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

            public void Teleport(Vector2 location)
            {
                if (player == null)
                    return;

                CustomNetworkTransform nt = player.GetComponent<CustomNetworkTransform>();

                if (nt == null)
                    return;

                nt.RpcSnapTo(location);
            }
        }
    }
}
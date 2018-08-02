using Landfall.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace TABHax.Game
{
    public static class PhotonServerExtensions
    {
        public static GLDCILHFINE[] GetPhotonPlayers(this PhotonServerHandler server)
        {
            return server.JLIIGCJEPPJ;
        }
        public static GLDCILHFINE GetPhotonLocalPlayer(this PhotonServerHandler server)
        {
            return server.MLNNNKNLMCJ;
        }
        public static void KillAll(this PhotonServerHandler server)
        {
            var players = GetPhotonPlayers(server);
            var localPlayer = GetPhotonLocalPlayer(server);
            foreach(var p in players)
            {
                p.PMNJHACFFLB(1);
                if (p == localPlayer)
                    continue;
                var nPlayer = p.GetField<GameObject>("IGBBGPEGMJG");
                if (nPlayer)
                    nPlayer.GetComponent<Player>().Kill();
            }
        }
        public static Landfall.Network.NetworkPlayer[] GetAllPlayers(this PhotonServerHandler server)
        {
            var players = server.JLIIGCJEPPJ;
            var rPlayers = new Landfall.Network.NetworkPlayer[players.Length];
            for (int i = 0; i < players.Length; i++)
                rPlayers[i] = players[i].CBCAFEODHPM;

            return rPlayers;
        }
    }
}

using GameServer.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Connection
{
    public interface ICommunicate
    {
        int Port { get; set; }

        List<string> ConnectedDevices { get; set; }

        void Send(GameRequest requestToSend);
        //string Receive();
        //void CheckConnections();
        //void RemoveConnectedDevice(string ip);
    }
}

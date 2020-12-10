using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Connection
{
    interface ICommunicate
    {
        int Port { get; set; }

        List<string> ConnectedDevices { get; set; }

        void Send();
        string Receive();
        void CheckConnections();
        void RemoveConnectedDevice(string ip);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Connection
{
    class SocketHandler : ICommunicate
    {
        public int Port { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public List<string> ConnectedDevices { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void CheckConnections()
        {
            throw new NotImplementedException();
        }

        public string Receive()
        {
            throw new NotImplementedException();
        }

        public void RemoveConnectedDevice(string ip)
        {
            throw new NotImplementedException();
        }

        public void Send()
        {
            throw new NotImplementedException();
        }
    }
}

using GameServer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Connection
{
    class ConnectionRequest : Request
    {
        private string username;
        public string Username
        {
            get { return username; }
            set { username = value; }
        }
    }
}

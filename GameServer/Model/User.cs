using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Model
{
    public class User
    {
        private byte point;

        public byte Point
        {
            get { return point; }
            set { point = value; }
        }

        private byte requestType;
        public byte RequestType
        {
            get { return requestType; }
            set { requestType = value; }
        }

        private string name;
        public string Name
        {
            get { return name; }
        }

        private string email;
        public string Email
        {
            get { return email; }
            set { email = value; }
        }

        private int wins;
        public int Wins
        {
            get { return wins; }
            set { wins = value; }
        }

        private string ip;
        public string IP
        {
            get { return ip; }
        }

        public TcpClient Client;

        public User(string name, string email, int wins, string ip, TcpClient client)
        {
            this.name = name;
            Email = email;
            Wins = wins;
            this.ip = ip;
            Client = client;
        }
    }
}

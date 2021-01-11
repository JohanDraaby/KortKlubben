using GameServer.Game;
using GameServer.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GameServer.Connection
{
    class SocketHandler : ICommunicate
    {
        //private object _lock;
        readonly object _lock = new object();

        private Dictionary<int, TcpClient> tcpClients;

        public Dictionary<int, TcpClient> TcpClients
        {
            get { return tcpClients; }
            set { tcpClients = value; }
        }

        private List<User> clientList;

        public List<User> ClientList
        {
            get { return clientList; }
            set { clientList = value; }
        }

        private ObservableCollection<GameRequest> listOfGameRequests = new ObservableCollection<GameRequest>();

        public ObservableCollection<GameRequest> ListOfGameRequests
        {
            get { return listOfGameRequests; }
            set { listOfGameRequests = value; }
        }

        private IConvertable messageConverter;

        public IConvertable MessageConverter
        {
            get { return messageConverter; }
            set { messageConverter = value; }
        }

        public SocketHandler(IConvertable converter)
        {
            MessageConverter = converter;
            clientList = new List<User>();
        }


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

        public void Send(GameRequest requestToSend)
        {

            string s;
            byte[] buffer;

            s = JsonConvert.SerializeObject(requestToSend, Formatting.Indented);
            buffer = Encoding.UTF8.GetBytes(s);

            if (requestToSend.RequestType == 2)
            {
                Console.WriteLine("This is the server giving a card to a player\n" + s);
            }
            if (requestToSend.RequestType == 3)
            {
                Console.WriteLine("Asked player for cards\n" + s);
            }

            // Find client
            NetworkStream stream;

            lock (_lock)
            {
                foreach (User c in ClientList)
                {
                    if (c.Name == requestToSend.UserTo)
                    {
                        stream = c.Client.GetStream();
                        stream.Write(buffer, 0, buffer.Length);
                        break;
                    }
                }
            }
            // Send to client
        }

        public void HandleConnections()
        {
            tcpClients = new Dictionary<int, TcpClient>();

            int count = 1;

            TcpListener ServerSocket = new TcpListener(IPAddress.Any, 5000);

            ServerSocket.Start();

            while (true)
            {
                TcpClient client = ServerSocket.AcceptTcpClient();
                lock (_lock) tcpClients.Add(count, client);
                Console.WriteLine("Someone connected!!");

                Thread t = new Thread(() => InitUserSession(client));
                t.Start();
                count++;
            }
        }

        public Dictionary<int, TcpClient> GetTcpClients()
        {
            return TcpClients;
        }

        public List<User> GetClients()
        {
            return ClientList;
        }

        public ObservableCollection<GameRequest> GetGameRequests()
        {
            ObservableCollection<GameRequest> list = ListOfGameRequests;

            ListOfGameRequests.Clear();

            return list;
        }

        private void InitUserSession(TcpClient client)
        {
            // User can do stuff in their own session.
            NetworkStream stream;
            byte[] buffer;
            int byte_count;

            string data;
            Request converted;

            while (true)
            {
                stream = client.GetStream();
                buffer = new byte[1024];
                byte_count = stream.Read(buffer, 0, buffer.Length);

                data = Encoding.UTF8.GetString(buffer, 0, byte_count);


                if (data.Contains("Username"))
                {
                    Console.WriteLine("Userdata:");
                    Console.WriteLine(data);
                    converted = JsonConvert.DeserializeObject<ConnectionRequest>(data);
                    ConnectionRequestHandler((ConnectionRequest)converted, client);
                }
                else
                {
                    Console.WriteLine("\n\nRequestData (Player)");
                    Console.WriteLine(data + "\n\n");
                    // This is bad, but it works for now.
                    converted = JsonConvert.DeserializeObject<GameRequest>(data);

                    if (converted is GameRequest)
                    {
                        // Add to collection of gameRequests
                        ListOfGameRequests.Add((GameRequest)converted);
                    }
                }
            }
        }

        private void ConnectionRequestHandler(ConnectionRequest converted, TcpClient client)
        {
            switch (converted.RequestType)
            {
                case 1:
                    // Connect
                    if (converted is ConnectionRequest)
                    {
                        AddTCPClient(converted.Username, client);
                    }
                    break;
                case 2:
                    // DisConnect
                    break;
                case 3:
                    // ReConnect
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Adds a connected TCPClient to list of connected TCPClients
        /// </summary>
        /// <param name="user"></param>
        /// <param name="client"></param>
        void AddTCPClient(string user, TcpClient client)
        {
            ClientList.Add(new User(user, "dsa", 0, "dsa", client));
        }
    }
}

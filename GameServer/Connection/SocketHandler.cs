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
        
        public int Port { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public List<string> ConnectedDevices { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public SocketHandler(IConvertable converter)
        {
            MessageConverter = converter;
            clientList = new List<User>();
        }


        /// <summary>
        /// Check already established connections
        /// </summary>
        public void CheckConnections()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Receive data from client
        /// </summary>
        public string Receive()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Remove the connected client from ConnectedDevices
        /// </summary>
        /// <param name="ip"></param>
        public void RemoveConnectedDevice(string ip)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Send data to a client
        /// </summary>
        /// <param name="requestToSend"></param>
        public void Send(GameRequest requestToSend)
        {

            string s;
            byte[] buffer;

            s = JsonConvert.SerializeObject(requestToSend, Formatting.Indented);
            buffer = Encoding.UTF8.GetBytes(s);

            // Write to console what action the server is doing based on RequestType
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

            // Send to client
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
        }

        /// <summary>
        /// Handle connected client's connection
        /// </summary>
        /// <param name="user"></param>
        /// <param name="client"></param>
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

        // Outdated
        /// <summary>
        /// Return list of connected TCP clients
        /// </summary>
        public Dictionary<int, TcpClient> GetTcpClients()
        {
            return TcpClients;
        }

        /// <summary>
        /// Return list of connected clients
        /// </summary>
        public List<User> GetClients()
        {
            return ClientList;
        }

        // Outdated
        /// <summary>
        /// Return collection of <see cref="GameRequest"/>s
        /// </summary>
        public ObservableCollection<GameRequest> GetGameRequests()
        {
            ObservableCollection<GameRequest> list = ListOfGameRequests;

            ListOfGameRequests.Clear();

            return list;
        }

        /// <summary>
        /// Initializes <see cref="User"/>'s session after they've connected
        /// </summary>
        /// <param name="client"></param>
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

        /// <summary>
        /// Handles <see cref="ConnectionRequest"/>s from a client
        /// </summary>
        /// <param name="converted"></param>
        /// <param name="client"></param>
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
                    // Disconnect
                    break;
                case 3:
                    // Reconnect
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
            Console.WriteLine("=============================");
            Console.WriteLine("Added: " + user + " to tcpclients!");
            Console.WriteLine("=============================");
        }
    }
}

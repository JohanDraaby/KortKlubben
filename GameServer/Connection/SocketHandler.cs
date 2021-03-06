﻿using GameServer.Game;
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

        /// <summary>
        /// Contructor for <see cref="SocketHandler"/>
        /// </summary>
        /// <param name="converter"></param>
        public SocketHandler(IConvertable converter)
        {
            MessageConverter = converter;
            clientList = new List<User>();
        }

        /// <summary>
        /// Check already established connections
        /// </summary>
        //public void CheckConnections()
        //{
        //    throw new NotImplementedException();
        //}

        /// <summary>
        /// Receive data from client
        /// </summary>
        //public string Receive()
        //{
        //    throw new NotImplementedException();
        //}

        /// <summary>
        /// Remove the connected client from ConnectedDevices
        /// </summary>
        /// <param name="ip"></param>
        //public void RemoveConnectedDevice(string ip)
        //{
        //    throw new NotImplementedException();
        //}

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

            // Sending Buffer
            SendStreamToUser(requestToSend, buffer);
        }

        /// <summary>
        /// Sends <see cref="NetworkStream"/> To <see cref="User"/>
        /// </summary>
        /// <param name="requestToSend"></param>
        /// <param name="buffer"></param>
        private void SendStreamToUser(GameRequest requestToSend, byte[] buffer)
        {
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

                Thread t = new Thread(() => InitUserSession(client));
                t.Start();
                count++;
            }
        }

        /// <summary>
        /// Return list of connected clients
        /// </summary>
        public List<User> GetClients()
        {
            return ClientList;
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

            while (true)
            {
                stream = client.GetStream();
                buffer = new byte[1024];
                byte_count = stream.Read(buffer, 0, buffer.Length);

                data = Encoding.UTF8.GetString(buffer, 0, byte_count);
                // Handeling request types
                HandleRequestType(client, data);
            }
        }

        /// <summary>
        /// Converts the <see cref="Request"/> to the proper type of <see cref="Request"/>
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data"></param>
        private void HandleRequestType(TcpClient client, string data)
        {
            Request converted;
            if (data.Contains("Username"))
            {
                converted = JsonConvert.DeserializeObject<ConnectionRequest>(data);
                ConnectionRequestHandler((ConnectionRequest)converted, client);
            }
            else
            {
                converted = JsonConvert.DeserializeObject<GameRequest>(data);

                if (converted is GameRequest)
                {
                    // Add to collection of gameRequests
                    ListOfGameRequests.Add((GameRequest)converted);
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
            ClientList.Add(new User(converted.Username, "(Isert Ip)", 0, "(Insert Ip)", client));
        }
    }
}

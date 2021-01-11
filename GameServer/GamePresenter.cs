using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GameServer.Connection;
using GameServer.Dal;
using GameServer.Game;
using GameServer.Model;
using Newtonsoft.Json;

namespace GameServer
{
    class GamePresenter
    {
        static private List<Card> deck;
        static public List<Card> Deck
        {
            get { return deck; }
            set { deck = value; }
        }

        private ICommunicate socketHandler;

        public ICommunicate SocketHandler
        {
            get { return socketHandler; }
            set { socketHandler = value; }
        }


        static Random rng = new Random();

        static bool gameIsStarted = false;

        DataManager dm = new DataManager();

        private GameController gameController;
        public GameController GameController
        {
            get { return gameController; }
            set { gameController = value; }
        }

        private List<User> onlineUsers;
        public List<User> OnlineUsers
        {
            get { return onlineUsers; }
            set { onlineUsers = value; }
        }

        public GamePresenter()
        {
            SocketHandler = new SocketHandler(new Game.JsonConverter());
            // GameController = new GoFishController();
            // GameController.NewGame(OnlineUsers);

            StartCommunication();
        }

        readonly object _lock = new object();
        Dictionary<int, TcpClient> tcpClients = new Dictionary<int, TcpClient>();
        static List<User> clientList;

        /// <summary>
        /// Connects <see cref="TcpClient"/>s to the socket.
        /// </summary>
        void StartCommunication()
        {
            SocketHandler handler = (SocketHandler)SocketHandler;
            clientList = new List<User>();
            GoFishController gfc = new GoFishController(SocketHandler);

            // Start socket connection thread
            lock (_lock)
            {
                Thread connectionThread = new Thread(handler.HandleConnections);
                connectionThread.Start();
                while (true)
                {
                    Thread.Sleep(1000);

                    if (handler.GetClients().Count >= 2)
                    {
                        clientList = handler.GetClients();
                        gfc.NewGame(clientList);

                        gfc.SetStartPlayer(gfc.Games.Last());
                        break;
                    }
                }
            }
        }
    }
}

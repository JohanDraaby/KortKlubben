using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GameServer.Dal;
using GameServer.Game;
using GameServer.Model;
using Newtonsoft.Json;

namespace GameServer
{
    class RequestManager
    {
        static private List<Card> deck;
        static public List<Card> Deck
        {
            get { return deck; }
            set { deck = value; }
        }

        static Random rng = new Random();

        static void Shuffle()
        {
            int n = Deck.Count;
            for (int i = 0; i < 10; i++)
            {
                while (n > 1)
                {
                    n--;
                    int k = rng.Next(n + 1);
                    Card c = Deck[k];
                    Deck[k] = Deck[n];
                    Deck[n] = c;
                }
            }
        }

        /// <summary>
        /// Deals 7 <see cref="Card"/>s to each player in the game, at the start of the game.
        /// </summary>
        static void Deal()
        {
            string s;
            byte[] buffer;

            Deck = CardFactory.CreateDeck();

            for (int i = 0; i < tcpClients.Count; i++)
            {
                List<Card> hand = new List<Card>();
                for (int j = 0; j < 7; j++)
                {
                    hand.Add(Deck[Deck.Count - 1]);
                    Deck.RemoveAt(Deck.Count - 1);
                }

                GameRequest dealCard = new GameRequest();
                dealCard.Cardlist = hand;
                dealCard.RequestType = 2;

                s = JsonConvert.SerializeObject(dealCard, Formatting.Indented);
                buffer = Encoding.UTF8.GetBytes(s);

                SendRequest(tcpClients[i].Name, buffer);
            }
        }

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

        public RequestManager()
        {
            // GameController = new GoFishController();
            // GameController.NewGame(OnlineUsers);

            StartCommunication();
        }

        static readonly object _lock = new object();
        static readonly Dictionary<int, TcpClient> list_clients = new Dictionary<int, TcpClient>();
        static List<User> tcpClients;

        /// <summary>
        /// Connects <see cref="TcpClient"/>s to the socket.
        /// </summary>
        void StartCommunication()
        {
            tcpClients = new List<User>();

            int count = 1;

            TcpListener ServerSocket = new TcpListener(IPAddress.Any, 5000);

            ServerSocket.Start();

            while (true)
            {
                TcpClient client = ServerSocket.AcceptTcpClient();
                lock (_lock) list_clients.Add(count, client);
                Console.WriteLine("Someone connected!!");

                Thread t = new Thread(SetUpGame);
                t.Start(count);
                count++;
            }
        }

        /// <summary>
        /// Sets up the game, waiting for users to join.
        /// </summary>
        /// <param name="o"></param>
        public static void SetUpGame(object o)
        {
            int id = (int)o; // Gives the client a unique ID for the game.
            TcpClient client;
            string data = "";
            GameRequest converted;

            NetworkStream stream;
            byte[] buffer = new byte[1024];
            int byte_count;

            lock (_lock) client = list_clients[id];

            while (true)
            {
                stream = client.GetStream();
                buffer = new byte[1024];
                byte_count = stream.Read(buffer, 0, buffer.Length);

                if (byte_count == 0)
                {
                    break;
                }

                data = Encoding.UTF8.GetString(buffer, 0, byte_count);
                converted = JsonConvert.DeserializeObject<GameRequest>(data);

                switch (converted.RequestType)
                {
                    case 1:
                        AddTCPClient(converted.UserFrom, client); // Add TCP Client
                        break;
                }

                // Waiting for to players to be in game
                while (tcpClients.Count != 2) { } break;
            }

            // Starts game
            StartGame(client, byte_count);
        }

        static void StartGame(TcpClient client, int byte_count)
        {
            NetworkStream stream;
            byte[] buffer;

            string data;
            GameRequest converted;

            while (true)
            {
                if (!gameIsStarted)
                {
                    gameIsStarted = true;
                    Deal();
                    Console.WriteLine("Cards Dealt");
                }

                stream = client.GetStream();
                buffer = new byte[1024];
                byte_count = stream.Read(buffer, 0, buffer.Length);

                data = Encoding.UTF8.GetString(buffer, 0, byte_count);
                converted = JsonConvert.DeserializeObject<GameRequest>(data);

                Console.WriteLine("Recived game request " + converted.RequestType);

                switch (converted.RequestType)
                {
                    case 3:
                        Console.WriteLine("CASE 3");
                        ReceiveCardRequest(data); // Request Card
                        break;
                    case 4:
                        Console.WriteLine("CASE 4");
                        ReceiveGiveCardRequest(data);
                        break;
                }
            }
        }

        /// <summary>
        /// When an oppnent returns a card request
        /// </summary>
        /// <param name="data"></param>
        private static void ReceiveGiveCardRequest(string data)
        {
            GameRequest validateData = JsonConvert.DeserializeObject<GameRequest>(data);

            GoFishRequest(validateData);

            validateData.RequestType = 2;
            data = JsonConvert.SerializeObject(validateData, Formatting.None);

            byte[] buffer = Encoding.UTF8.GetBytes(data + Environment.NewLine);

            SendRequest(validateData.UserTo, buffer);
        }

        /// <summary>
        /// When a player request a card from an opponent
        /// </summary>
        /// <param name="data"></param>
        private static void ReceiveCardRequest(string data)
        {
            GameRequest validateData = ForwardReceiveCardRequest(data);

            validateData.RequestType = 2;
            data = JsonConvert.SerializeObject(validateData, Formatting.None);
            byte[] buffer = Encoding.UTF8.GetBytes(data + Environment.NewLine);
            SendRequest(validateData.UserTo, buffer);
        }

        /// <summary>
        /// Sends a rquest to a player
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static GameRequest ForwardReceiveCardRequest(string data)
        {
            GameRequest validateData = JsonConvert.DeserializeObject<GameRequest>(data);

            byte[] buffer = Encoding.UTF8.GetBytes(data + Environment.NewLine);

            SendRequest(validateData.UserTo, buffer);

            return validateData;
        }

        /// <summary>
        /// Addsp a connected TCPClient to list of connected TCPClients
        /// </summary>
        /// <param name="user"></param>
        /// <param name="client"></param>
        static void AddTCPClient(string user, TcpClient client)
        {
            tcpClients.Add(new User(user, "dsa", 0, "dsa", client));
        }

        /// <summary>
        /// Sends a request if the user is a part of our connected users.
        /// </summary>
        /// <param name="nameToValidate"></param>
        /// <param name="buffer"></param>
        private static void SendRequest(string nameToValidate, byte[] buffer)
        {
            NetworkStream stream;

            lock (_lock)
            {
                foreach (User c in tcpClients)
                {
                    if (c.Name == nameToValidate)
                    {
                        stream = c.Client.GetStream();
                        stream.Write(buffer, 0, buffer.Length);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// If the <see cref="GameRequest.Cardlist"/> is empty, add top card from the <see cref="Deck"/> if there are any.
        /// </summary>
        /// <param name="validateData"></param>
        private static void GoFishRequest(GameRequest validateData)
        {
            Console.WriteLine("Fishing Card From Deck");
            if (validateData.Cardlist.Count == 0 && Deck.Count > 0)
            {
                validateData.Cardlist.Add(Deck[0]);
                Deck.RemoveAt(0);
            }
        }
    }
}

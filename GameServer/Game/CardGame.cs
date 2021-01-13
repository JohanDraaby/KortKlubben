using GameServer.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GameServer.Game
{
    public class CardGame
    {
        private List<User> listOfUsers;
        public List<User> ListOfUsers
        {
            get { return listOfUsers; }
            set { listOfUsers = value; }
        }

        private List<byte> listOfUserPoints = new List<byte>();

        public List<byte> ListOfUserPoints
        {
            get { return listOfUserPoints; }
            set { listOfUserPoints = value; }
        }



        private List<Card> deck;
        public List<Card> Deck
        {
            get { return deck; }
            set { deck = value; }
        }

        private string activePlayer;

        public string ActivePlayer
        {
            get { return activePlayer; }
            set { activePlayer = value; }
        }

        private int scoresReceived;

        public int ScoresReceived
        {
            get { return scoresReceived; }
            set { scoresReceived = value; }
        }


        readonly object _lock = new object();

        Random rng = new Random();

        /// <summary>
        /// Constructor for the <see cref="CardGame"/> class.
        /// It will be constructed with a deck of cards, 
        /// and the cards will be <see cref="Shuffle"/>d 
        /// and the cards will be <see cref="Deal"/>t.
        /// </summary>
        public CardGame(List<User> listOfPlayers)
        {
            ListOfUsers = listOfPlayers;
            Deck = CardFactory.CreateDeck();
            Shuffle();
            Deal();
            ActivePlayer = ListOfUsers.First().Name;

            List<string> listOfUsernames = new List<string>();

            for (int i = 0; i < ListOfUsers.Count; i++)
            {
                listOfUsernames.Add(ListOfUsers[i].Name);
                listOfUserPoints.Add(0);
            }

            Thread.Sleep(1000);
            string s;
            byte[] buffer;
            NetworkStream stream;

            SetupRequest setupRequest = new SetupRequest();
            setupRequest.RequestType = 4;
            setupRequest.Players = listOfUsernames;

            s = JsonConvert.SerializeObject(setupRequest, Formatting.Indented);
            buffer = Encoding.UTF8.GetBytes(s);

            lock (_lock)
            {
                foreach (User player in ListOfUsers)
                {
                    stream = player.Client.GetStream();
                    stream.Write(buffer, 0, buffer.Length);
                }
            }
        }

        /// <summary>
        /// Reorganize the deck of <see cref="Card"/>s to simulate shuffling the cards.
        /// </summary>
        void Shuffle()
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

        // Outdated
        /// <summary>
        /// Deals 7 <see cref="Card"/>s to each player in the game, at the start of the game.
        /// </summary>
        //void Deal()
        //{
        //    for (int i = 0; i < listOfUsers.Count; i++)
        //    {
        //        List<Card> hand = new List<Card>();
        //        for (int j = 0; j < 7; j++)
        //        {
        //            hand.Add(Deck[Deck.Count - 1]);
        //            Deck.RemoveAt(Deck.Count - 1);
        //        }
        //        SendCards(listOfUsers[i], hand);
        //    }
        //}

        /// <summary>
        /// Deals 7 <see cref="Card"/>s to each player in the game, at the start of the game.
        /// </summary>
        void Deal()
        {
            string s;
            byte[] buffer;

            for (int i = 0; i < ListOfUsers.Count; i++)
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
                dealCard.UserFrom = "Dealer";

                s = JsonConvert.SerializeObject(dealCard, Formatting.Indented);
                buffer = Encoding.UTF8.GetBytes(s);

                SendRequest(ListOfUsers[i].Name, buffer);
            }
        }

        /// <summary>
        /// Sends 7 <see cref="Card"/>s to a player. This is the players hand.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="cardList"></param>
        void SendCards(User player, List<Card> cardList)
        {

        }

        /// <summary>
        /// Start and run game
        /// </summary>
        /// <param name="client"></param>
        /// <param name="byte_count"></param>
        void StartGame(TcpClient client, int byte_count)
        {
            NetworkStream stream;
            byte[] buffer;

            string data;
            GameRequest converted;

            while (true)
            {
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

        public void EndGame()
        {
            GameRequest endGameRequest = new GameRequest();
            endGameRequest.RequestType = 0;

            string s;
            byte[] buffer;

            s = JsonConvert.SerializeObject(endGameRequest, Formatting.Indented);
            buffer = Encoding.UTF8.GetBytes(s);


            for (int i = 0; i < ListOfUsers.Count; i++)
            {
                endGameRequest.UserTo = ListOfUsers[i].Name;
                SendRequest(endGameRequest.UserTo, buffer);
            }

            Console.WriteLine("==============================");
            Console.WriteLine("=       Ending Game          =");
            Console.WriteLine("==============================");
        }

        /// <summary>
        /// When an oppnent returns a card request
        /// </summary>
        /// <param name="data"></param>
        private void ReceiveGiveCardRequest(string data)
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
        private void ReceiveCardRequest(string data)
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
        private GameRequest ForwardReceiveCardRequest(string data)
        {
            GameRequest validateData = JsonConvert.DeserializeObject<GameRequest>(data);

            byte[] buffer = Encoding.UTF8.GetBytes(data + Environment.NewLine);

            SendRequest(validateData.UserTo, buffer);

            return validateData;
        }

        /// <summary>
        /// Sends a request if the user is a part of our connected users.
        /// </summary>
        /// <param name="nameToValidate"></param>
        /// <param name="buffer"></param>
        private void SendRequest(string nameToValidate, byte[] buffer)
        {
            NetworkStream stream;

            lock (_lock)
            {
                foreach (User c in ListOfUsers)
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
        private void GoFishRequest(GameRequest validateData)
        {
            Console.WriteLine("Fishing Card From Deck");
            if (validateData.Cardlist.Count == 0 && Deck.Count > 0)
            {
                validateData.Cardlist.Add(Deck[0]);
                Deck.RemoveAt(0);
            }
        }

        /// <summary>
        /// Set active player to the next in line
        /// </summary>
        public void SetActivePlayer()
        {
            foreach (User u in ListOfUsers)
            {
                if (u.Name == ActivePlayer)
                {
                    if (ListOfUsers.IndexOf(u) == ListOfUsers.Count - 1)
                    {
                        ActivePlayer = ListOfUsers[0].Name;
                    }
                    else
                    {
                        ActivePlayer = ListOfUsers[ListOfUsers.IndexOf(u) + 1].Name;
                    }
                    break;
                }
            }
        }
    }
}

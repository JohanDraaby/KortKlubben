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

            // Shuffle Deck
            Shuffle();

            // Deals card
            Deal();

            // Set the first player to active player
            ActivePlayer = ListOfUsers.First().Name;

            List<string> listOfUsernames = new List<string>();
            UserSetup(listOfUsernames);
            SendPlayerList(listOfUsernames);
        }

        /// <summary>
        /// Send a list of <see cref="User.Name"/> To all palyers in that session
        /// </summary>
        /// <param name="listOfUsernames"></param>
        private void SendPlayerList(List<string> listOfUsernames)
        {
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
        /// Setting up <see cref="User"/>'s for the game
        /// </summary>
        /// <param name="listOfUsernames"></param>
        private void UserSetup(List<string> listOfUsernames)
        {
            for (int i = 0; i < ListOfUsers.Count; i++)
            {
                listOfUsernames.Add(ListOfUsers[i].Name);
                listOfUserPoints.Add(0);
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
        /// Broadcasting end of game to all players
        /// </summary>
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

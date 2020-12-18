using GameServer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
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

        private List<Card> deck;
        public List<Card> Deck
        {
            get { return deck; }
            set { deck = value; }
        }

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
            //Shuffle();
            //Deal();
        }

        /// <summary>
        /// Reorganizes the deck of <see cref="Card"/>s to simulate a shuffling of the cards.
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
            for (int i = 0; i < listOfUsers.Count; i++)
            {
                List<Card> hand = new List<Card>();
                for (int j = 0; j < 7; j++)
                {
                    hand.Add(Deck[Deck.Count - 1]);
                    Deck.RemoveAt(Deck.Count - 1);
                }
                SendCards(listOfUsers[i], hand);
            }
        }

        /// <summary>
        /// Sends 7 <see cref="Card"/>s to a player. This is the players hand.
        /// </summary>
        void SendCards(User player, List<Card> cardList)
        {

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Game
{
    class CardFactory
    {
        private Card CreateCard(byte value, string suit, string colour)
        {
            return new Card(value, suit, colour);
        }

        public List<Card> CreateDeck()
        {
            List<Card> deck = new List<Card>();

            for (byte b = 1; b < 14; b++)
            {
                deck.Add(new Card(b, "Hjerter", "Rød"));
                deck.Add(new Card(b, "Ruder", "Rød"));
                deck.Add(new Card(b, "Klør", "Sort"));
                deck.Add(new Card(b, "Spar", "Sort"));
            }

            deck.Add(new Card(14, "", "Rød"));
            deck.Add(new Card(14, "", "Rød"));
            deck.Add(new Card(14, "", "Sort"));
            deck.Add(new Card(14, "", "Sort"));

            return deck;
        }
    }
}

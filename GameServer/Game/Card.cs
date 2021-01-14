using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Game
{
    public class Card
    {
        private byte value;
        public byte Value
        {
            get { return value; }
        }

        private string suit;
        public string Suit
        {
            get { return suit; }
            set { suit = value; }
        }

        private string colour;
        public string Colour
        {
            get { return colour; }
        }

        private string fullName;
        public string FullName
        {
            get { return fullName; }
            set { fullName = value; }
        }

        /// <summary>
        /// Initiate An Object Of Type <see cref="Card"/>
        /// </summary>
        public Card(byte value, string suit, string colour)
        {
            this.value = value;
            Suit = suit;
            FullName = suit + " ";
            this.colour = colour;

            switch (value)
            {
                case 1:
                    FullName += "Es";
                    break;
                case 11:
                    FullName += "Knægt";
                    break;
                case 12:
                    FullName += "Dame";
                    break;
                case 13:
                    FullName += "Konge";
                    break;
                case 14:
                    FullName = "Joker";
                    break;
                default:
                    FullName += value;
                    break;
            }
        }
    }
}

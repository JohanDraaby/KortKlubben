using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Game
{
    class Card
    {
        private byte value;
        private string suit;
        private string colour;
        private string fullName;

        /**
         *
         * @return value of {@link Card}
         */
        public int getValue()
        {
            return this.value;
        }

        /**
         *
         * @return suit of {@link Card}
         */
        public string getSuit()
        {
            return this.suit;
        }

        /**
         *
         * @return colour of {@link Card}
         */
        public string getColour()
        {
            return this.colour;
        }

        /**
         *
         * @return fullName of {@link Card}
         */
        public string getFullName()
        {
            return this.fullName;
        }

        /**
         * Sets a new name to the {@link Card}
         * @param newName
         */
        public void setFullName(string newName)
        {
            this.fullName = newName;
        }

        /// <summary>
        /// Used To Initiate An Object Of Type <see cref="Card"/>
        /// </summary>
        public Card(byte value, string suit, string colour)
        {
            this.value = value;
            this.suit = suit;
            this.fullName = suit + " ";
            this.colour = colour;

            switch (value)
            {
                case 1:
                    this.fullName += "Es";
                    break;
                case 11:
                    this.fullName += "Knægt";
                    break;
                case 12:
                    this.fullName += "Dame";
                    break;
                case 13:
                    this.fullName += "Konge";
                    break;
                case 14:
                    this.fullName = "Joker";
                    break;
                default:
                    this.fullName += value;
                    break;


            }
        }
    }
}

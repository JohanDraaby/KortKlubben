using GameServer.Connection;
using GameServer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Game
{
    public abstract class GameController
    {
        private List<CardGame> games;

        public List<CardGame> Games
        {
            get { return games; }
            set { games = value; }
        }

        private ICommunicate commHandler;

        public ICommunicate CommHandler
        {
            get { return commHandler; }
            set { commHandler = value; }
        }

        /// <summary>
        /// Constructor for the <see cref="GameController"/> class.
        /// </summary>
        public GameController(ICommunicate comm)
        {
            CommHandler = comm;

            Games = new List<CardGame>();
        }

        /// <summary>
        /// Instantiates a new <see cref="CardGame"/> and adds it to the list of games.
        /// </summary>
        /// <param name="players">List of players to enter the game</param>
        public void NewGame(List<User> players)
        {
            games.Add(new CardGame(players));
        }

        /// <summary>
        /// Disposes of a <see cref="CardGame"/> once the players are done with it.
        /// </summary>
        /// <param name="gameToDispose"></param>
        public void DisposeGame(CardGame gameToDispose)
        {

        }

        /// <summary>
        /// Sends a <see cref="GameRequest"/> to one of the players.
        /// </summary>
        public void SendRequest()
        {

        }

        /// <summary>
        /// Receives a <see cref="GameRequest"/> from one of the players.
        /// </summary>
        public void ReceiveRequest()
        {

        }
    }
}

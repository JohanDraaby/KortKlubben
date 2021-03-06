﻿using GameServer.Connection;
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
        /// <param name="comm">Object which implements ICommunicate</param>
        public GameController(ICommunicate comm)
        {
            CommHandler = comm;
            Games = new List<CardGame>();
        }

        /// <summary>
        /// Instantiate a new <see cref="CardGame"/> and adds it to the list of games.
        /// </summary>
        /// <param name="players">List of players to enter the game</param>
        public void NewGame(List<User> players)
        {
            games.Add(new CardGame(players));
        }

        /// <summary>
        /// Dispose of a <see cref="CardGame"/> once the players are done with it.
        /// </summary>
        /// <param name="gameToDispose"></param>
        //public void DisposeGame(CardGame gameToDispose)
        //{

        //}
    }
}

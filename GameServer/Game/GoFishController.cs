using GameServer.Connection;
using GameServer.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GameServer.Game
{
    public class GoFishController : GameController
    {
        private List<GameRequest> listOfGameRequests;
        public List<GameRequest> ListOfGameRequests
        {
            get { return listOfGameRequests; }
            set { listOfGameRequests = value; }
        }

        private SocketHandler socketHandler;
        SocketHandler SocketHandler
        {
            get { return socketHandler; }
            set { socketHandler = value; }
        }

        /// <summary>
        /// Contructor of <see cref="GoFishController"/>
        /// </summary>
        /// <param name="comm"></param>
        public GoFishController(ICommunicate comm) : base(comm)
        {
            SocketHandler CommHandler = (SocketHandler)comm;
            SocketHandler = CommHandler;

            // Subscribe to collection
            // Make a collection to observe and add a few Person objects.
            // Wire up the CollectionChanged event.

            //ListOfGameRequests = CommHandler.GetGameRequests();
            ListOfGameRequests = new List<GameRequest>();

            CommHandler.ListOfGameRequests.CollectionChanged += GameRequests_CollectionChanged;

            // Start thread that handles GameRequests
            Thread gameRequestThread = new Thread(HandleGameRequests);
            gameRequestThread.Start();
        }

        /// <summary>
        /// Listen for new <see cref="GameRequest"/>s
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void GameRequests_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                // Added the NEW items that were inserted.
                foreach (GameRequest g in e.NewItems)
                {
                    ListOfGameRequests.Add(g);
                }
            }
        }

        /// <summary>
        /// Remove a <see cref="GameRequest"/> which has been properly handled from collection
        /// </summary>
        /// <param name="gameRequestCompleted"></param>
        void CompleteGameRequest(GameRequest gameRequestCompleted)
        {
            SocketHandler.ListOfGameRequests.Remove(gameRequestCompleted);
            ListOfGameRequests.Remove(gameRequestCompleted);
        }

        /// <summary>
        /// Handles all incoming <see cref="GameRequest"/>s
        /// </summary>
        void HandleGameRequests()
        {
            while (true)
            {
                if (ListOfGameRequests.Count != 0)
                {
                    Console.WriteLine();
                    Console.WriteLine("There are requests waiting to be handled...");
                    Console.WriteLine();

                    for (int i = 0; i < ListOfGameRequests.Count; i++)
                    {
                        HandleRequest(ListOfGameRequests[i]);
                        CompleteGameRequest(ListOfGameRequests[i]);
                    }
                }
            }
        }

        /// <summary>
        /// Handle all incoming <see cref="Request"/>s
        /// </summary>
        /// <param name="gameRequestToHandle"></param>
        void HandleRequest(GameRequest gameRequestToHandle)
        {
            Console.WriteLine();
            Console.WriteLine("==================");
            Console.WriteLine("Handling request type " + gameRequestToHandle.RequestType + " sent from " + gameRequestToHandle.UserFrom);
            Console.WriteLine("==================");
            Console.WriteLine();

            switch (gameRequestToHandle.RequestType)
            {
                // ForwardRequest
                case 1:
                    CardGame tempGame = GetCardGame(gameRequestToHandle.UserFrom);

                    if (gameRequestToHandle.UserFrom == tempGame.ActivePlayer)
                    {
                        gameRequestToHandle.RequestType = 3;
                        SocketHandler.Send(gameRequestToHandle);
                    }
                    else
                    {
                        gameRequestToHandle.UserTo = tempGame.ActivePlayer;
                        SocketHandler.Send(gameRequestToHandle);
                    }
                    break;

                // SendPlayerCards
                case 3:
                    // Fish From Deck
                    FindCard(gameRequestToHandle);

                    // Sending Gamerequest
                    gameRequestToHandle.RequestType = 2;
                    SocketHandler.Send(gameRequestToHandle);

                    tempGame = GetCardGame(gameRequestToHandle.UserFrom);

                    // Condition to end game
                    if (tempGame.Deck.Count == 0)
                    {
                        tempGame.EndGame();
                        break;
                    }

                    Console.WriteLine("The active player now is " + tempGame.ActivePlayer);
                    Console.WriteLine("Request came from " + gameRequestToHandle.UserFrom);

                    // Set new active player if the current active player's turn ends
                    SetActivePlayer(gameRequestToHandle, tempGame);
                    break;

                // Gets points from the users and find the winner
                case 10:
                    CardGame tempCardGame = GetCardGame(gameRequestToHandle.UserFrom);

                    // Sets the player points
                    SetPlayerPoints(gameRequestToHandle, tempCardGame);

                    if (tempCardGame.ScoresReceived == tempCardGame.ListOfUsers.Count)
                    {
                        // Finding Winner From TempCardGame
                        FindWinner(gameRequestToHandle, tempCardGame);

                        // Informs players of winner
                        for (int i = 0; i < tempCardGame.ListOfUsers.Count; i++)
                        {
                            gameRequestToHandle.UserTo = tempCardGame.ListOfUsers[i].Name;
                            SocketHandler.Send(gameRequestToHandle);
                        }
                    }
                    break;
                default:
                    break;
            }

        }

        /// <summary>
        /// Uses <see cref="CardGame.ListOfUsers"/> to find a winner based on higest points
        /// </summary>
        /// <param name="gameRequestToHandle"></param>
        /// <param name="tempCardGame"></param>
        private void FindWinner(GameRequest gameRequestToHandle, CardGame tempCardGame)
        {
            int highestScoore = 0;
            for (int i = 0; i < tempCardGame.ListOfUsers.Count; i++)
            {
                if (tempCardGame.ListOfUserPoints[i] > tempCardGame.ListOfUserPoints[highestScoore])
                {
                    highestScoore = i;
                }
                gameRequestToHandle.UserFrom = tempCardGame.ListOfUsers[highestScoore].Name;
            }
        }

        /// <summary>
        /// Set points for the individual <see cref="User"/>
        /// </summary>
        /// <param name="gameRequestToHandle"></param>
        /// <param name="tempCardGame"></param>
        private void SetPlayerPoints(GameRequest gameRequestToHandle, CardGame tempCardGame)
        {
            tempCardGame.ScoresReceived++;
            for (int i = 0; i < tempCardGame.ListOfUsers.Count; i++)
            {
                if (gameRequestToHandle.UserFrom == tempCardGame.ListOfUsers[i].Name)
                {
                    tempCardGame.ListOfUserPoints[i] = gameRequestToHandle.CardValue;
                }
            }
        }

        /// <summary>
        /// Sets the active <see cref="User"/>
        /// </summary>
        /// <param name="gameRequestToHandle"></param>
        /// <param name="tempGame"></param>
        private void SetActivePlayer(GameRequest gameRequestToHandle, CardGame tempGame)
        {
            if (gameRequestToHandle.UserFrom == tempGame.ActivePlayer)
            {
                tempGame.SetActivePlayer();

                GameRequest newActivePlayer = new GameRequest();
                newActivePlayer.UserTo = tempGame.ActivePlayer;
                newActivePlayer.RequestType = 1;

                Console.WriteLine("Set Active player to " + newActivePlayer.UserTo);
                Console.WriteLine("=========================================");
                Console.WriteLine("=========================================");
                SocketHandler.Send(newActivePlayer);
            }
        }

        /// <summary>
        /// Fetch the first item in the deck and adds to a <see cref="GameRequest"/>
        /// </summary>
        /// <param name="gameRequestToHandle"></param>
        private void FindCard(GameRequest gameRequestToHandle)
        {
            if (gameRequestToHandle.Cardlist.Count == 0)
            {
                for (int i = 0; i < Games.Count; i++)
                {
                    for (int j = 0; j < Games[i].ListOfUsers.Count; j++)
                    {
                        if (Games[i].ListOfUsers[j].Name == gameRequestToHandle.UserTo)
                        {
                            gameRequestToHandle.Cardlist.Add(Games[i].Deck.ElementAt(0));
                            Games[i].Deck.RemoveAt(0);
                            Console.WriteLine("===");
                            Console.WriteLine(Games[i].Deck.Count);
                            Console.WriteLine("===");
                        }
                    }
                }
                gameRequestToHandle.UserFrom = gameRequestToHandle.UserTo;
            }
        }

        /// <summary>
        /// Find a game through a player
        /// </summary>
        /// <param name="currentActivePlayer"></param>
        public CardGame GetCardGame(string currentActivePlayer)
        {
            foreach (CardGame c in Games)
            {
                foreach (User u in c.ListOfUsers)
                {
                    if (u.Name == currentActivePlayer)
                    {
                        return c;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Set an active player at the start of a game
        /// </summary>
        /// <param name="cg"></param>
        public void SetStartPlayer(CardGame cg)
        {
            GameRequest newActivePlayer = new GameRequest();
            newActivePlayer.UserTo = cg.ActivePlayer;
            newActivePlayer.RequestType = 1;

            SocketHandler.Send(newActivePlayer);
        }
    }
}

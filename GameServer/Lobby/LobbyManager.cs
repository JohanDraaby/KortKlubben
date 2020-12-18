using GameServer.Connection;
using GameServer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Lobby
{
    class LobbyManager
    {
        private List<GameLobby> lobbies;
        public List<GameLobby> Lobbies
        {
            get { return lobbies; }
            set { lobbies = value; }
        }

        private ICommunicate commHandler = new SocketHandler();

        public ICommunicate CommHandler
        {
            get { return commHandler; }
            set { commHandler = value; }
        }

        private GameLobby CreateNewLobby()
        {
            throw new NotImplementedException();
        }

        private void DisposeLobby()
        {
            throw new NotImplementedException();
        }

        private void KickPlayer(User user)
        {
            throw new NotImplementedException();
        }

        private void RequestGameStart()
        {
            throw new NotImplementedException();
        }

        private List<User> GetListOfConnectedUsers()
        {
            throw new NotImplementedException();
        }
    }
}

using GameServer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Lobby
{
    class GameLobby
    {
        private string name;
        public string Name
        {
            get { return name; }
        }

        private List<User> listOfUsers;
        public List<User> ListOfUsers
        {
            get { return listOfUsers; }
            set { listOfUsers = value; }
        }


    }
}

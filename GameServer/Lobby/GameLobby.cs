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
        //private string name;
        //public string Name
        //{
        //    get { return name; }
        //}

        //private List<User> listOfUsers;
        //public List<User> ListOfUsers
        //{
        //    get { return listOfUsers; }
        //    set { listOfUsers = value; }
        //}

        ///// <summary>
        ///// Sets up the game, waiting for users to join.
        ///// </summary>
        ///// <param name="o"></param>
        //public static void SetUpGame(object o)
        //{
        //    int id = (int)o; // Gives the client a unique ID for the game.
        //    TcpClient client;
        //    string data = "";
        //    GameRequest converted;

        //    NetworkStream stream;
        //    byte[] buffer = new byte[1024];
        //    int byte_count;

        //    lock (_lock) client = list_clients[id];

        //    while (true)
        //    {
        //        stream = client.GetStream();
        //        buffer = new byte[1024];
        //        byte_count = stream.Read(buffer, 0, buffer.Length);

        //        if (byte_count == 0)
        //        {
        //            break;
        //        }

        //        data = Encoding.UTF8.GetString(buffer, 0, byte_count);
        //        converted = JsonConvert.DeserializeObject<GameRequest>(data);

        //        switch (converted.RequestType)
        //        {
        //            case 1:
        //                AddTCPClient(converted.UserFrom, client); // Add TCP Client
        //                break;
        //        }

        //        // Waiting for to players to be in game
        //        while (tcpClients.Count != 2) { }
        //        break;
        //    }

        //    // Starts game
        //    StartGame(client, byte_count);
        //}
    }
}

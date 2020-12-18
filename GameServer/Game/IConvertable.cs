using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Game
{
    interface IConvertable
    {
        string ConvertFromGameRequest(GameRequest input);
        GameRequest ConvertToGameRequest(string input);
        byte[] ToByteArray(GameRequest input);
        GameRequest FromByteArray(byte[] input);
    }
}

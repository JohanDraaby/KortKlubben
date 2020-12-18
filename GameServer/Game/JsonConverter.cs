using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json; 
using Newtonsoft.Json.Serialization;

namespace GameServer.Game
{
    class JsonConverter : IConvertable
    {
        public string ConvertFromGameRequest(GameRequest input)
        {
            string messageToReturn = JsonConvert.SerializeObject(input, Formatting.Indented);

            return messageToReturn;
        }

        public GameRequest ConvertToGameRequest(string input)
        {
            GameRequest messageToReturn = JsonConvert.DeserializeObject<GameRequest>(input);

            return messageToReturn;
        }


        // Might be needed for chat later
        public GameRequest FromByteArray(byte[] input)
        {
            throw new NotImplementedException();
        }

        public byte[] ToByteArray(GameRequest input)
        {
            throw new NotImplementedException();
        }
    }
}

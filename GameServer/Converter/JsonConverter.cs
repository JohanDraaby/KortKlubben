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
        /// <summary>
        /// Serialize <see cref="GameRequest"/> into a string of a JSON format
        /// </summary>
        /// <param name="input"></param>
        public string ConvertFromGameRequest(GameRequest input)
        {
            string messageToReturn = JsonConvert.SerializeObject(input, Formatting.Indented);

            return messageToReturn;
        }

        /// <summary>
        /// Deserialize string of a JSON format into a <see cref="GameRequest"/>
        /// </summary>
        /// <param name="input"></param>
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

        // Might be needed for chat later
        public byte[] ToByteArray(GameRequest input)
        {
            throw new NotImplementedException();
        }
    }
}

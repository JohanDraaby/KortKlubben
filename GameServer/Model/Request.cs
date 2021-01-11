using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Model
{
    public class Request
    {
        private byte requestType;
        public byte RequestType
        {
            get { return requestType; }
            set { requestType = value; }
        }
    }
}

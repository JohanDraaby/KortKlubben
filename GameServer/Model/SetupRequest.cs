using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Model
{
    class SetupRequest : Request
    {
        private List<string> players;

        public List<string> Players
        {
            get { return players; }
            set { players = value; }
        }
    }
}
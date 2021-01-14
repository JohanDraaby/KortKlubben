using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace GoFishClient
{
    /// <summary>
    /// Deriving from <see cref="Request"/> - used to receive playerlist from server.
    /// </summary>
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
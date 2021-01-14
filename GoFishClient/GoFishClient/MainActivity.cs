using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using System.Threading;
using System.Collections.Generic;

namespace GoFishClient
{
    // This is the top bar
    [Activity(Label = "Go Fish!", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        TextView setsTextView;
        TextView statusTextView;
        Spinner playerCardSpinner;
        Spinner playerSpinner;
        TextView receivedCardsTextView;
        Button connectButton;
        Button buttonGoFish;

        bool gameEnded = false;
        bool active = false;

        static TcpClient client = new TcpClient();

        static List<Card> hand = new List<Card>();

        static byte points = 0;

        static string localPlayer = "Chris";

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            //Init Components
            setsTextView = FindViewById<TextView>(Resource.Id.SetsTextView);
            statusTextView = FindViewById<TextView>(Resource.Id.StatusTextView);
            playerCardSpinner = FindViewById<Spinner>(Resource.Id.PlayerCardSpinner);
            receivedCardsTextView = FindViewById<TextView>(Resource.Id.ReceivedCardsTextView);
            playerSpinner = FindViewById<Spinner>(Resource.Id.PlayerSpinner);
            buttonGoFish = FindViewById<Button>(Resource.Id.ButtonGoFish);
            connectButton = FindViewById<Button>(Resource.Id.ButtonConnect);

            buttonGoFish.Click += (sender, e) =>
            {
                SendCardRequest();
            };

            // Connect to server
            Connect();
        }

        /// <summary>
        /// Connects the client to the server.
        /// </summary>
        private void Connect()
        {
            connectButton.Click += (sender, e) =>
            {
                connectButton.Enabled = false;
                buttonGoFish.Enabled = true;

                IPAddress IP = IPAddress.Parse("172.16.19.10");
                int port = 5000;

                client = new TcpClient();

                client.Connect(IP, port);
                connectButton.Text = "FORBUNDET";

                NetworkStream networkStream = client.GetStream();

                Thread thread = new Thread(o => ReceiveData((TcpClient)o));
                thread.Start(client);

                SendConnectionRequest(client);

                SetupSpinner();
            };
        }

        /// <summary>
        /// Sets up the spinner, updating the <see cref="hand"/>
        /// </summary>
        void SetupSpinner()
        {
            RunOnUiThread(() =>
            {
                String[] items = new String[hand.Count];
                // Create a list of items for the spinner
                for (int i = 0; i < hand.Count; i++)
                {
                    items[i] = hand[i].FullName;
                }

                ArrayAdapter adapter = new ArrayAdapter(this, (Resource.Layout.support_simple_spinner_dropdown_item), items);
                // Set the spinners adapter.
                playerCardSpinner.Adapter = adapter;
            });
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        private void ReceiveData(TcpClient client)
        {
            // Receive requests
            NetworkStream ns = client.GetStream();

            byte[] receiveBytes = new byte[1024];
            int byteCount;

            while ((byteCount = ns.Read(receiveBytes, 0, receiveBytes.Length)) > 0)
            {
                Request request;

                // Set request type either GameRequest or SetupRequest.
                if (Encoding.UTF8.GetString(receiveBytes, 0, byteCount).Contains("Players"))
                {
                    request = JsonConvert.DeserializeObject<SetupRequest>(Encoding.UTF8.GetString(receiveBytes, 0, byteCount));
                }
                else
                {
                    request = JsonConvert.DeserializeObject<GameRequest>(Encoding.UTF8.GetString(receiveBytes, 0, byteCount));
                }

                GameRequest gameRequest;

                switch (request.RequestType)
                {
                    // End game
                    case 0:
                        gameEnded = true;
                        statusTextView.Text = "Spillet er slut!";
                        AnswerPointRequest();
                        break;
                    // Answer on request
                    case 1:
                        gameRequest = (GameRequest)request;

                        if (gameRequest.UserTo == localPlayer)
                        {
                            statusTextView.Text = "Det er din tur!";
                        }
                        else
                        {
                            statusTextView.Text = "Det er ikke din tur endnu!";
                        }
                        break;
                    // Receive card(s)
                    case 2:
                        gameRequest = (GameRequest)request;
                        UpdateUIOnReceive(gameRequest);

                        AddCardsToCollection(gameRequest.Cardlist, gameRequest.UserFrom);
                        CheckForSets();

                        SetupSpinner();
                        break;
                    // Give cards away
                    case 3:
                        gameRequest = (GameRequest)request;

                        GiveCardsAway(gameRequest, client);

                        SetupSpinner();
                        break;
                    // Set up list of players for the spinner
                    case 4:
                        SetupRequest setupRequest = (SetupRequest)request;
                        SetupPlayerlist(setupRequest);
                        break;
                    case 10:
                        gameRequest = (GameRequest)request;
                        receivedCardsTextView.Text = (gameRequest.UserFrom + " vandt med " + gameRequest.CardValue + " stik!");
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Sets the <see cref="playerSpinner"/> up with a list of players
        /// </summary>
        /// <param name="setupRequest"></param>
        private void SetupPlayerlist(SetupRequest setupRequest)
        {
            RunOnUiThread(() =>
            {
                String[] players = new String[setupRequest.Players.Count - 1];
                int counter = 0;
                foreach (string player in setupRequest.Players)
                {
                    if (player != localPlayer)
                    {
                        players[counter] = player;
                        counter++;
                    }
                }

                ArrayAdapter adapter = new ArrayAdapter(this, (Resource.Layout.support_simple_spinner_dropdown_item), players);

                playerSpinner.Adapter = adapter;
            });
        }

        /// <summary>
        /// When receiving <see cref="Card"/>s, update the UI appropriately
        /// </summary>
        /// <param name="gameRequest"></param>
        private void UpdateUIOnReceive(GameRequest gameRequest)
        {
            if (gameRequest.UserFrom == "Dealer")
            {
                statusTextView.Text = "Det er ikke din tur endnu!";
            }
            else if (gameRequest.UserFrom != localPlayer)
            {
                statusTextView.Text = "Det er din tur!";
                if (gameRequest.Cardlist.Count == 1)
                {
                    receivedCardsTextView.Text = "Du modtog " + gameRequest.Cardlist.Count + " kort!";
                }
                else
                {
                    receivedCardsTextView.Text = "Du modtog " + gameRequest.Cardlist.Count + " kort!";
                }
            }
            else
            {
                statusTextView.Text = "Det er ikke din tur endnu!";
                receivedCardsTextView.Text = "Fisk!";
            }
        }

        /// <summary>
        /// Check for sets of 4 <see cref="Card"/>s, then remove sets from hand and add to points.
        /// </summary>
        private void CheckForSets()
        {
            bool setFound = false;

            for (byte i = 1; i < 15; i++)
            {
                byte counter = 0;
                foreach (Card c in hand)
                {
                    if (c.Value == i)
                        counter++;

                    if (counter == 4)
                    {
                        setFound = true;
                        break;
                    }
                }

                if (setFound)
                {
                    points++;

                    setsTextView.Text = "Stik: " + points;

                    // Remove set from hand
                    for (int j = hand.Count - 1; j >= 0; j--)
                    {
                        if (hand[j].Value == i)
                        {
                            hand.RemoveAt(j);
                        }
                    }

                    if (hand.Count == 0)
                    {
                        GoFish();
                    }

                    setFound = false;
                }
            }
        }

        /// <summary>
        /// Sends a <see cref="GameRequest"/> to the server to ask for a card.
        /// </summary>
        private void GoFish()
        {
            GameRequest gr = new GameRequest();
            gr.UserFrom = localPlayer;
            gr.UserTo = localPlayer;

            string s;

            NetworkStream ns = client.GetStream();

            gr.RequestType = 3;
            byte[] buffer;

            gr.RequestType = 3;
            gr.CardValue = 0;

            s = JsonConvert.SerializeObject(gr, Formatting.Indented);
            buffer = Encoding.UTF8.GetBytes(s);
            ns.Write(buffer, 0, buffer.Length);
            active = false;

            statusTextView.Text = "Det er ikke din tur endnu!";
            receivedCardsTextView.Text = "Fisk!";
        }

        private static void AddCardsToCollection(List<Card> cardlist, string opponentName)
        {
            if (cardlist.Count > 0)
            {
                for (int i = 0; i < cardlist.Count; i++)
                {
                    hand.Add(cardlist[i]);
                }
            }
        }

        /// <summary>
        /// Checks <see cref="hand"/> to look for asked <see cref="Card"/>s. Removes <see cref="Card"/>s from <see cref="hand"/> that matches
        /// </summary>
        /// <param name="cardValue"></param>
        /// <param name="client"></param>
        private void GiveCardsAway(GameRequest gameRequest, TcpClient client)
        {
            List<Card> cardsToSend = new List<Card>();

            // Check cards
            for (int i = hand.Count - 1; i >= 0; i--)
            {
                if (hand[i].Value == gameRequest.CardValue)
                {
                    cardsToSend.Add(hand[i]);
                    hand.RemoveAt(i);
                }
            }

            if (hand.Count == 0)
            {
                GoFish();
            }

            GameRequest gr = SendRequestedCards(gameRequest, client, cardsToSend);

            string cardsStolen = "";

            #region Action description
            switch (gameRequest.CardValue)
            {
                case 1:
                    if (gr.Cardlist.Count == 1)
                    {
                        cardsStolen = gr.Cardlist.Count + " es";
                    }
                    else
                    {
                        cardsStolen = gr.Cardlist.Count + " esser";
                    }
                    break;
                case 11:
                    if (gr.Cardlist.Count == 1)
                    {
                        cardsStolen = gr.Cardlist.Count + " knægt";
                    }
                    else
                    {
                        cardsStolen = gr.Cardlist.Count + " knægte";
                    }
                    break;
                case 12:
                    if (gr.Cardlist.Count == 1)
                    {
                        cardsStolen = gr.Cardlist.Count + " dame";
                    }
                    else
                    {
                        cardsStolen = gr.Cardlist.Count + " damer";
                    }
                    break;
                case 13:
                    if (gr.Cardlist.Count == 1)
                    {
                        cardsStolen = gr.Cardlist.Count + " konge";
                    }
                    else
                    {
                        cardsStolen = gr.Cardlist.Count + " konger";
                    }
                    break;
                case 14:
                    if (gr.Cardlist.Count == 1)
                    {
                        cardsStolen = gr.Cardlist.Count + " joker";
                    }
                    else
                    {
                        cardsStolen = gr.Cardlist.Count + " jokere";
                    }
                    break;
                default:
                    if (gr.Cardlist.Count == 1)
                    {
                        cardsStolen = gr.Cardlist.Count + " " + gameRequest.CardValue;
                    }
                    else
                    {
                        cardsStolen = gr.Cardlist.Count + " " + gameRequest.CardValue + "s";
                    }
                    break;
            }
            #endregion

            receivedCardsTextView.Text = gr.UserTo + " har stjålet " + cardsStolen;
        }

        /// <summary>
        /// Sends <see cref="Cards"/> matching the original request.
        /// </summary>
        /// <param name="cardValue"></param>
        /// <param name="client"></param>
        /// <param name="cardsToSend"></param>
        /// <returns></returns>
        private GameRequest SendRequestedCards(GameRequest gameRequest, TcpClient client, List<Card> cardsToSend)
        {
            string s;
            GameRequest gr = new GameRequest();
            NetworkStream ns = client.GetStream();

            gr.Cardlist = cardsToSend;
            gr.RequestType = 3;
            gr.UserTo = gameRequest.UserFrom;
            gr.UserFrom = localPlayer;
            gr.CardValue = gameRequest.CardValue;

            s = JsonConvert.SerializeObject(gr, Formatting.Indented);
            byte[] buffer = Encoding.UTF8.GetBytes(s);
            ns.Write(buffer, 0, buffer.Length);
            return gr;
        }

        /// <summary>
        /// Sends a <see cref="ConnectionRequest"/> to the server.
        /// </summary>
        /// <param name="client"></param>
        private void SendConnectionRequest(TcpClient client)
        {
            GameRequest gr = new GameRequest();
            gr.UserFrom = localPlayer;
            if (playerSpinner.Count != 0)
            {
                gr.UserTo = playerSpinner.SelectedItem.ToString();
            }
            else
            {
                gr.UserTo = null;
            }

            string s;

            NetworkStream ns = client.GetStream();

            ConnectionRequest cr = new ConnectionRequest();
            cr.Username = localPlayer;
            cr.RequestType = 1;

            s = JsonConvert.SerializeObject(cr, Formatting.Indented);
            byte[] buffer = Encoding.UTF8.GetBytes(s);
            ns.Write(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// Requests a <see cref="Card"/> from a specified <see cref="User.Name"/>
        /// </summary>
        void SendCardRequest()
        {
            NetworkStream ns = client.GetStream();
            GameRequest gr = new GameRequest();

            for (int i = 0; i < hand.Count; i++)
            {
                if (hand[i].FullName == playerCardSpinner.SelectedItem.ToString())
                {
                    gr.CardValue = hand[i].Value;
                }
            }

            gr.RequestType = 1;
            gr.UserFrom = localPlayer;
            gr.UserTo = playerSpinner.SelectedItem.ToString();
            string s = JsonConvert.SerializeObject(gr, Formatting.Indented);
            byte[] buffer = Encoding.UTF8.GetBytes(s);
            buffer = Encoding.UTF8.GetBytes(s);
            ns.Write(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// Sends a <see cref="GameRequest"/>, containing accumulated <see cref="points"/> from a game session.
        /// </summary>
        void AnswerPointRequest()
        {
            NetworkStream ns = client.GetStream();
            GameRequest gr = new GameRequest();

            gr.RequestType = 10;
            gr.UserFrom = localPlayer;
            gr.CardValue = points;
            string s = JsonConvert.SerializeObject(gr, Formatting.Indented);
            byte[] buffer = Encoding.UTF8.GetBytes(s);
            buffer = Encoding.UTF8.GetBytes(s);
            ns.Write(buffer, 0, buffer.Length);
        }
    }
}
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Unosquare.Labs.EmbedIO;
using Unosquare.Labs.EmbedIO.Modules;
using Unosquare.Swan;

namespace CombatManager.LocalService
{
    public class CombatManagerNotificationServer : WebSocketsServer
    {
        CombatState state;

        public CombatManagerNotificationServer(CombatState state)
            : base(true)
        {
            this.state = state;

            state.CharacterAdded += State_CharacterAdded;
            state.CharacterRemoved += State_CharacterRemoved;
            state.CharacterPropertyChanged += State_CharacterPropertyChanged;
            state.CharacterSortCompleted += State_CharacterSortCompleted;
            state.PropertyChanged += State_PropertyChanged;
            state.TurnChanged += State_TurnChanged;
        }

        void SendState()
        {
            RemoteCombatState rs = state.ToRemote();
            LocalServiceMessage message = LocalServiceMessage.Create("State", rs);
            String data = JsonConvert.SerializeObject(message);


            new Thread(() =>
           Broadcast(data)
        ).Start();



        }


        private void State_TurnChanged(object sender, CombatStateCharacterEventArgs e)
        {
            SendState();
        }


        private void State_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
        }

        private void State_CharacterSortCompleted(object sender, EventArgs e)
        {
            
        }

        private void State_CharacterPropertyChanged(object sender, CombatStateCharacterEventArgs e)
        {
            switch (e.Property)
            {
                case "HP":
                case "Name":
                case "MaxHP":
                    SendState();
                    break;

            }
        }

        private void State_CharacterRemoved(object sender, CombatStateCharacterEventArgs e)
        {
            SendState();
        }

        private void State_CharacterAdded(object sender, CombatStateCharacterEventArgs e)
        {
            SendState();
        }

        public override string ServerName => "Combat State Notifcation Server";

        protected override void OnMessageReceived(IWebSocketContext context, byte[] rxBuffer, IWebSocketReceiveResult rxResult)
        {
            foreach (var ws in WebSockets)
            {
                //if (ws != context)
                  //  Send(ws, rxBuffer.ToText());
            }
        }

        protected override void OnClientConnected(
            IWebSocketContext context,
            System.Net.IPEndPoint localEndPoint,
            System.Net.IPEndPoint remoteEndPoint)
        {

        }

        protected override void OnFrameReceived(IWebSocketContext context, byte[] rxBuffer, IWebSocketReceiveResult rxResult)
        {
            // placeholder
        }

        protected override void OnClientDisconnected(IWebSocketContext context)
        {
        }
    }


}

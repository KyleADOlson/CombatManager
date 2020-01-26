using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EmbedIO;
using EmbedIO.WebSockets;

namespace CombatManager.LocalService
{
    public class CombatManagerNotificationServer : WebSocketModule
    {
        CombatState state;

        public CombatManagerNotificationServer(string path, CombatState state)
            : base(path, true)
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
           BroadcastAsync(data)
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


        protected override Task OnMessageReceivedAsync(IWebSocketContext context, byte[] buffer, IWebSocketReceiveResult result)
        {
            return Task.CompletedTask;
            
        }


        protected override Task OnClientConnectedAsync(
            IWebSocketContext context)
        {

            return Task.CompletedTask;
        }

        protected override Task OnFrameReceivedAsync(IWebSocketContext context, byte[] rxBuffer, IWebSocketReceiveResult rxResult)
        {

            return Task.CompletedTask;
        }

        protected override Task OnClientDisconnectedAsync(IWebSocketContext context)
        {
            return Task.CompletedTask;
        }


    }


}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unosquare.Labs.EmbedIO;
using Unosquare.Labs.EmbedIO.Modules;

namespace CombatManager.LocalService
{
    public class LocalCombatManagerService
    {
        public enum UIAction
        {
            BringToFront,
            Minimize,
            Goto,
            ShowCombatListWindow,
            HideCombatListWindow
        }

        public class UIActionEventArgs : EventArgs
        {
            public UIAction Action { get; set; }
            public object Data { get; set; }
        }

        public delegate void UIActionEvent(object sender, UIActionEventArgs args);


        public event UIActionEvent UIActionTaken;



        CombatState state;
        int port;
        WebServer server;
        Character.HPMode hpmode;

        public const ushort DefaultPort = 12457;


        public delegate void ActionCallback(Action action);

        public ActionCallback StateActionCallback { get; set; }

        public LocalCombatManagerServiceController serviceController;

        public Character.HPMode HPMode
        {
            get => hpmode;
            set
            {
                if (hpmode != value)
                {
                    hpmode = value;
                    if (serviceController != null)
                    {
                        serviceController.HPMode = value;
                    }
                }
            }

        }

        public LocalCombatManagerService(CombatState state, ushort port = DefaultPort)
        {
            this.state = state;
            this.port = port;

        }

        void RunActionCallback(Action action)
        {
            StateActionCallback?.Invoke(action);
        }

        public void TakeUIAction(UIAction ui, object data = null)
        {
            UIActionTaken?.Invoke(this, new UIActionEventArgs() { Action = ui, Data = data });
        }

        public void Start()
        {
            var url = "http://localhost:" + port + "/";


            // Our web server is disposable.
            server = new WebServer(port);
            server.RegisterModule(new WebApiModule());
            server.Module<WebApiModule>().RegisterController<LocalCombatManagerServiceController>(y =>
            {
                serviceController = new LocalCombatManagerServiceController(y, state, this, RunActionCallback);
                serviceController.HPMode = hpmode;
                return serviceController;
             });
            server.RunAsync();
    
        }

        public void Close()
        {
            if (server != null)
            {
                try
                {
                    server.Dispose();
                }
                catch (Exception)
                {

                }
                server = null;
            }

        }
    }
}

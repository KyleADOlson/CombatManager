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
        CombatState state;
        int port;
        WebServer server;

        public const ushort DefaultPort = 12457;

        public delegate void ActionCallback(Action action);

        public ActionCallback StateActionCallback { get; set; } 

        public LocalCombatManagerService(CombatState state, ushort port = DefaultPort)
        {
            this.state = state;
            this.port = port;

        }

        void RunActionCallback(Action action)
        {
            StateActionCallback?.Invoke(action);
        }

        public void Start()
        {
            var url = "http://localhost:" + port + "/";


            // Our web server is disposable.
            server = new WebServer(port);
            server.RegisterModule(new WebApiModule());
            server.Module<WebApiModule>().RegisterController<LocalCombatManagerServiceController>(y =>
                new LocalCombatManagerServiceController(y, state, RunActionCallback));
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

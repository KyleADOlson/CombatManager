using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmbedIO;
using EmbedIO.Routing;
using CombatManager;

namespace CombatManager.LocalService
{

    class CombatManagerHTMLServer : WebModuleBase
    {
        CombatState state;

        public CombatManagerHTMLServer(string baseRoute, CombatState state) : base(baseRoute)
        {
            this.state = state;
        }

        public override bool IsFinalHandler => true;

        protected override Task OnRequestAsync(IHttpContext context)
        {
            return ShowCombatStatePage(context);
        }

        private Task ShowCombatStatePage(IHttpContext context)
        {
            StringBuilder blocks = new StringBuilder();

            blocks.CreateHtmlHeader();

            blocks.Append(MonsterHtmlCreator.CreateCombatList(state));
                    

            blocks.CreateHtmlFooter();

            String text = blocks.ToString();

            context.Response.StatusCode = 200;


            return context.SendStringAsync(text, "text/html", Encoding.UTF8);

        }
    }
}

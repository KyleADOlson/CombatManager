using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmbedIO;
using EmbedIO.Routing;
using CombatManager;
using System.Text.RegularExpressions;
using CombatManager.Html;

namespace CombatManager.LocalService
{

    class CombatManagerHTMLServer : WebModuleBase
    {
        CombatState state;
        LocalCombatManagerService.ActionCallback actionCallback;

        public CombatManagerHTMLServer(string baseRoute, CombatState state, LocalCombatManagerService.ActionCallback actionCallback) : base(baseRoute)
        {
            this.state = state;
            this.actionCallback = actionCallback;
        }

        public override bool IsFinalHandler => true;

        protected override Task OnRequestAsync(IHttpContext context)
        {
            string[] path = SplitPath(context);


            path.TryGetIndex(0, out string service);
            path.TryGetIndex(1, out string param);

            Type t = path.GetType();

            switch (service)
            {
                case "css":
                    return ShowCSSPage(context, param);
                case "js":
                    return ShowJSPage(context, param);
                case "char":
                    return ShowCharPage(context, param);
                case "db":
                    return ShowDatabasePage(context, path.PopFront());
                    
            }

            return ShowCombatStatePage(context);

        }

        private string [] SplitPath(IHttpContext context)
        {
            string [] path = context.RequestedPath.Split('/');
            return path.PopFront();
        }

        private Task ShowErrorPage(IHttpContext context, int error, string text)
        {
            context.Response.StatusCode = error;
            return context.SendStringAsync(HtmlBlockCreator.CreateSimpleHtmlPage(text, "WebStyles"), "text/html", Encoding.UTF8);
        }

        private Task ShowCSSPage(IHttpContext context, string cssMatch)
        {

            if (cssMatch.IsEmptyOrNull())
            {

            }

            string text = HtmlBlockCreator.GetSaveTextFile(cssMatch);

            if (text.IsEmptyOrNull())
            {
                return ShowErrorPage(context, 404, "Not found");
            }
            context.Response.StatusCode = 200;
            return context.SendStringAsync(text, "text/css", Encoding.UTF8);
        }

        private Task ShowJSPage(IHttpContext context, string jsMatch)
        {

            if (jsMatch.IsEmptyOrNull())
            {
                return ShowErrorPage(context, 404, "Not found");

            }
            string text = HtmlBlockCreator.GetSaveTextFile(jsMatch);

            if (text.IsEmptyOrNull())
            {
                return ShowErrorPage(context, 404, "Not found");
            }
            context.Response.StatusCode = 200;
            return context.SendStringAsync(text, "application/javascript", Encoding.UTF8);
        }

        private Task ShowCharPage(IHttpContext context, string id)
        {
            if (id.IsEmptyOrNull() || !Guid.TryParse(id, out Guid gid) || ! state.TryGetCharacterById(gid, out var ch))
            {
                return ShowErrorPage(context, 404, "Not found");
            }

            String text = MonsterHtmlCreator.CreateHtml(ch.Monster, ch, completePage : true, css: "WebStyles");


            context.Response.StatusCode = 200;
            return context.SendStringAsync(text, "text/html", Encoding.UTF8);
        }

        private Task ShowCombatStatePage(IHttpContext context)
        {

            StringBuilder blocks = new StringBuilder();

            blocks.CreateHtmlHeader("WebStyles", cl: "combatbody");
            blocks.AppendOpenTag("div", cl: "combatpage");

            actionCallback(() =>
            {
                blocks.Append(MonsterHtmlCreator.CreateCombatList(state));
            });

            blocks.AppendTag("div", classname: "divider");
            string texaasdft = blocks.ToString();

            actionCallback(() =>
            {
                blocks.Append(PlayerHtmlCreator.CreatePlayerView(context, state));
            });
            blocks.AppendCloseTag("div");

            blocks.CreateHtmlFooter();

            String text = blocks.ToString();

            context.Response.StatusCode = 200;


            return context.SendStringAsync(text, "text/html", Encoding.UTF8);

        }

        private Task ShowDatabasePage(IHttpContext context, string [] param)
        {
            if (!param.TryGetIndex(0, out var type))
            {
                return ShowErrorPage(context, 404, "Not found");
            }

            var queryData = context.GetRequestQueryData();
            bool hasId = queryData.TryGetInt("id", out int id);
            bool custom = queryData.GetBool("custom");

            string text = "";

            if (!hasId && (new string [] { "monster", "spell", "feat", "magicitem"}).Contains(type))
            {
                return ShowErrorPage(context, 404, "Not found");
            }


            switch (type)
            {
                case "monster":
                    if (!Monster.TryByID(custom, id, out Monster m))
                    {
                        return ShowErrorPage(context, 404, "Not found");
                    }

                    text = MonsterHtmlCreator.CreateHtml(monster: m, completePage: true, css: "WebStyles", addDescription: true) ;
                    break;
                case "spell":
                    if (!Spell.TryByID(custom, id, out Spell s))
                    {
                        return ShowErrorPage(context, 404, "Not found");
                    }

                    text = SpellHtmlCreator.CreateHtml(spell: s, completepage: true, css: "WebStyles");
                    break;
                case "feat":
                    if (!Feat.TryByID(custom, id, out Feat f))
                    {
                        return ShowErrorPage(context, 404, "Not found");
                    }

                    text = FeatHtmlCreator.CreateHtml(feat: f, completepage: true, css: "WebStyles");
                    break;
                case "magicitem":
                    if (!MagicItem.TryByID(custom, id, out MagicItem mi))
                    {
                        return ShowErrorPage(context, 404, "Not found");
                    }

                    text = MagicItemHtmlCreator.CreateHtml(item: mi, completepage: true, css: "WebStyles");
                    break;
                default:
                    return ShowErrorPage(context, 404, "Not found");
            }



            context.Response.StatusCode = 200;
            return context.SendStringAsync(text, "text/html", Encoding.UTF8);
        }
    }
}

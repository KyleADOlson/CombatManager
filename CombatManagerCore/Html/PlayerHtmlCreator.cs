using EmbedIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CombatManager.Html
{
    public class PlayerHtmlCreator
    {
        public static string CreatePlayerView(IHttpContext context, CombatState state)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendTag("script", attributes: new[] { ("src", "/www/js/PlayerWebPage.js") });
            builder.AppendOpenTag("div", cl: "playerview");
            builder.CreateHeader("Player View");
            builder.AppendSpanWithTooltip(content: "player", tiptext: "Player!");
            builder.AppendSelect(from ch in state.Characters where !ch.IsMonster select (ch.ID.ToString(), ch.Name), id: "playerSelect", attributes: new []{ ("onchange", "playerSelectChanged()")});
            
            builder.AppendCloseTag("div");
            return builder.ToString(); 
        }
    }
}

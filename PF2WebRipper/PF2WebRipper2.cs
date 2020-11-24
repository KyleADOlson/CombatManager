using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace PF2WebRipper
{
    public class PF2WebRipper2
    {
        public event Action<string> MessageCallback;

        const string baseUrl = "https://2e.aonprd.com/Monsters.aspx?ID=";
        const string monsterPattern = "monster.{0}.html";
        const string monsterSPattern = "monster.s.{0}.txt";

        const int start = 1;
        const int end = 882;

        public async Task GetPages()
        {
            for (int i= start; i< end; i++)
            {
                if (!File.Exists(PageName(i)))
                {
                    await UrlName(i).DownloadToAsync(PageName(i));
                    Thread.Sleep(1000);
                    ConsoleHelper.Write(PageName(i));
                        
                }
            }

        }

        public async Task ParsePages()
        {

            for (int i = start; i < end; i++)
            {
                await SplitPage(i);
            }
        }

        

        public async Task SplitPage(int num)
        {
            string text = await PageName(num).LoadFileAsync();

            if (Regex.Match(text, "Object reference not set to an instance of an object").Success)
            {
                return;
            }

            string rt = "<h1 class=\"title\">(<a href=\"Monsters.aspx\\?ID=[0-9]+\">)?(?<body>(.|\\n)+)";
            rt += new string[] { "</span></span>" + SB().Whitespace().ToString() + "<br />", "<h2 class=\"title\">All Monsters", 
            
            "<br />" + SB().Whitespace().ToString() + "</div>" + SB().Whitespace().ToString() + "<div class=\"clear"
            }.Options(group:true) ;

            rt.SpitJSRegex("rt.txt");

            Regex r = new Regex(rt);
            Match m = r.Match(text);

            if (m.Success)
            {
                string textout = m.Groups["body"].ToString();
                if (textout == null || textout.Trim().Length < 5)
                {
                    ConsoleHelper.Write("X " + num);
                }
               textout.SpitFile(SplitName(num));
            }
            else
            {
                ConsoleHelper.WriteLine("Failed: " + num);
            }
        }


        public string PageName(int num) => string.Format(monsterPattern, num);


        public string SplitName(int num) => string.Format(monsterSPattern, num);


        public string UrlName(int num) => baseUrl + num;
        


        private static StringBuilder SB(string text = null)
        {
            return (text == null) ? new StringBuilder() : new StringBuilder(text);
        }

        private static StringBuilder SB(StringBuilder builder)
        {
            return new StringBuilder(builder.ToString());
        }
    }
}

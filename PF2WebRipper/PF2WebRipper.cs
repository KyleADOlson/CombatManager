using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace PF2WebRipper
{
    public class PF2WebRipper
    {
        const string pagematch = "(?<item><tr>\\s*(\\r?\\n)?\\s*<td>\\s*" +
            "<a href=\"(?<page>https://pf2.d20pfsrd.com/(?<type>[-A-Za-z0-9]+)/[-A-Za-z0-9]+)/\">" +
            "(?<name>[-\\., A-Za-z0-9()]+)</a></td>\\s*(\\r?\\n)?\\s*<td>(?<type2>[-A-Za-z0-9]+)</td>" +
            "\\s*(\\r?\\n)?\\s*<td><a href=\"(?<puburl>https?://([-\\.A-Za-z0-9()]+|/)+)\" rel=\"tag\">" +
            "(?<pubname>[-\\., A-Za-z0-9()]+)</a></td>\\s*(\\r?\\n)?\\s*<td><a href=\"(?<sourceurl>https?://([-\\.A-Za-z0-9()]+|/)+)\" " +
            "rel=\"tag\">(?<sourcename>[-\\., A-Za-z0-9()]+)</a></td>\\s*(\\r?\\n)?\\s*</tr>)";



        HttpClient client;
        public PF2WebRipper()
        {

            client = new HttpClient();

        }



        public async Task<List<PF2WebListItem>> GetMonsters()
        {

            var list = new List<PF2WebListItem>();

            StringBuilder match = new StringBuilder();
            match.OpenRow();
            match.MakeUrlCell("url", "name", false);
            match.MultiUrlCell("familyurl", "family", optional: true);
            match.MakeTextCell(name:"level");
            match.MakeUrlCell("publisherurl", "publisher", true);
            match.MakeUrlCell("sourceurl", "source", true);
            match.CloseRow();


            HttpResponseMessage resp = await client.GetAsync("https://pf2.d20pfsrd.com/monster");

            string text = await resp.Content.ReadAsStringAsync();

            string mt = match.ToString();

            var m = Regex.Matches(text, mt);



            using (var s = File.Open("lasturl.txt", FileMode.Create))
             using (var r = new StreamWriter(s))
             {
                r.WriteLine(mt);
            }
            foreach (Match mo in m)
            {
                var x = new PF2WebListItem()
                {
                    Name = mo.Value("name"),
                    Family = mo.Value("family"),
                    Url = mo.Value("url"),
                    Publisher = mo.Value("publisher"),
                    Source = mo.Value("source"),
                    Level = mo.Value("level")
                };
                list.Add(x);
            }


            return list;
        }
        

        public async Task<List<PF2WebListItem>> GetSourcePage()
        {

            var list = new List<PF2WebListItem>();

            HttpResponseMessage resp = await client.GetAsync("https://pf2.d20pfsrd.com/srd-content-source/pf2bestiary/");

            string text = await resp.Content.ReadAsStringAsync();



            var m = Regex.Matches(text, pagematch);



            foreach (Match mo in m)
            {
                var x = new PF2WebListItem()
                {
                    Name = mo.Value("name"),
                    Type = mo.Value("type"),
                    Url = mo.Value("page"),
                    Publisher = mo.Value("pubname"),
                    Source = mo.Value("source")
                };
                list.Add(x);
            }


            return list;
        }


        public StringBuilder AddHeader()
        {
             string headermatch1 = "<h4 class=\"monster\">(?<name>[-\\., A-Za-z0-9()]+) <span class=\"monster-level\">(?<type>[-\\., A-Za-z0-9()]+) (?<cr>[0-9]+)</span></h4>";

            var sb1 = SB(headermatch1);
            var sb2 = SB().AddTag("h1", content: SB().TextGroup()).Whitespace().AddOpenTag("p", "header");

            //Froghemoth <span class="level">Creature 13</span></p>
            sb2.TextGroup(group: "name2").AddTag("span", cl: "level", content: "(?<type2>[-\\., A-Za-z0-9()]+) (?<cr2>[0-9]+)").AddCloseTag("p");
            var sb = SB((new[] { sb1, sb2 }).Options());

            return sb;
        }
        

        public void AddFirstSection(StringBuilder sb)
        {
            sb.Whitespace();

            StringBuilder traits = new StringBuilder();
            traits.AddOpenTag("p", cl: "traits");
            traits.OptionalGroup(SB().AddCloseTag("p"));
            traits.AddSpan(cl: "frequency-[a-z]+", content: SB().MakeTextGroup("frequency"), min: 0);

            traits.AddSpan(cl: "alignment", content: SB().MakeTextGroup("alignment"));
            traits.AddSpan(cl: "size", content: SB().MakeTextGroup("size"));
            traits.AddSpan(cl: "type_[a-z]+", content: SB().MakeTextGroup("typetrait"), min: 0);
            traits.AddSpan(cl: "trait", content: SB().TextOrUrlGroup("trait", "traitu"), min: 0, max: -1);

            sb.Append(traits.ToString());
        }


        public void AddToPerception(StringBuilder sb)
        {
            sb.AddCloseTag("p");
            sb.AddOpenTag("p");
            sb.AddTag("b", content: "Senses");
            sb.AddLink("Perception");
            sb.PFNumberGroup("perception");
            var optper = SB().Append(" \\(");
            var ppp = SB().Append(",? ?").PFNumberGroup("optperval").Append(" ").TextGroup(group: "optpertext", options: new TextGroupOptions() { comma = false, paren = false });
            optper.Group(ppp, "fullopt", min: 1, max: -1);
            optper.Append("\\)");
            optper.ToString().SpitFile("optper.txt");
            sb.Group(optper, name: "perceptionmods", min: 0);

        }

        void AddToRough(StringBuilder sb)
        {
            var persb = SB();
            persb.Append("; ");



            var open = SB().AddOpenTag("a", cl: "[-_a-zA-Z0-9]+", attributes: new[] { WRAttribute.Url() }, classoptional: true);
            var front = " ?" + open.ToString().Wrap("()") + "?";
            var mid = SB().Group(WRRegexUtils.TextPatternNoComma, name: "peritem").ToString();
            var back = "(</a>" + SB().Group(WRRegexUtils.TextPatternNoCommaOpt, name: "perend").ToString() + ")?";
            string endGroup = "(, |</)";
            var all = (front + mid + back + endGroup).Wrap("()") + "*";

            all.SpitFile("rough.txt");

            persb.Append(all);
            string rough = persb.ToString();
            sb.Append(rough);
            sb.Append("p>").Whitespace();
        }

        void AddToStats(StringBuilder sb)
        {

            /*sb.AddOpenTag("p").AddTag("b", content: "Languages");
            sb.TextGroup("languages");
            sb.AddCloseTag("p").Whitespace();
            */
            sb.PlainPLine("languages", "Languages");
            //< p >< b > Skills </ b >
            var sksb = SB().AddOpenTag("p").AddTag("b", content: "Skills");
            // <a href="https://pf2.d20pfsrd.com/rules/skills/#Acrobatics_Dex">Acrobatics</a> +16,
            //Axis <a href="https://pf2.d20pfsrd.com/rules/skills/#Lore_Int">Lore</a> +17, Craft +21,
            //<a href="https://pf2.d20pfsrd.com/rules/skills/#Diplomacy_Cha">Diplomacy</a> +15,
            //<a href="https://pf2.d20pfsrd.com/rules/skills/#Occultism_Int">Occultism</a> +17,
            //<a href="https://pf2.d20pfsrd.com/rules/skills/#Religion_Wis">Religion</a> +17
            //</p>.

            var inditag = SB().TextGroup(options: new TextGroupOptions() { comma = false }, min: 0).Group(SB().AddLink(content: SB().TextGroup().ToString()).ToString(), min: 0)
                .TextGroup(options: new TextGroupOptions() { comma = false }, min: 0)
                .Append(" ").PFNumberGroup().Append("(,|</p)");
            sksb.Group(inditag, min: 0, max: -1);

            sksb.SpitJSRegex("skills.txt");
            sb.Group(sksb, name: "skills");
            sb.Append(">").Whitespace();
            // <p><b>Str</b> +5, <b>Dex</b> +1, <b>Con</b> +6, <b>Int</b> +3, <b>Wis</b> +5, <b>Cha</b> +4</p> 
            sb.AddOpenTag("p");
            AddAbilityTag(sb, "Str").Append(", ");
            AddAbilityTag(sb, "Dex").Append(", ");
            AddAbilityTag(sb, "Con").Append(", ");
            AddAbilityTag(sb, "Int").Append(", ");
            AddAbilityTag(sb, "Wis").Append(", ");
            AddAbilityTag(sb, "Cha");

            sb.AddCloseTag("p");
        }

        void AddToSaves(StringBuilder sb)

        {
            var optDivEnd = SB().Group(SB().AddCloseTag("div").Whitespace(), min: 0);

            sb.PlainPLine("itemsline", "Items", min: 0);

            //other abilities

            var oat = SB().AddOpenTag("p").AddTag("b", content: SB().TextGroup().ToString());
            oat.LinksOrText();
            oat.AddCloseTag("p").Whitespace();
            sb.Group(oat, name: "otherpower", min: 0, max: -1);
            oat.SpitJSRegex("oat.txt");
            sb.Append(optDivEnd);
            sb.Append("<hr( /)?>").Whitespace();
            sb.AddOpenTag("p").AddTag("b", "AC");
            sb.Append(" ").NumberGroup(group: "ac").Append("; ");
            AddAbilityTag(sb, "Fort").Append(", ");
            AddAbilityTag(sb, "Ref").Append(", ");
            AddAbilityTag(sb, "Will");
            sb.Group(SB("; ").TextGroup("bonussave"), min: 0);
            sb.AddCloseTag("p");

        }

        void AddToResist(StringBuilder sb)
        {

            sb.AddOpenTag("p");

            AddAbilityTag(sb, "HP", pf: false);
            sb.Group(name: "hpextra", text: SB(", ").LinksOrText(new TextGroupOptions() { semicolon = false }), min: 0);
            var im = SB("; ");
            AddTextTag(im, "Immunities", new TextGroupOptions() { semicolon = false });
            sb.Group(im, min: 0);
            var weak = SB("; ");
            AddTextTag(weak, "Weaknesses", new TextGroupOptions() { semicolon = false });
            sb.Group(weak, min: 0);
            var resi = SB("; ");
            AddTextTag(resi, "Resistances", new TextGroupOptions() { semicolon = false });
            sb.Group(resi, min: 0);
            sb.AddCloseTag("p");

        }

        void AddToEnd(StringBuilder sb)
        {
            //<p><b>Reality Twist</b> [reaction] <b>Trigger</b> The pleroma critically fails the saving throw. <b>Effect</b> The critical failure becomes a normal failure.</p>
            var reas = SB().AddOpenTag("p");
            reas.AddTag("b", content: SB().TextGroup().ToString()).LinksOrText();
            reas.Group(SB().AddTag("b", content: "Trigger").LinksOrText(), min: 0);
            reas.Group(SB().AddTag("b", content: "Effect").LinksOrText(), min: 0);
            reas.AddCloseTag("p");
            sb.Group(reas, name: "reactions", min: 0, max: -1);

            sb.Append("<hr( /)?>");

            //<p><b>Speed</b> fly 40 feet; <a class="spell" href="https://pf2.d20pfsrd.com/spell/freedom-of-movement/">freedom of movement</a></p> 
            //<p><b>Melee</b> [one-action] energy touch (agile, lawful, magical) +36, <b>Damage</b> 5d8+18 positive or negative damage plus 1d6 lawful</p> 
            //<p><b>Ranged</b> [one-action] Sphere of Oblivion +37 (magical), <b>Effect</b> see Sphere of Oblivion</p> 

            sb.Whitespace();
            sb.Group(SB().AddOpenTag("p").AddTag("b", content: "Speed").Append(SB().
                Group(SB().LinksOrText(new TextGroupOptions()), name: "speedline")).AddCloseTag("p"));

            //<p><b>Melee</b> [one-action] shortsword +9 (agile, finesse, lawful, magical, versatile S), <b>Damage</b> 1d6+1 piercing plus 1d4 lawful</p>


            //<p><b>Melee</b> [one-action] bastard sword +26 (lawful, magical, two-hand d12), <b>Damage</b> 2d8+13 slashing plus 1d6 lawful</p> 
            //<p><b>Melee</b> [one-action] fist +23 (agile, lawful, magical), <b>Damage</b> 1d10+11 bludgeoning plus 1d6 lawful</p> 

            sb.Group(SB().AddOpenTag("p").AddTag("b", content: "Melee").Append(SB().
                Group(SB().LinksOrText(new TextGroupOptions()).Append(",").Whitespace().AddTag("b", content: "Damage").LinksOrText(), name: "meleeline")).AddCloseTag("p"), max: -1);

            //<p><b>Ranged</b> [one-action] Sphere of Oblivion +37 (magical), <b>Effect</b> see Sphere of Oblivion</p> 

            //sb.Group(SB().AddOpenTag("p"))

            /*
               <p><b>Speed</b> 10 feet, swim 60 feet</p> 
  <p><b>Melee</b> [one-action] tentacle +16 (agile, magical, reach 15 feet), <b>Damage</b> 2d8+10 bludgeoning plus slime</p> 
  <p><b>Occult Innate Spells</b> DC 25; <b>7th</b> <a class="spell" href="https://pf2.d20pfsrd.com/spell/project-image/">project image</a> (at will), <a class="spell" href="https://pf2.d20pfsrd.com/spell/veil/">veil</a> (at will); <b>6th</b> <a class="spell" href="https://pf2.d20pfsrd.com/spell/dominate/">dominate</a> (×3), <a class="spell" href="https://pf2.d20pfsrd.com/spell/illusory-scene/">illusory scene</a> (at will); <b>5th</b> <a class="spell" href="https://pf2.d20pfsrd.com/spell/illusory-object/">illusory object</a> (at will); <b>4th</b> <a class="spell" href="https://pf2.d20pfsrd.com/spell/hallucinatory-terrain/">hallucinatory terrain</a> (at will); <b>3rd</b> <a class="spell" href="https://pf2.d20pfsrd.com/spell/hypnotic-pattern/">hypnotic pattern</a> (at will)</p> 
  <p><b>Slime</b> (curse, occult, virulent); <b>Saving Throw</b> <a href="https://pf2.d20pfsrd.com/rules/character-creation/#Fortitude">Fortitude</a> DC 25; <b>Stage 1</b> no ill effect (1 round); <b>Stage 2</b> the victim's skin softens, inflicting drained 1 (1 round); <b>Stage 3</b> the victim's skin transforms into a clear, slimy membrane, inflicting drained 2 until the curse ends; every hour this membrane remains dry, the creature's drained condition increases by 1 (permanent).</p> 
  <p>A <a class="spell" href="https://pf2.d20pfsrd.com/spell/remove-disease/">remove disease</a> spell can counteract this curse, but immunity to disease offers no protection against it.</p> 
<p class="header">About</p>
             * */




            //string val = SB().AddOpenTag("p").AddBText("Ranged").Group(SB().AddBText("[' a-zA-Z0-9]+"), min: 0, max:-1).AddCloseTag("p").ToString();
            //sb.Group(val, min: 0);
            sb.PBoldLine("ranged", "Ranged", min: 0, breaks: true);
            sb.PBoldLineAnon("otherline", WRRegexUtils.TitleString, min: 0, max: -1, breaks: true);
            //sb.Group(SB().AddOpenTag("p").AddTa("b", content: SB().TextGroup()).Append(SB().
            //   Group(SB().LinksOrText(new TextGroupOptions()), name: "others")).AddCloseTag("p"), min: 0, max: -1);

            //<p class="header">About</p>
            sb.Append("<p");

            //sb.AddOpenTag("p", cl: "header").Append("About");//.AddCloseTag("p");
            //sb.AddOpenTag("p").LinksOrText().AddCloseTag("p");
            //sb.Append("Section");

            //sb.AddTag("p", cl: "header", classoptional:true, content: "About");


            //itemes


        }


        public string BuildMatchText()
        {
            var optDiv = SB().Group(SB().AddOpenTag("div", cl: "[-a-zA-Z0-9_]+", classoptional: true).Whitespace(), min: 0); 
            
            
            StringBuilder sb = AddHeader();

            AddFirstSection(sb);

            /*AddToPerception(sb);

            AddToRough(sb);

            AddToStats(sb);



            AddToSaves(sb);

            AddToResist(sb);

            AddToEnd(sb);*/





            return sb.ToString();
        }

        public StringBuilder AddAbilityTag(StringBuilder sb, string name, bool pf = true)
        {
            sb.AddTag("b", content: name);
            sb.Append(" ");
            if (pf)
            {
                sb.PFNumberGroup(name);
            }
            else

            {
                sb.NumberGroup(name);
            }
            return sb;
        }

        public StringBuilder AddTextTag(StringBuilder sb, string name, TextGroupOptions options = null)
        {
            sb.AddTag("b", content: name);
            sb.Append(" ");
            sb.TextGroup(group:name, options:options);
            return sb;
        }

        public string InfoFileName(PF2WebListItem item)
        {
            return item.Name + ".f.html";
        }

        public string BuildFileName(PF2WebListItem item)
        {
            return item.Name + ".b.html";
        }

        public async Task<string> GetPage(PF2WebListItem item)
        {
            string infoname = InfoFileName(item);

            string text;

            if (File.Exists(infoname))
            {
                text = infoname.LoadFile();
            }
            else
            {

                HttpResponseMessage resp = await client.GetAsync(item.Url);

                text = await resp.Content.ReadAsStringAsync();
                text.SpitFile(infoname);


                Thread.Sleep(500);

            }


            return text;
        }


        public async Task<List<PF2WebMonsterInfo>> GetInfo(PF2WebListItem item)
        {
            List<PF2WebMonsterInfo> list = new List<PF2WebMonsterInfo>();

            string text = await GetPage(item);

            text = text.PrefixEscapes();
            text.SpitFile(item.Name + ".b.html");


            string matchtext = BuildMatchText() ;

            var ma = Regex.Matches(text, matchtext) ;

            using (var s = File.Open(item.Name + ".txt", FileMode.Create))
            using (var r = new StreamWriter(s))
            {
                int foo = 0;

                foreach (Match m in ma)
                {

                    PF2WebMonsterInfo info = new PF2WebMonsterInfo();
                    info.Name = m.Value("name") ?? m.Value("name2");

                    
                    
                    info.Type = m.Values("trait");
                    list.Add(info);

                    if (foo > 0)
                    {
                        r.WriteLine("--------");
                    }
                    foo++;


                    r.WriteLine(info.Name);


                }
            }


            return list;
        }

        private static StringBuilder SB(string text = null)
        {
            return (text == null) ? new StringBuilder() : new StringBuilder(text);
        }

        private static StringBuilder SB(StringBuilder builder)
        {
            return new StringBuilder(builder.ToString());
        }
    }
    public class PF2WebListItem
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Family { get; set; }
        public string Url { get; set; }
        public string Publisher { get; set; }
        public string Source { get; set; }

        public string Level { get; set; }

    }

    public class PF2WebMonsterInfo
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string CL { get; set; }

        public string Rarity { get; set; }
        public string Alignment { get; set; }
        public string Size { get; set; }
        public List<string> Traits { get; set; }   
    }

}

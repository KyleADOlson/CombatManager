using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PF2WebRipper
{


    public static class WRRegexUtils
    {
        const string rowOpen = "<tr>";
        const string rowClose = "</tr>";
        const string cellOpen = "<td>";
        const string cellClose = "</td>";
        const string hrefOpenStart = "<a href=\"";
        const string hrefOpenEnd = "\"( rel=\"tag\")?>";
        const string hrefClose = "</a>";


        const string textPattern = "[-\\.:#×, A-Za-z0-9()]+";
        const string textPatternNoCommaBase = "[-\\.:#× A-Za-z0-9()]";
        const string textPatternNoComma = textPatternNoCommaBase + "+";
        const string textPatternNoParen = "[-\\.:#×, A-Za-z0-9]+";
        const string textPatternNoCommaParen = "[-\\.:#× A-Za-z0-9]+";
        const string textPatternNoCommaOpt = textPatternNoCommaBase + "*";
        const string numPattern = "-?[0-9]+";
        const string urlPattern = "https?://[-\\.#×A-Za-z0-9()/_]+";
        const string simpleTextPattern = "[-A-Za-z0-9_]+";

        public static string UrlPattern
        {
            get
            {
                return urlPattern;
            }

        }

        public static string TextPattern
        {
            get
            {
                return textPattern;
            }

        }
        public static string TextPatternNoComma
        {
            get
            {
                return textPatternNoComma;
            }

        }
        public static string TextPatternNoParen
        {
            get
            {
                return textPatternNoParen;
            }

        }
        public static string TextPatternNoCommaParen
        {
            get
            {
                return textPatternNoCommaParen;
            }

        }
        public static string TextPatternNoCommaOpt
        {
            get
            {
                return textPatternNoCommaOpt;
            }

        }
        public static string SimpleTextPattern
        {
            get
            {
                return simpleTextPattern;
            }

        }

        public static string NumPattern
        {
            get
            {
                return numPattern;
            }
        }

        public static string PrefixEscapes(this string text)
        {
            text = text.Replace("&#8216;", "'");
            text = text.Replace("&#8217;", "'");

            // AC 26; <b>Fort
            /*
  </p> 
  <hr> 
  <p><b>AC</b>
             */
            string m = " AC ([0-9]+); <b>Fort";
            text = Regex.Replace(text, m, "</p> \r\n  <hr>\r\n  <p><b>AC</b> $1; <b>Fort");

            foreach (string s in new[] { "Immunities", "Weaknesses", "Resistances" })
            {
                text = text.FixBoldGroupS(s);
            }
            text = text.KillDiv();
            text = text.BlankKill();
            return text;
        }

        static string FixBoldGroupS(this string text, string name)
        {
            return text.Replace(", <b>" + name, "; <b>" + name);
        }

        static string KillDiv(this string text)
        {

            var div = SB().AddOpenTag("div", cl: "[- a-zA-Z0-9_]+", classoptional: true).ToString();
            text = Regex.Replace(text, div, "");

            var dive = SB().AddCloseTag("div").ToString();
            text = Regex.Replace(text, dive, "");
            return text;
        }

        static string BlankKill(this string text)
        {
            var x = "(?<!\\r)\\n";
            int l1 = text.Length;
            text = Regex.Replace(text, x, "\r\n");
            int l2 = text.Length;
            var t = "\\r\\n( *\\r\\n)+";
            return Regex.Replace(text, t, "\r\n");

        }


        const string wsBreakws = "([ \\t\\r\\n]*)";

        const int groupmin = 1;
        const int groupmax = 1;

        public static StringBuilder OpenRow(this StringBuilder s, bool whitespace = true)
        {
            return s.Append(rowOpen).Whitespace(whitespace);

        }
        public static StringBuilder CloseRow(this StringBuilder s, bool whitespace = true)
        {
            return s.Append(rowClose).Whitespace(whitespace);

        }
        public static StringBuilder OpenCell(this StringBuilder s, bool whitespace = false)
        {
            return s.Append(cellOpen).Whitespace(whitespace);

        }
        public static StringBuilder CloseCell(this StringBuilder s, bool whitespace = true)
        {
            return s.Append(cellClose).Whitespace(whitespace);

        }

        public static StringBuilder Whitespace(this StringBuilder s, bool active = true)
        {
            if (active && !s.ToString().EndsWith(wsBreakws))
            {
                return s.Append(wsBreakws);
            }
            return s;
        }

        public static StringBuilder Spaces(this StringBuilder s, bool active = true)
        {
            if (active)
            {
                return s.Append(" *");
            }
            return s;
        }


        public static StringBuilder SB()
        {
            return new StringBuilder();
        }
        public static StringBuilder SB(string s)
        {
            return new StringBuilder(s);
        }


        public static StringBuilder MakeUrlCell(this StringBuilder s, string urlname, string contentname, bool optional = true)
        {
            var s1 = SB().OpenCell();

            s1.OptionalGroup(MakeURLContentGroup(urlname, contentname), optional);
            s1.CloseCell();
            s1.Whitespace();
            return s.Append(s1);
        }

        const string defSeparator = " *, *";

        public static StringBuilder MultiUrlCell(this StringBuilder s, string urlname, string contentname, string separator = defSeparator, bool optional = true)
        {
            var s1 = SB().OpenCell();
            s1.List(MakeURLContentGroup(urlname, contentname), min: optional ? 0 : 1);
            s1.CloseCell();
            s1.Whitespace();
            String t = s1.ToString();
            return s.Append(s1);
        }

        public static StringBuilder List(this StringBuilder s, string content, string separator = defSeparator, int min = 0, int max = int.MaxValue, string name = null)
        {
            int testMin = Math.Max(0, min);

            int testMax = Math.Max(testMin, max);
            if (testMax == 0)
            {
                return s;
            }

            string group = content.Group(name);

            s.Append(group);
            if (testMin == 0)
            {
                s.Optional();
            }

            if (testMax > 1)
            {
                s.Group(defSeparator + group);

                int newMin = (min == 0) ? 0 : (min - 1);
                int newMax = (max == int.MaxValue) ? int.MaxValue : (max - 1);

                if (newMax == int.MaxValue)
                {
                    if (newMin == 0)
                    {
                        s.ZeroOrMore();
                    }
                    else if (newMin == 1)
                    {
                        s.OneOrMore();
                    }
                    else
                    {
                        s.Min(newMin);
                    }
                }
                else if (newMin == 0)
                {
                    if (newMax == 1)
                    {
                        s.Optional();

                    }
                    else
                    {
                        s.Max(newMax);
                    }
                }
                else if (newMin == newMax)
                {
                    if (newMin > 1)
                    {
                        s.Exactly(newMin);
                    }
                }
                else
                {
                    s.Between(newMin, newMax);
                }
            }
            return s;

        }

        public static StringBuilder MakeTextCell(this StringBuilder s, bool optional = true, string name = null)
        {
            s.OpenCell();
            s.OptionalGroup(textPattern, optional, name);
            s.CloseCell();
            return s.Whitespace();
        }

        public static StringBuilder MakeNumberCell(this StringBuilder s, bool optional = true, string name = null)
        {
            s.OpenCell();
            s.OptionalGroup(numPattern, optional, name);
            s.CloseCell();
            return s.Whitespace();
        }

        public static StringBuilder OptionalGroup(this StringBuilder s, string option, bool optional = true, string name = null)
        {
            return s.OptionalGroup(SB(option), optional, name);
        }

        public static StringBuilder OptionalGroup(this StringBuilder s, StringBuilder option, bool optional = true, string name = null)
        {
            var s1 = SB();
            if (optional || name != null)
            {
                s1.Group(option, name).Optional();
            }
            else
            {
                s1.Append(option);
            }
            return s.Append(s1);
        }

        public static StringBuilder Optional(this StringBuilder s, bool optional = true)
        {
            if (!optional)
            {
                return s;
            }
            return s.Append("?");
        }

        public static StringBuilder Ungreedy(this StringBuilder s, bool ungreedy = true)
        {
            if (!ungreedy)
            {
                return s;
            }
            return s.Append("?");
        }

        public static StringBuilder Group(this StringBuilder b, StringBuilder text, string name = null, int min = groupmin, int max = groupmax, bool ungreedy = false)
        {
            return b.Append(text.ToString().Group(name, min, max, ungreedy));
        }

        public static StringBuilder Group(this StringBuilder b, string text, string name = null, int min = groupmin, int max = groupmax, bool ungreedy = false)
        {
            return b.Append(text.Group(name, min, max, ungreedy));
        }

        public static string Group(this string text, string name = null, int min = groupmin, int max = groupmax, bool ungreedy = false)
        {
            var s = SB("(");
            if (name != null)
            {
                s.Append("?<" + name + ">");
            }
            s.Append(text + ")");
            s.Append(Quantifier(min, max, ungreedy));
            return s.ToString();
        }

        public static StringBuilder NumberGroup(this StringBuilder s, string group = null)
        {
            return s.Group(text: "-?[0-9]+", name: group);
        }

        public static StringBuilder PFNumberGroup(this StringBuilder s, string group = null)
        {
            return s.Group(text: "(\\+|-)?[0-9]+", name: group);
        }

        public static StringBuilder TextGroup(this StringBuilder s, string group = null, TextGroupOptions options = null, int min = 1, int max = -1)
        {
            TextGroupOptions opt = (options != null) ? options : new TextGroupOptions();

            string textPattern = "";
            if (opt.dash)
            {
                textPattern += "-";
            }
            if (opt.period)
            {
                textPattern += "\\.";
            }
            textPattern += "#×A-Za-z0-9'";
            if (opt.space)
            {
                textPattern += " ";
            }
            if (opt.colon)
            {
                textPattern += ":";
            }
            if (opt.semicolon)
            {
                textPattern += ";";
            }
            if (opt.plus)
            {
                textPattern += "+";
            }
            if (opt.comma)
            {
                textPattern += ",";
            }
            if (opt.paren)
            {
                textPattern += "()";
            }
            if (opt.square)
            {
                textPattern += "\\[\\]";
            }


            return s.Group(text: textPattern.Wrap("[]").Quantify(min, max), name: group);
        }



        public static StringBuilder TextOrUrlGroup(this StringBuilder s, string name = null, string altname = null)
        {
            string usealtname = (altname == null) ? name : altname;

            var x = SB().AddLink(SB().TextGroup(usealtname).ToString());

            return s.Append((new[] { x, SB().TextGroup(name) }).Options(true));
        }


        public static StringBuilder ZeroOrMore(this StringBuilder s, bool ungreedy = false)
        {
            return s.Append("*").Ungreedy(ungreedy);
        }


        public static StringBuilder Exactly(this StringBuilder s, int count)
        {
            return s.Append("{" + count + "}");
        }

        public static StringBuilder Min(this StringBuilder s, int min, bool ungreedy = false)
        {
            return s.Append("{" + min + ",}").Ungreedy(ungreedy);
        }

        public static StringBuilder Max(this StringBuilder s, int max, bool ungreedy = false)
        {
            return s.Append("{0," + max + "}").Ungreedy(ungreedy);
        }

        public static StringBuilder Between(this StringBuilder s, int min, int max, bool ungreedy = false)
        {
            return s.Append("{" + min + "," + max + "}").Ungreedy(ungreedy);
        }


        public static StringBuilder OneOrMore(this StringBuilder s, bool ungreedy = false)
        {
            return s.Append("+").Ungreedy(ungreedy);
        }

        public static StringBuilder MakeTextGroup(this StringBuilder s, string name, int min = groupmin, int max = groupmax, bool ungreedy = false)
        {
            return SB().Group(textPattern, name, min, max, ungreedy);
        }



        public static StringBuilder MakeURLContentGroup(this StringBuilder s, string urlname, string contentname)
        {
            return s.Append(MakeURLContentGroup(urlname, contentname));
        }

        public static string MakeURLContentGroup(string urlname, string contentname)
        {
            var b = SB();
            b.Append(hrefOpenStart);
            b.Append(urlPattern.Group(urlname));
            b.Append(hrefOpenEnd);
            b.Append(textPattern.Group(contentname));
            b.Append(hrefClose);
            b.Whitespace();

            return b.ToString();
        }

        public static StringBuilder AddSpan(this StringBuilder s, StringBuilder content = null, string cl = null, int min = groupmin, int max = groupmax, IEnumerable<WRAttribute> attributes = null)
        {
            return s.AddSpan(content.ToStringOrNull(), cl, min: min, max: max, attributes: attributes);
        }

        public static StringBuilder AddParagraph(this StringBuilder s, string content = null, string cl = null, int min = groupmin, int max = groupmax, IEnumerable<WRAttribute> attributes = null)
        {
            return s.AddTag("p", content, cl, min: min, max: max, attributes: attributes);
        }
        public static StringBuilder AddSpan(this StringBuilder s, string content = null, string cl = null, int min = groupmin, int max = groupmax, IEnumerable<WRAttribute> attributes = null)
        {
            return s.AddTag("span", content, cl, min: min, max: max, attributes: attributes);
        }



        public static StringBuilder AddTag(this StringBuilder s, string tag, StringBuilder content = null, string cl = null, bool classoptional = false, int min = groupmin, int max = groupmax, bool ungreedy = false, IEnumerable<WRAttribute> attributes = null)
        {
            return s.AddTag(tag, content.ToStringOrNull(), cl, classoptional, min, max, ungreedy, attributes);
        }

        public static StringBuilder AddTag(this StringBuilder s, string tag, string content = null, string cl = null, bool classoptional = false, int min = groupmin, int max = groupmax, bool ungreedy = false, IEnumerable<WRAttribute> attributes = null)
        {
            var ns = SB();

            bool empty = content == null;


            ns.AddOpenTag(tag, cl, classoptional: classoptional, attributes: attributes, terminate: !empty);

            if (empty)
            {
                string[] vals = { "/>", "></" + tag + ">" };
                ns.Append(vals.Options(group: true));
            }
            else
            {
                ns.Append(content);

                ns.AddCloseTag(tag);
            }

            return s.Group(ns.ToString(), min: min, max: max, ungreedy: ungreedy);
        }


        public static StringBuilder AddLink(this StringBuilder s, string content, string cl = null, bool classoptional = false, int min = groupmin, int max = groupmax, bool ungreedy = false, IEnumerable<WRAttribute> attributes = null)
        {
            List<WRAttribute> list = new List<WRAttribute>();
            list.Add(WRAttribute.Url());
            list.AddRangeIfNotNull(attributes);
            return s.AddTag("a", content, cl, classoptional, min, max, ungreedy, list);
        }

        public static StringBuilder Quantify(this StringBuilder s, int min = 1, int max = 1, bool ungreedy = false)
        {
            return s.Append(Quantifier(min, max, ungreedy));
        }

        public static string Quantify(this string s, int min = 1, int max = 1, bool ungreedy = false)
        {
            return s + Quantifier(min, max, ungreedy);
        }

        public static string Quantifier(int min = 1, int max = 1, bool ungreedy = false)
        {
            string ret;

            bool unlimited = (max == int.MaxValue || max < 0);

            if (min == 1 && max == 1)
            {
                return "";
            }

            if (min == 0 && max == 1)
            {
                ret = "?";
            }
            else if (min == 0 && unlimited)
            {
                ret = "*";
            }
            else if (min == 1 && unlimited)
            {
                ret = "+";
            }
            else if (min == max)
            {
                ret = min.Wrap("{,}");
            }
            else if (max == int.MaxValue)
            {
                ret = new[] { min.ToString(), "" }.Wrap("{,}");
            }
            else
            {
                ret = new[] { min, max }.Wrap("{,}");
            }

            if (ungreedy)
            {
                ret += "?";
            }
            return ret;
        }

        public static StringBuilder AddOpenTag(this StringBuilder s, string tag, string cl = null, bool classoptional = false, IEnumerable<WRAttribute> attributes = null, bool terminate = true)
        {

            s.Append("<").Append(tag);
            if (cl != null)
            {
                s.Append(SB().AppendAttribute("class", cl).Spaces().Wrap("()").Optional(classoptional));
            }
            if (attributes != null)
            {
                foreach (var at in attributes)
                {
                    s.AppendAttribute(at);
                }
            }
            if (terminate)
            {
                s.Append(">");
            }
            return s;
        }


        public static StringBuilder AppendAttribute(this StringBuilder s, WRAttribute attr)
        {
            return s.Append(MakeAttribute(attr));
        }

        public static StringBuilder AppendAttribute(this StringBuilder s, string attribute, string value, string group = null, bool optional = false)
        {
            return s.Append(MakeAttribute(attribute, value, group, optional));
        }

        public static string MakeAttribute(WRAttribute attr)
        {
            return MakeAttribute(attr.attribute, attr.value, attr.group, attr.optional);
        }

        public static string MakeAttribute(string attribute, string value, string group = null, bool optional = false)
        {
            string val;
            if (group == null)
            {
                val = value;
            }
            else
            {
                val = value.Group(group);
            }

            val = attribute + "=" + val.Wrap("\"\"");
            val = SB().Spaces().Append(val).ToString();
            if (!optional)
            {
                val = SB(val.Group()).Optional().ToString();

            }
            return val;
        }

        public static StringBuilder AddCloseTag(this StringBuilder s, string tag, string cl = null, bool whitespace = true)
        {

            s.Append("</").Append(tag).Append(">");

            return s.Whitespace(whitespace);
        }

        //*****STRING BUILDER*****//
        public static string ToStringOrNull<T>(this T s)
        {
            if (s == null)
            {
                return null;
            }
            else
            {
                return s.ToString();
            }
        }


        //****NON-HTML*****//

        public static bool GroupSuccess(this Match m, string group)
        {
            return m.Groups[group].Success;
        }

        public static bool AnyGroupSuccess(this Match m, IEnumerable<string> groups)
        {
            foreach (string s in groups)
            {
                if (m.GroupSuccess(s))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool HasGroup(this Match m, string group)
        {
            return m.Groups[group] != null;
        }


        public static string Value(this Match m, string group)
        {
            string val = null;

            if (m.Groups[group].Success)
            {
                val = m.Groups[group].Value;
            }

            return val;
        }

        public static string Values(this Match m, string group)
        {
            string val = null;

            if (m.Groups[group].Success)
            {
                val = m.Groups[group].Value;
            }

            return val;
        }



        public static int? IntValue(this Match m, string name)
        {
            if (m.Groups[name].Success)
            {
                if (int.TryParse(m.Groups[name].Value, out int val))
                {
                    return val;
                }
            }
            return null;
        }

        public static string Wrap(this int i, string wrapper)
        {
            return i.ToString().Wrap(wrapper);
        }

        public static string Wrap(this string s, string wrapper)
        {
            if (wrapper.Length < 2)
            {
                return s;
            }
            return wrapper[0] + s + wrapper[wrapper.Length - 1];

        }

        public static StringBuilder Wrap(this StringBuilder s, string wrapper)
        {

            if (s == null || wrapper.Length < 2)
            {
                return s;
            }
            return s.Insert(0, wrapper[0]).Append(wrapper[wrapper.Length - 1]);
        }

        public static string Wrap(this IEnumerable<string> list, string wrapper)
        {
            string spacer;
            if (wrapper.Length < 2)
            {
                return "";
            }
            if (wrapper.Length == 2)
            {
                spacer = "";
            }
            else
            {
                spacer = wrapper.Substring(1, wrapper.Length - 2);
            }


            StringBuilder b = new StringBuilder();

            bool first = true;
            foreach (string s in list)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    b.Append(spacer);
                }
                b.Append(s);
            }
            return b.ToString().Wrap(wrapper);
        }

        public static string Wrap(this IEnumerable<int> list, string wrapper)
        {
            return (from x in list select x.ToString()).Wrap(wrapper);
        }

        public static string Options(this IEnumerable<StringBuilder> list, bool group = false, string name = null, int min = 1, int max = 1)
        {

            return (from x in list select x.ToString()).Options(group, name, min, max);
        }

        public static string Options(this IEnumerable<string> list, bool group = false, string name = null, int min = 1, int max = 1)
        {
            string val = list.ToTokenString("|");
            if (group || min != 1 || max != 1)
            {
                if (name != null)
                {
                    val = "?" + name.Wrap("<>") + val;
                }

                val = val.Wrap("()");
                val += Quantifier(min, max);
            }
            return val;
        }


        public static StringBuilder LinksOrText(this StringBuilder sb, TextGroupOptions options = null, bool breaks = false)
        {

            return sb.Append(LinksOrText(options, breaks));

        }

        public static string LinksOrText(TextGroupOptions options = null, bool breaks = false)
        {
            var tg = SB().TextGroup(options: options);
            List<StringBuilder> list = new List<StringBuilder>();
            list.Add(SB().AddLink(content: tg.ToString(), cl: "[a-zA-Z0-9]+", classoptional: true));
            list.Add(tg.TextGroup());
            if (breaks)
            {
                list.Add(SB("<br>"));
            }


            return ((list.ToArray()).Options(min: 1, max: -1)).ToString();

        }

        public static StringBuilder PlainPLine(this StringBuilder sb, string name, string content, int min = 1, int max = 1)
        {

            return sb.Group(SB().AddOpenTag("p").AddTag("b", content: content).Append(SB().
                Group(SB().LinksOrText(new TextGroupOptions()))).AddCloseTag("p"), name: name, min: min, max: max);

        }

        public static StringBuilder PBoldLine(this StringBuilder sb, string name, string content, bool breaks = false, int min = 1, int max = 1)
        {
            return sb.Group(SB().AddOpenTag("p").AddBText(content, breaks: breaks).Group(SB().AddBText(TitleString, breaks: breaks), min: 0, max: -1).AddCloseTag("p"), name: name, min: min, max: max);
        }

        public static StringBuilder PBoldLineAnon(this StringBuilder sb, string name, string content, bool breaks = false, int min = 1, int max = 1)
        {
            return sb.Group(SB().LinksOrText().Group(SB().AddBText(TitleString, breaks: breaks), min: 0, max: -1).AddCloseTag("p"), name: name, min: min, max: max);
        }

        public const string TitleString = "[-()' a-zA-Z0-9]+";

        public static StringBuilder AddBText(this StringBuilder sb, string title, bool optional = false, bool breaks = false)
        {
            return sb.Group(SB().AddTag("b", content: title).Append(SB().LinksOrText(new TextGroupOptions(), breaks)), min: optional ? 0 : 1);
        }


        public static string ToTokenString(this IEnumerable<string> strings, string token)
        {
            if (strings == null)
            {
                return null;
            }
            string list = "";
            bool first = true;
            foreach (string str in strings)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    list += token;
                }
                list += str;
            }
            return list;
        }

        public static void AddRangeIfNotNull<T>(this List<T> list, IEnumerable<T> news)
        {
            if (news != null)
            {
                list.AddRange(news);
            }
        }


        public static void SpitFile(this StringBuilder text, string filename)
        {
            text.ToString().SpitFile(filename);

        }


        public static void SpitFile(this string text, string filename)
        {

            using (var s = File.Open(filename, FileMode.Create))
            using (var r = new StreamWriter(s))
            {
                r.Write(text);
            }
        }

        public static async Task<bool> SpitFileAsync(this string text, string filename)
        {

            using (var s = File.Open(filename, FileMode.Create))
            using (var r = new StreamWriter(s))
            {
                await r.WriteAsync(text);
            }
            return true;
        }

        public static string LoadFile(this string filename)
        {

            using (var s = File.Open(filename, FileMode.Open))
            using (var r = new StreamReader(s))
            {
                return r.ReadToEnd();
            }

        }



        public async static Task<string> DownloadFileAsync(this string url)
        {
            try
            {
                HttpClient client = new HttpClient();
                HttpResponseMessage resp = await client.GetAsync(url);

                return await resp.Content.ReadAsStringAsync();

            }
            catch (Exception)
            {

            }
            return null;
        }

        public static async Task<string> LoadFileAsync(this string filename)
        {

            using (var s = File.Open(filename, FileMode.Open))
            using (var r = new StreamReader(s))
            {
                return await r.ReadToEndAsync();
            }

        }

        public static async Task<string> DownloadToAsync(this string url, string filename)
        {
            string text = await url.DownloadFileAsync();
            if (text != null)
            {
                await text.SpitFileAsync(filename);
            }
            return text;
       
        }

        public static void SpitJSRegex(this StringBuilder text, string filename)
        {
            text.ToString().JSRegexFix().SpitFile(filename);

        }

        public static void SpitJSRegex(this string text, string filename)
        {
            text.JSRegexFix().SpitFile(filename);

        }




        static string JSRegexFix(this string text)
        {
            return text.Replace("/", "\\/");

        }

    }

    public static class AsyncUtils
    {
        private static Task<T> MakeTask<T>(Func<T> func)
        {
            return new Task<T>(func);
        }
        private static Task<T> MakeTask<T, X>(Func<X, T> func, X val)
        {
            return new Task<T>(() =>func(val) );
        }
        private static Task<T> MakeTask<T, X, Y>(Func<X, Y, T> func, X val, Y val2)
        {
            return new Task<T>(() => func(val, val2)); ;
        }
    }

    public class WRAttribute
    {
        public WRAttribute()
        {

        }

        public WRAttribute(string attribute, string value, string group = null, bool optional = false)
        {
            this.attribute = attribute;
            this.value = value;
            this.group = group;
            this.optional = optional;
        }

        public string attribute;
        public string value;
        public string group;
        public bool optional;

        public static WRAttribute Url(string attribute = "href", string group = null, bool optional = false)
        {
            return new WRAttribute(attribute, WRRegexUtils.UrlPattern, group, optional);
        }

    }

    public class TextGroupOptions
    {
        public bool dash = true;
        public bool period = true;
        public bool space = true;
        public bool comma = true;
        public bool colon = true;
        public bool semicolon = true;
        public bool paren = true;
        public bool plus = true;
        public bool square = true;

        public TextGroupOptions() { }

    }
}

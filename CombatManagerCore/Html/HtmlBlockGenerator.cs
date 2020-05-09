/*
 *  HtmlBlockGenerator.cs
 *
 *  Copyright (C) 2010-2012 Kyle Olson, kyle@kyleolson.com
 *
 *  This program is free software; you can redistribute it and/or
 *  modify it under the terms of the GNU General Public License
 *  as published by the Free Software Foundation; either version 2
 *  of the License, or (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 * 
 *  You should have received a copy of the GNU General Public License
 *  along with this program; if not, write to the Free Software
 *  Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 *
 */

using System;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using CombatManager;

using System.Collections.Generic;

namespace CombatManager.Html
{
	public static class HtmlBlockCreator
	{

		private static string _Header;
		private static string _Footer;

		private static Dictionary<string, string> _CSSFiles;

		public static (string, string) StyleNoTop = ("border-top", "none");
		public static (string, string) StyleNoBottom = ("border-bottom", "none");

		public static HashSet<string> MustCloseTags = new HashSet<string>()
		{
			"div",
			"script",
		};

		private static string Header
		{
			get
			{
				if (_Header == null)
				{
					using (StreamReader r = CreateReader("HtmlBlockHeader.txt"))
					{
						_Header = r.ReadToEnd();
					}

				}
				return _Header;
			}
		}

		private static string Footer
		{
			get
			{
				if (_Footer == null)
				{

					using (StreamReader r = CreateReader("HtmlBlockFooter.txt"))
					{
						_Footer = r.ReadToEnd();
					}
				}
				return _Footer;
			}
		}

		private static Dictionary<string, string> SavedFiles
		{
			get
			{
				if (_CSSFiles == null)
				{
					_CSSFiles = new Dictionary<string, string>();
				}
				return _CSSFiles;
			}
		}

		static StreamReader CreateReader(string filename)
		{
#if ANDROID
            Stream io = CombatManager.CoreContext.Context.Assets.Open(filename);
            
            return new StreamReader(io);
#else
			try
			{
				return new StreamReader(filename);
			}
			catch (FileNotFoundException)
			{
				return null;
			}
#endif
		}


		public static StringBuilder AppendHtml(this StringBuilder builder, string text)
		{
			return builder.Append(HttpUtility.HtmlEncode(text));
		}
		public static StringBuilder AppendOpenTag(this StringBuilder builder, string tag, string cl = null, string id = null, 
			(string, string)[] attributes = null, 
			(string, string) [] styles=null, HtmlStyle [] htmlStyles = null)
		{
			string text = "<" + tag;
			text += BuildAttributes(cl, id, attributes.ToList(), styles.ToList(), htmlStyles);
			text += ">";
			return builder.Append(text);
		}
		public static StringBuilder AppendCloseTag(this StringBuilder builder, string tag)
		{
			return builder.Append("</" + tag + ">");
		}

		public static StringBuilder AppendSpan(this StringBuilder builder, string content, string cl)
		{
			builder.AppendEscapedTag("span", content, cl);

			return builder;
		}


		public static StringBuilder AppendTag(this StringBuilder builder, string tag, string content = null, bool escaped = false,
			string classname = null, string id = null, (string, string)[] attributes = null, (string, string)[] styles = null, HtmlStyle[] htmlStyles = null)
		{
			return builder.Append(CreateTag(tag, content, escaped, classname, id, attributes, styles, htmlStyles));
		}


		public static string CreateTag(string tag, string content = null, bool escaped = false, 
			string classname = null, string id = null, (string, string)[] attributes = null, (string, string)[] styles = null, HtmlStyle[] htmlStyles = null)
		{
			StringBuilder text = new StringBuilder();
			text.Append("<" + tag);
			text.Append(BuildAttributes(classname, id, attributes.ToList(), styles.ToList(), htmlStyles));

			if (content == null && !MustCloseTags.Contains(tag))
			{
				text.Append("/>");
			}
			else
			{
				text.Append(">");

				if (content != null)
				{
					if (escaped)
					{
						text.Append(HttpUtility.HtmlEncode(content));
					}
					else
					{
						text.Append(content);
					}
				}

				text.Append("</" + tag + ">");
			}
			return text.ToString();

		}

		public static string BuildAttributes(string classname = null, string id = null, 
			List<(string, string)> attributes = null, List<(string, string)> styles = null,
			HtmlStyle[] htmlStyles = null)
		{
			var finalAttributes = attributes.CopyOrCreate();
			List<(string, string)> finalStyles = styles.CopyOrCreate();
			if (classname != null)
			{
				finalAttributes.Add(("class", classname));
			}
			if (id != null)
			{
				finalAttributes.Add(("id", id));
			}
			if (htmlStyles != null)
			{
				finalStyles.AddRange(ParseHtmlStyles(htmlStyles));
			}
			if (finalStyles != null && finalStyles.Count > 0)
			{
				finalAttributes.Add(("style", CreateStyles(finalStyles)));
			}
			return CreateAttributes(finalAttributes);
		}

		public static string CreateAttributes(List<(string, string)> attributes)
		{
			StringBuilder text = new StringBuilder();
			foreach (var att in attributes)
			{
				text.Append(" " + att.Item1 + "=\"" +att.Item2 + "\"");
			}
			return text.ToString();


		}

		public static string CreateStyles(List<(string, string)> styles)
		{
			StringBuilder text = new StringBuilder();
			foreach (var style in styles)
			{
				text.Append(style.Item1 + ":" + style.Item2 + ";");
			}
			return text.ToString();
		}





		public static StringBuilder AppendEscapedTag(this StringBuilder builder, string tag, string content, string cl = null)
		{
			return builder.Append(CreateEscapedTag(tag, content, cl));
		}


		public static string CreateEscapedTag(string tag, string content, string cl = null)
		{
			string text = "<" + tag;

			if (cl != null)
			{
				text += " class=\"" + cl + "\"";
			}
			text += ">";
			text += HttpUtility.HtmlEncode(content);
			text += "</" + tag + ">";

			return text;
		}


		public static StringBuilder CreateHtmlHeader(string css = null, string cl = null)
		{
			return new StringBuilder().CreateHtmlHeader(css, cl);
		}

		public static StringBuilder CreateHtmlHeader(this StringBuilder builder, string css = null, string cl = null)
		{
			String cssText;
			if (css == null)
			{
				cssText = GetDefaultStyleTag();
			}
			else
			{
				cssText = GetCssFileStyleTag(css);

			}
		    builder.Append(Header.Replace("%%STYLE_AREA%%", cssText));
			return builder.AppendOpenTag("body", cl:cl);

		}

		public static string CreateSimpleHtmlPage(string content, string css = null, string cl = null)
		{
			var builder = CreateHtmlHeader(css, cl);
			builder.AppendHtml(content);
			builder.CreateHtmlFooter();
			return builder.ToString();
		}

		private static string GetCssFileStyleTag(string css)
		{
			return CreateTag(tag: "link", attributes : new []{ ("rel", "stylesheet"), ("type", "text/css"), ("href", "/www/css/" + css + ".css") });
		}

		private static string GetDefaultStyleTag()
		{
			StringBuilder builder = new StringBuilder();
			builder.AppendTag("style", GetSaveTextFile("WebStyles.css"));
			return builder.ToString();
		}

		public static string GetSaveTextFile(string css)
		{
			if (!SavedFiles.TryGetValue(css, out string fileText))
			{
				using (StreamReader r = CreateReader(css ))
				{
					if (r != null)
					{
						fileText = r.ReadToEnd();
					}
					else
					{
						fileText = "";
					}
				}

			}
			return fileText;
		}



		public static StringBuilder CreateHtmlFooter(this StringBuilder builder)
		{
			
			return builder.Append(Footer);
		}
		
		public static StringBuilder CreateItemIfNotNull(this StringBuilder builder, string title, string value)
		{
			return CreateItemIfNotNull (builder, title, true, value, null, true);
		}
		
		public static StringBuilder AppendLineBreak(this StringBuilder builder)
		{
			return builder.AppendOpenTag ("br");	
		}



		public static StringBuilder StartParagraph(this StringBuilder builder, string cl = null, string id = null)
		{
			builder.AppendOpenTag("p", cl, id);
			return builder;
		}

		public static StringBuilder EndParagraph(this StringBuilder builder)
		{
			builder.AppendCloseTag("p");
			return builder;
		}
		
		public static String HtmlEncode(this string text)
		{
			int startSpace = 0;
			for (int i=0; i<text.Length; i++)
			{
				if (text[i]  != ' ')
				{
					startSpace = i;
					break;
				}
			}
			int endSpace = 0;
			if (startSpace < text.Length)
			{
				for (int i=0; i<text.Length; i++)
				{
					if (text[text.Length - i - 1] != ' ')
					{
						endSpace = i;
						break;
					}
				}
			}

			string val = text;
			if (startSpace > 0 || endSpace > 0)
			{
				val = val.Substring(startSpace, val.Length - startSpace - endSpace);	
			}
			
			val = HttpUtility.HtmlEncode(val);
			
			for (int i=0; i<startSpace; i++)
			{
				val = "&nbsp;" + val;
			}
			for (int i=0; i<endSpace; i++)
			{
				val += "&nbsp;";
			}
			
			return val;
			
		}
	
		
		public static StringBuilder CreateItemIfNotNull(this StringBuilder builder, string title, bool boldTitle, string value, string end, bool linebreak)
		{
			if (!String.IsNullOrEmpty(value))
			{
				if (title != null)
				{
					if (boldTitle)
					{
						builder.AppendOpenTag("sp", "bolded");
					}
					
					builder.Append(HttpUtility.HtmlEncode(title));
					
					if (boldTitle)
					{
						builder.AppendCloseTag("sp");
					}
				}
				
				
				builder.Append(HttpUtility.HtmlEncode(value));
				
				if (end != null)
				{
					int startSpace = 0;
					for (int i=0; i<end.Length; i++)
					{
						if (end[i]  != ' ')
						{
							startSpace = i;
							break;
						}
					}
					int endSpace = 0;
					if (startSpace < end.Length)
					{
						for (int i=0; i<end.Length; i++)
						{
							if (end[end.Length - i - 1] != ' ')
							{
								endSpace = i;
								break;
							}
						}
					}
					
					for (int i=0; i<startSpace; i++)
					{
						builder.AppendSpace();
					}
					
					builder.AppendHtml(end);
					
					for (int i=0; i<endSpace; i++)
					{
						builder.AppendSpace();
					}
				}
				
				if (linebreak)
				{
					builder.AppendLineBreak();
				}
			}
			
			return builder;
		}
		
		public static StringBuilder CreateHeader(this StringBuilder builder, string name)
		{
			return builder.CreateHeader(name, null);
		}
		
        public static StringBuilder CreateHeader(this StringBuilder builder, string name, string extra)
        {
            return builder.CreateHeader(name, extra, "h1");

        }

	    public static StringBuilder CreateHeader(this StringBuilder builder, string name, string extra, string header)
		{
			if (extra == null)
			{
				builder.AppendEscapedTag(header, name);
				
			}
			else
			{
				builder.AppendOpenTag(header);
				
				builder.AppendOpenTag("table", "headertable");
				
				builder.AppendOpenTag("tr");
				
				builder.AppendEscapedTag("th", name, "headertablename");
				builder.AppendEscapedTag("th", extra, "headertableextra");
				
				builder.AppendCloseTag("tr");
				
				
				
				builder.AppendCloseTag ("table");
				
				builder.AppendCloseTag(header);
			}
						return builder;
		}

		
		public static StringBuilder AppendSpace(this StringBuilder builder, int count = 1)
		{
			for (int i = 0; i < count; i++)
			{
				builder.Append("&nbsp;");
			}
			return builder;
			
		}
		
		public static StringBuilder CreateSectionHeader(this StringBuilder builder, string text)
		{
			return builder.AppendEscapedTag("p", text, "sectionheader");
		}
		
        public static  StringBuilder CreateMultiValueLine(this StringBuilder blocks, List<TitleValuePair> values, string seperator)
        {
			bool itemAdded = false;

            foreach (TitleValuePair pair in values)
            {
                if (pair.Value != null && pair.Value.Length > 0)
                {
                    if (itemAdded && seperator != null)
                    {
                        blocks.AppendHtml(seperator);
                    }

                    blocks.CreateItemIfNotNull(pair.Title, true, pair.Value, null, false);

                    itemAdded = true;
                }
            }

            if (itemAdded)
            {
            	blocks.AppendLineBreak();
            }

           return blocks;
        }

		public static StringBuilder AppendCRLF(this StringBuilder builder)
		{
			return builder.Append("\r\n");
		}

        public static StringBuilder AppendWebImg(this StringBuilder builder, string img, string type = "image",  string cl = null,
			string id = null, (string, string)[] attributes = null,
			(string, string)[] styles = null, HtmlStyle[] htmlStyles = null)
        {
			string name = "/img/" + ((type == null) ? "image" : type) + "/" + img;
			return builder.AppendTag("img", attributes: attributes.CombineEvenNull(new[] { ("src", name) }), classname : cl, id:id,
				styles:styles, htmlStyles:htmlStyles);


        }

		public static StringBuilder AppendWebIcon(this StringBuilder builder, string name, string cl = "icon",
			string id = null, (string, string)[] attributes = null,
			(string, string)[] styles = null, HtmlStyle[] htmlStyles = null)
		{
			return builder.AppendWebImg(name, type: "icon", cl: cl, id: id,
				attributes: attributes, styles: styles, htmlStyles: htmlStyles);
		}


		public static StringBuilder AppendSmallIcon(this StringBuilder builder, string name, string cl = null, 
			string id = null, (string, string)[] attributes = null, 
			(string, string)[] styles = null, HtmlStyle[] htmlStyles = null)
		{
			string src = "Images / External / " + name + " - 16.png";
			(string, string)[] finalAttr = attributes.CombineEvenNull(new[] { ("src", src) });


			return builder.AppendTag("img", classname: cl, id:id, attributes: finalAttr, styles:styles, htmlStyles:htmlStyles);
        }

		public static StringBuilder AppendDiv(this StringBuilder builder, string content,
			string classname = null, string id = null, (string, string)[] attributes = null, (string, string)[]styles = null)
		{
			return builder.AppendTag("div", content:content, escaped:true, classname:classname, id:id, attributes: attributes, styles:styles);
		}


		public static StringBuilder AppendSpan(this StringBuilder builder, string content,
			string classname = null, string id = null, (string, string)[] attributes = null, (string, string)[] styles = null)
		{
			return builder.AppendTag("span", content: content, escaped: true, classname: classname, id: id, attributes: attributes, styles: styles);
		}

		public static List<(string, string)> ParseHtmlStyles(HtmlStyle[] styles)
		{
			List<(string, string)> allStyles = new List<(string, string)>();
			foreach (HtmlStyle style in styles)
			{
				allStyles.AddRange(style.ToArray());
			}
			return allStyles;
		}

		public static StringBuilder AppendSelect(this StringBuilder builder, IEnumerable<(string, string)> options, string classname = null, string id = null, (string, string)[] attributes = null, (string, string)[] styles = null, HtmlStyle [] htmlStyles = null)
		{
			 builder.AppendOpenTag("select",  cl: classname, id: id, attributes: attributes, styles: styles, htmlStyles: htmlStyles);

			foreach (var s in options)
			{
				builder.AppendTag("option", attributes: new[] { ("value", s.Item1) }, content:s.Item2, escaped:true);
			}

			builder.AppendCloseTag("select");

			return builder;
		}
	}
	
	

}


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
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Collections.Generic;

namespace CombatManager
{
	public static class HtmlBlockCreator
	{
		
		private static string _Header;
		private static string _Footer;
		
		public static string Header
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
		
		public static string Footer
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
		
        static StreamReader CreateReader(string filename)
        {
#if ANDROID
            Stream io = CombatManager.CoreContext.Context.Assets.Open(filename);
            
            return new StreamReader(io);
#else
            return new StreamReader(filename);
#endif
        }


		public static StringBuilder AppendHtml(this StringBuilder builder, string text)
		{
			return builder.Append(HttpUtility.HtmlEncode(text));
		}
		public static StringBuilder AppendOpenTag(this StringBuilder builder, string tag, string cl = null)
		{
			string text = "<" + tag;
			if (cl != null)
			{
				text += " class=\"" + cl + "\"";

			}
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
		
		public static string CreateTag(string tag, string content, string cl = null)
		{
			string text = "<" + tag;
			if (cl != null)
			{
				text += " class=\"" + cl + "\"";
			}
			text += ">";
			text += content;
			text += "</" + tag + ">";
			
			
			return text;
		}



		public static StringBuilder AppendTag(this StringBuilder builder, string tag, string content, string cl = null)
		{
			return builder.Append (CreateTag (tag, content, cl));
		}
		
		
		
		public static StringBuilder AppendEscapedTag(this StringBuilder builder, string tag, string content, string cl = null)
		{
			return builder.Append (CreateEscapedTag (tag, content, cl));
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


		
		
		public static StringBuilder CreateHtmlHeader(this StringBuilder builder)
		{
			return builder.Append(Header);	
			
			
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



		public static StringBuilder StartParagraph(this StringBuilder builder, string cl = null)
		{
			builder.AppendOpenTag("p", cl);
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
		
		public static bool NotNullString(this string str)
		{
			return str != null && str.Trim() != "";
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

        public static void AppendImg(this StringBuilder builder, string img, string cl = null)
        {

			String text = "<img src=\"" + img + "\"";

			if (cl != null)
			{
				text += " class=\"" + cl + "\"";
			}

			text += "/>";

            builder.Append(text);

        }


		public static void AppendSmallIcon(this StringBuilder builder, string name)
        {

            String text = "<img src=\"Images/External/" + name + "-16.png\"/>";

            builder.Append(text);

        }
		
	}
	
	
    public class TitleValuePair
    {
        public string Title;
        public string Value;
    }

}


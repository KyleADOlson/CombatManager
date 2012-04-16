using System;
using System.Text;
using CombatManager;
using System.Collections.Generic;


namespace CombatManagerMono
{
	public class GenericHtmlCreator
	{
		public GenericHtmlCreator ()
		{
		}

        public static string CreateHtml(String title, string text)
        {
        	StringBuilder blocks = new StringBuilder();
			blocks.CreateHtmlHeader();
			
            blocks.CreateHeader(title);
                
			blocks.AppendHtml(text);
			
			blocks.CreateHtmlFooter();


            return blocks.ToString();

        }
	}
}


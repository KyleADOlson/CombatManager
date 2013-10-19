using System;
using System.Text;

namespace CombatManager
{
    public static class ConditionHtmlCreator
    {
        public static string CreateHtml(Condition item)
        {
            return CreateHtml(item, true);
        }

        public static string CreateHtml(Condition item, bool showTitle)
        {
            switch (item.Type)
            {
            case ConditionType.Condition:
                return PlainConditionHtml(item);
            case ConditionType.Spell:
                return  SpellHtmlCreator.CreateHtml(item.Spell);
            case ConditionType.Afflicition:
                return AfflictionHtml(item.Affliction);
                case ConditionType.Custom:
                break;
            }
            return "";
        }

        public static string PlainConditionHtml(Condition item)
        {
            StringBuilder blocks = new StringBuilder();
            blocks.CreateHtmlHeader();

            blocks.AppendEscapedTag("p", "bolded", item.Name);
            blocks.AppendOpenTag("p");
            blocks.AppendHtml(item.Text);
            blocks.AppendCloseTag("p");
            blocks.CreateHtmlFooter();
            return blocks.ToString();
        }

        public static string AfflictionHtml(Affliction af)
        {
            StringBuilder blocks = new StringBuilder();
            blocks.CreateHtmlHeader();

            blocks.AppendEscapedTag("p", "bolded", af.Name);
            blocks.AppendOpenTag("p");
            blocks.AppendHtml(af.Text);
            blocks.AppendCloseTag("p");


            blocks.CreateHtmlFooter();

            return blocks.ToString();
        }

    }
}


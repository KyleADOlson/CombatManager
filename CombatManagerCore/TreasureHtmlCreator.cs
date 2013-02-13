using System;
using System.Text;
using CombatManager;
using System.Collections.Generic;


namespace CombatManager
{
    public class TreasureHtmlCreator
    {
        public TreasureHtmlCreator ()
        {
        }
        
        
        public static string CreateHtml(Treasure treasure)
        {
            return CreateHtml(treasure, true);
        }

        public static string CreateHtml(Treasure treasure, bool showTitle)
        {

            StringBuilder blocks = new StringBuilder();

            blocks.CreateHtmlHeader();



            if (treasure.Coin != null && treasure.Coin.GPValue != 0)
            {

                blocks.CreateSectionHeader("Coin");

                blocks.AppendOpenTag("p");
                blocks.AppendHtml(treasure.Coin.ToString());
                blocks.AppendCloseTag("p");

            }
            if (treasure.Goods.Count > 0)
            {

                SortedDictionary<string, List<Good>> groupedItems = new SortedDictionary<string, List<Good>>();

                foreach (Good t in treasure.Goods)
                {
                    if (!groupedItems.ContainsKey(t.Name))
                    {
                        groupedItems[t.Name] = new List<Good>();
                    }
                    groupedItems[t.Name].Add(t);
                }

                StringBuilder subBlocks = new StringBuilder();


                subBlocks.AppendOpenTag("p");

                bool first = true;
                decimal goodsVal = 0;
                foreach (List<Good> list in groupedItems.Values)
                {
                    if (!first)
                    {
                        subBlocks.AppendLineBreak();
                    }


                    if (list.Count > 1)
                    {
                        subBlocks.AppendHtml(list.Count + " x ");
                    }
                    

                    Good good = list[0];

                    subBlocks.AppendHtml(good.Name);

                    bool firstTreasure = true;
                    subBlocks.AppendHtml(" (");
                    decimal tv = 0;
                    bool allSame = true;

                    foreach (Good ti in list)
                    {
                        goodsVal += ti.Value;

                        if (firstTreasure)
                        {
                            tv = ti.Value;
                            firstTreasure = false;
                        }
                        else
                        {
                            if (tv != ti.Value)
                            {
                                allSame = false;
                            }
                        }
                    }

                    if (allSame && list.Count > 1)
                    {
                        subBlocks.AppendHtml(good.Value + " gp each)");
                    }
                    else
                    {

                        firstTreasure = true;
                        foreach (Good ti in list)
                        {
                            if (!firstTreasure)
                            {
                                subBlocks.AppendHtml(", ");
                            }
                            firstTreasure = false;

                            subBlocks.AppendHtml(ti.Value + " gp");
                        }
                        subBlocks.AppendHtml(")");
                    }

                    first = false;
                }

                subBlocks.AppendCloseTag("p");

                blocks.CreateSectionHeader("Goods (" + goodsVal + " gp)");

                blocks.Append (subBlocks.ToString());

            }
            if (treasure.Items.Count > 0)
            {

                SortedDictionary<string, List<TreasureItem>> groupedItems = new SortedDictionary<string, List<TreasureItem>>();

                foreach (TreasureItem t in treasure.Items)
                {
                    if (!groupedItems.ContainsKey(t.Name))
                    {
                        groupedItems[t.Name] = new List<TreasureItem>();
                    }
                    groupedItems[t.Name].Add(t);
                }

                
                StringBuilder subBlocks = new StringBuilder();


                subBlocks.AppendOpenTag("p");


                bool first = true;
                string type = "";
                decimal itemsVal = 0;
                foreach (List<TreasureItem> list in groupedItems.Values)
                {
                    TreasureItem item = list[0];

                    if (item.Type != type)
                    {
                        if (type != "")
                        {
                            subBlocks.AppendLineBreak();
                        }
                        type = item.Type;
                        subBlocks.AppendHtmlSpan("itemspace", type);
                        first = true;
                    }
                    if (first)
                    {

                        subBlocks.AppendLineBreak();
                    }

                    if (list.Count > 1)
                    {
                        subBlocks.AppendHtml(list.Count + " x ");
                    }
                    

                   
                    if (item.MagicItem != null)
                    {

                        subBlocks.AppendHtml(item.MagicItem.Name);
                    }
                    else if ((item.Type == "Scroll" || item.Type == "Wand" || item.Type == "Potion") && item.Spell != null)
                    {
                        if (item.Type == "Scroll")
                        {
                            subBlocks.AppendHtml("Scroll of ");
                        }
                        else if (item.Type == "Potion")
                        {
                            subBlocks.AppendHtml("Potion of ");
                        }
                        else
                        {
                            subBlocks.AppendHtml("Wand of ");
                        }

                        subBlocks.AppendHtml(item.Spell.Name);
                    }
                    else
                    {
                        subBlocks.AppendHtml(item.Name);
                    }

                    bool firstTreasure = true;
                    subBlocks.AppendHtml(" (");
                    decimal tv = 0;
                    bool allSame = true;
                    
                    foreach (TreasureItem ti in list)
                    {
                        itemsVal += ti.Value;

                        if (firstTreasure)
                        {
                            tv = ti.Value;
                            firstTreasure = false;
                        }
                        else
                        {
                            if (tv != ti.Value)
                            {
                                allSame = false;
                            }
                        }
                    }

                    if (allSame && list.Count > 1)
                    {
                        subBlocks.AppendHtml(item.Value + " gp each)");
                    }
                    else
                    {

                        firstTreasure = true;
                        foreach (TreasureItem ti in list)
                        {
                            if (!firstTreasure)
                            {
                                subBlocks.AppendHtml(", ");
                            }
                            firstTreasure = false;

                            subBlocks.AppendHtml(ti.Value + " gp");
                        }
                        subBlocks.AppendHtml(")");
                    }


                    subBlocks.AppendLineBreak();
                    first = false;
                }

                subBlocks.AppendCloseTag("p");

                blocks.CreateSectionHeader("Items (" + itemsVal + " gp)");
                blocks.Append (subBlocks.ToString());

            }

            blocks.CreateSectionHeader("Total Value " + treasure.TotalValue.ToString("0.##") + " gp");


            return blocks.ToString();
 

        }
    }
}


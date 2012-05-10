/*
 *  TreasureBlockCreator.cs
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

﻿using System;
using System.Data;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Serialization;

namespace CombatManager
{
    class TreasureBlockCreator : BlockCreator
    {

        DocumentLinkHander _LinkHandler;

        public TreasureBlockCreator(FlowDocument document)
            : this (document, null)
        {
        }

        public TreasureBlockCreator(FlowDocument document, DocumentLinkHander _LinkHandler)
            : base(document)
        {
            this._LinkHandler = _LinkHandler;
        }

        public List<Block> CreateBlocks(Treasure treasure)
        {


            List<Block> blocks = new List<Block>();


            if (treasure.Coin != null && treasure.Coin.GPValue != 0)
            {

                blocks.AddRange(CreateSectionHeader("Coin", true));

                Paragraph coin = new Paragraph();
                coin.Inlines.Add(treasure.Coin.ToString());
                coin.Margin = new Thickness(0, 3, 0, 16);

                blocks.Add(coin);

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


                Paragraph goods = new Paragraph();
                goods.Margin = new Thickness(0, 3, 0, 16);
                bool first = true;
                decimal goodsVal = 0;
                foreach (List<Good> list in groupedItems.Values)
                {
                    if (!first)
                    {
                        goods.Inlines.Add(new LineBreak());
                    }


                    if (list.Count > 1)
                    {
                        goods.Inlines.Add(list.Count + " x ");
                    }
                    

                    Good good = list[0];

                    goods.Inlines.Add(good.Name);

                    bool firstTreasure = true;
                    goods.Inlines.Add(" (");
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
                        goods.Inlines.Add(good.Value + " gp each)");
                    }
                    else
                    {

                        firstTreasure = true;
                        foreach (Good ti in list)
                        {
                            if (!firstTreasure)
                            {
                                goods.Inlines.Add(", ");
                            }
                            firstTreasure = false;

                            goods.Inlines.Add(ti.Value + " gp");
                        }
                        goods.Inlines.Add(")");
                    }

                    first = false;
                }


                blocks.AddRange(CreateSectionHeader("Goods (" + goodsVal + " gp)", true));

                blocks.Add(goods);
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



                Paragraph items = new Paragraph();
                items.Margin = new Thickness(0, 3, 0, 16);
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
                            items.Inlines.Add(new LineBreak());
                        }
                        type = item.Type;
                        items.Inlines.Add(new Italic(new Underline(new Run(type))));
                        first = true;
                    }
                    if (first)
                    {

                        items.Inlines.Add(new LineBreak());
                    }

                    if (list.Count > 1)
                    {
                        items.Inlines.Add(list.Count + " x ");
                    }
                    

                   
                    if (item.MagicItem != null)
                    {

                        Hyperlink link = new Hyperlink(new Run(item.Name));
                        link.Tag = item.Name;
                        link.Click += new RoutedEventHandler(MagicItemLinkClicked);
                        link.DataContext = item.MagicItem;
                        ToolTip t = (ToolTip)App.Current.MainWindow.FindResource("ObjectToolTip");

                        if (t != null)
                        {

                            ToolTipService.SetShowDuration(link, 360000);
                            link.ToolTip = t;
                            link.ToolTipOpening += new ToolTipEventHandler(link_ToolTipOpening);

                        }
                        items.Inlines.Add(link);
                    }
                    else if ((item.Type == "Scroll" || item.Type == "Wand" || item.Type == "Potion") && item.Spell != null)
                    {
                        if (item.Type == "Scroll")
                        {
                            items.Inlines.Add("Scroll of ");
                        }
                        else if (item.Type == "Potion")
                        {
                            items.Inlines.Add("Potion of ");
                        }
                        else
                        {
                            items.Inlines.Add("Wand of ");
                        }

                        Hyperlink link = new Hyperlink(new Run(item.Spell.name));
                        link.Tag = item.Spell.name;
                        link.Click += new RoutedEventHandler(MagicItemLinkClicked);
                        link.DataContext = item.Spell;
                        ToolTip t = (ToolTip)App.Current.MainWindow.FindResource("ObjectToolTip");

                        if (t != null)
                        {

                            ToolTipService.SetShowDuration(link, 360000);
                            link.ToolTip = t;
                            link.ToolTipOpening += new ToolTipEventHandler(link_ToolTipOpening);

                        }
                        items.Inlines.Add(link);
                    }
                    else
                    {
                        items.Inlines.Add(item.Name);
                    }

                    bool firstTreasure = true;
                    items.Inlines.Add(" (");
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
                        items.Inlines.Add(item.Value + " gp each)");
                    }
                    else
                    {

                        firstTreasure = true;
                        foreach (TreasureItem ti in list)
                        {
                            if (!firstTreasure)
                            {
                                items.Inlines.Add(", ");
                            }
                            firstTreasure = false;

                            items.Inlines.Add(ti.Value + " gp");
                        }
                        items.Inlines.Add(")");
                    }


                    items.Inlines.Add(new LineBreak());
                    first = false;
                }

                blocks.AddRange(CreateSectionHeader("Items (" + itemsVal + " gp)", true));
                blocks.Add(items);
            }

            blocks.AddRange(CreateSectionHeader("Total Value " + treasure.TotalValue + " gp"));


            return blocks;
        }


        void link_ToolTipOpening(object sender, ToolTipEventArgs e)
        {
            Hyperlink l = (Hyperlink)sender;
            ((ToolTip)l.ToolTip).DataContext = l.DataContext;
        }

        void MagicItemLinkClicked(object sender, RoutedEventArgs e)
        {

            if (_LinkHandler != null)
            {
                Hyperlink link = (Hyperlink)sender;

                string feat = (string)link.Tag;
                string type = link.DataContext.GetType().Name;

                _LinkHandler(this, new DocumentLinkEventArgs(feat, type));

            }
        }
    }
}
